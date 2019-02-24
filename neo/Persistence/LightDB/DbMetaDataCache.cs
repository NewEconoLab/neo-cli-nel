using Neo.IO;
using Neo.IO.Caching;
using Neo.IO.Data.LightDB;
using System;

namespace Neo.Persistence.LightDB
{
    class DbMetaDataCache<T> : MetaDataCache<T>
        where T : class, ICloneable<T>, ISerializable, new()
    {
        private readonly DB db;
        private readonly Neo.IO.Data.LightDB.Snapshot snapshot;
        private readonly WriteBatch batch;
        private readonly byte prefix;

        public DbMetaDataCache(DB db, Neo.IO.Data.LightDB.Snapshot snapshot, WriteBatch batch, byte prefix, Func<T> factory = null)
    : base(factory)
        {
            this.db = db;
            this.snapshot = snapshot ?? db.CreatNewSnapshot();
            this.batch = batch;
            this.prefix = prefix;
        }

        protected override void AddInternal(T item)
        {
            batch?.Put(new byte[] { prefix },item.ToArray());
        }

        protected override T TryGetInternal()
        {
            var val = db.CurSnapshot.GetValue(new byte[] { prefix }).buffer;
            if (val ==null|| val.Length == 0)
            {
                return null;
            }
            return val.AsSerializable<T>();
        }

        protected override void UpdateInternal(T item)
        {
            batch?.Put(new byte[] { prefix },item.ToArray());
        }
    }
}
