using System;
using LTISnapshot = LightDB.ISnapShot;


namespace Neo.IO.Data.LightDB
{
    public class ReadOptions
    {
        public static readonly ReadOptions Default = new ReadOptions();
        internal readonly IntPtr handle = RocksDbSharp.Native.Instance.rocksdb_readoptions_create();

        public bool VerifyChecksums
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_readoptions_set_verify_checksums(handle, value);
            }
        }

        public bool FillCache
        {
            set
            {
                RocksDbSharp.Native.Instance.rocksdb_readoptions_set_fill_cache(handle, value);
            }
        }

        public LTISnapshot Snapshot
        {
            set
            {
                Snapshot = value;
            }

            get
            {
                return Snapshot;
            }
        }

        ~ReadOptions()
        {
            RocksDbSharp.Native.Instance.rocksdb_readoptions_destroy(handle);
        }
    }
}
