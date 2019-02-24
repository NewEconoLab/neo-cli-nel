using Neo.IO.Caching;
using Neo.Ledger;
using Neo.Cryptography.ECC;
using Neo.IO.Wrappers;
using Neo.IO.Data.LightDB;

namespace Neo.Persistence.LightDB
{
    class DbSnapshot : Snapshot
    {
        private readonly DB db;
        private readonly Neo.IO.Data.LightDB.WriteBatch batch;
        private readonly Neo.IO.Data.LightDB.Snapshot snapshot;

        public override DataCache<UInt256, BlockState> Blocks { get; }
        public override DataCache<UInt256, TransactionState> Transactions { get; }
        public override DataCache<UInt160, AccountState> Accounts { get; }
        public override DataCache<UInt256, UnspentCoinState> UnspentCoins { get; }
        public override DataCache<UInt256, SpentCoinState> SpentCoins { get; }
        public override DataCache<ECPoint, ValidatorState> Validators { get; }
        public override DataCache<UInt256, AssetState> Assets { get; }
        public override DataCache<UInt160, ContractState> Contracts { get; }
        public override DataCache<StorageKey, StorageItem> Storages { get; }
        public override DataCache<UInt32Wrapper, HeaderHashList> HeaderHashList { get; }
        public override MetaDataCache<ValidatorsCountState> ValidatorsCount { get; }
        public override MetaDataCache<HashIndexState> BlockHashIndex { get; }
        public override MetaDataCache<HashIndexState> HeaderHashIndex { get; }

        public DbSnapshot(DB db)
        {
            this.db = db;
            this.snapshot = db.CurSnapshot;
            this.batch = db.CreateNewWriteBatch();
            Blocks = new DbCache<UInt256, BlockState>(db, snapshot, batch, Prefixes.DATA_Block);
            Transactions = new DbCache<UInt256, TransactionState>(db, snapshot, batch, Prefixes.DATA_Transaction);
            Accounts = new DbCache<UInt160, AccountState>(db, snapshot, batch, Prefixes.ST_Account);
            UnspentCoins = new DbCache<UInt256, UnspentCoinState>(db, snapshot, batch, Prefixes.ST_Coin);
            SpentCoins = new DbCache<UInt256, SpentCoinState>(db, snapshot, batch, Prefixes.ST_SpentCoin);
            Validators = new DbCache<ECPoint, ValidatorState>(db, snapshot, batch, Prefixes.ST_Validator);
            Assets = new DbCache<UInt256, AssetState>(db, snapshot, batch, Prefixes.ST_Asset);
            Contracts = new DbCache<UInt160, ContractState>(db, snapshot, batch, Prefixes.ST_Contract);
            Storages = new DbCache<StorageKey, StorageItem>(db, snapshot, batch, Prefixes.ST_Storage);
            HeaderHashList = new DbCache<UInt32Wrapper, HeaderHashList>(db, snapshot, batch, Prefixes.IX_HeaderHashList);
            ValidatorsCount = new DbMetaDataCache<ValidatorsCountState>(db, snapshot, batch, Prefixes.IX_ValidatorsCount);
            BlockHashIndex = new DbMetaDataCache<HashIndexState>(db, snapshot, batch, Prefixes.IX_CurrentBlock);
            HeaderHashIndex = new DbMetaDataCache<HashIndexState>(db, snapshot, batch, Prefixes.IX_CurrentHeader);
        }

        public override void Commit()
        {
            base.Commit();
            //if (batch.items.Count > 0)
            //{
            //    db.Write(batch);
            //}
            batch.Write();
        }

        public override void Dispose()
        {
            this.snapshot.Dispose();
        }
    }
}
