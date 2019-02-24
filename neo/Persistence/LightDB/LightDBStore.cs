using System;
using System.Reflection;
using Neo.IO.Caching;
using Neo.Ledger;
using Neo.Cryptography.ECC;
using Neo.IO.Wrappers;
using Neo.IO.Data.LightDB;
using Neo.Wallets;
using System.Threading.Tasks;
using NEL.Common;
using NEL.Pipeline;
using NEL.Peer.Tcp;

namespace Neo.Persistence.LightDB
{
    public class LightDBStore : Store, IDisposable
    {
        private DB db;

        private string serverAddress;
        private string serverPort;
        private string serverPath;

        public int dumpInfo_splitcount;
        public int dumpInfo_splitindex;
        public bool dumpInfo_onlylocal = false;

        public LightDBStore(string address,string port,string path, string dumpInfoPath = null, bool _dumpOnlyLocal = false, int _dumpInfo_splitcount = 1, int _dumpInfo_splitindex = 0)
        {
            //设置dumpinfo的一些参数
            SmartContract.Debug.DumpInfo.Path = dumpInfoPath;
            this.dumpInfo_onlylocal = _dumpOnlyLocal;
            this.dumpInfo_splitcount = _dumpInfo_splitcount;
            this.dumpInfo_splitindex = _dumpInfo_splitindex;
            this.serverAddress = address;
            this.serverPort = port;
            this.serverPath = path;

            if (string.IsNullOrEmpty(SmartContract.Debug.DumpInfo.Path) == false)
            {
                if (System.IO.Directory.Exists(SmartContract.Debug.DumpInfo.Path) == false)
                    System.IO.Directory.CreateDirectory(SmartContract.Debug.DumpInfo.Path);

            }
            else
            {
                SmartContract.Debug.DumpInfo.Path = null;
            }

            db = new DB();
            db.Open(serverAddress, serverPort, serverPath);
            var snapshot = db.CurSnapshot;
            Slice value = snapshot.GetValue(SliceBuilder.Begin(DataEntryPrefix.SYS_Version));
            if (value.buffer.Length > 0 && Version.TryParse(value.ToString(), out Version version) && version >= Version.Parse("2.5.4"))
                return;
            WriteBatch batch = db.CreateNewWriteBatch();
            {
                var it = snapshot.CreateNewIterator();
                it.SeekToFirst();
                while (it.MoveNext())
                {
                    var cur = it.Current;
                    batch.Delete(cur);
                }
            }
            batch.Put(SliceBuilder.Begin(Prefixes.SYS_Version), Assembly.GetExecutingAssembly().GetName().Version.ToString());
            batch.Write();
        }

        public void Dispose()
        {
            //db.Dispose();
        }

        public override DataCache<UInt160, AccountState> GetAccounts()
        {
            return new DbCache<UInt160, AccountState>(db, null, null, Prefixes.ST_Account);
        }

        public override DataCache<UInt256, AssetState> GetAssets()
        {
            return new DbCache<UInt256, AssetState>(db, null, null, Prefixes.ST_Asset);
        }

        public override DataCache<UInt256, BlockState> GetBlocks()
        {
            return new DbCache<UInt256, BlockState>(db, null, null, Prefixes.DATA_Block);
        }

        public override DataCache<UInt160, ContractState> GetContracts()
        {
            return new DbCache<UInt160, ContractState>(db, null, null, Prefixes.ST_Contract);
        }

        public override Snapshot GetSnapshot()
        {
            return new DbSnapshot(db);
        }

        public override DataCache<UInt256, SpentCoinState> GetSpentCoins()
        {
            return new DbCache<UInt256, SpentCoinState>(db, null, null, Prefixes.ST_SpentCoin);
        }

        public override DataCache<StorageKey, StorageItem> GetStorages()
        {
            return new DbCache<StorageKey, StorageItem>(db, null, null, Prefixes.ST_Storage);
        }

        public override DataCache<UInt256, TransactionState> GetTransactions()
        {
            return new DbCache<UInt256, TransactionState>(db, null, null, Prefixes.DATA_Transaction);
        }

        public override DataCache<UInt256, UnspentCoinState> GetUnspentCoins()
        {
            return new DbCache<UInt256, UnspentCoinState>(db, null, null, Prefixes.ST_Coin);
        }

        public override DataCache<ECPoint, ValidatorState> GetValidators()
        {
            return new DbCache<ECPoint, ValidatorState>(db, null, null, Prefixes.ST_Validator);
        }

        public override DataCache<UInt32Wrapper, HeaderHashList> GetHeaderHashList()
        {
            return new DbCache<UInt32Wrapper, HeaderHashList>(db, null, null, Prefixes.IX_HeaderHashList);
        }

        public override MetaDataCache<ValidatorsCountState> GetValidatorsCount()
        {
            return new DbMetaDataCache<ValidatorsCountState>(db, null, null, Prefixes.IX_ValidatorsCount);
        }

        public override MetaDataCache<HashIndexState> GetBlockHashIndex()
        {
            return new DbMetaDataCache<HashIndexState>(db, null, null, Prefixes.IX_CurrentBlock);
        }

        public override MetaDataCache<HashIndexState> GetHeaderHashIndex()
        {
            return new DbMetaDataCache<HashIndexState>(db, null, null, Prefixes.IX_CurrentHeader);
        }

        private MetaDataCache<NeoVersion> GetVersion()
        {
            return new DbMetaDataCache<NeoVersion>(db, null, null, Prefixes.SYS_Version);
        }

        private bool GetNeoVersion(out string version)
        {
            version = "";

            var neoVersion = GetVersion().Get();
            if (neoVersion != null)
            {
                version = neoVersion.SystemVersion;
            }
            if (version == null || version.Trim().Length == 0)
            {
                return false;
            }
            return true;
        }
    }
}
