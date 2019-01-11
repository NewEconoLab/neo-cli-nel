using System;

namespace Neo.IO.Data.LightDB
{
    public class WriteOptions
    {
        public static readonly WriteOptions Default = new WriteOptions();
        internal readonly IntPtr handle = RocksDbSharp.Native.Instance.rocksdb_writeoptions_create();
    }
}
