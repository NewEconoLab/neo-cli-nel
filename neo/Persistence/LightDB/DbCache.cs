using Neo.IO;
using Neo.IO.Caching;
using System;
using System.Collections.Generic;
using Neo.IO.Data.LightDB;
using System.Linq;

namespace Neo.Persistence.LightDB
{
    internal class DbCache<TKey, TValue> : DataCache<TKey, TValue>
        where TKey : IEquatable<TKey>, ISerializable, new()
        where TValue : class, ICloneable<TValue>, ISerializable, new()
    {
        private readonly DB db;
        private Neo.IO.Data.LightDB.Snapshot snapshot;
        private WriteBatch batch;
        private readonly byte prefix;

        public DbCache(DB db, Neo.IO.Data.LightDB.Snapshot _snapshot, WriteBatch batch, byte prefix)
        {
            this.db = db;
            this.snapshot = _snapshot ?? db.CreatNewSnapshot();
            this.batch =batch;
            this.prefix = prefix;
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            batch?.Put((new byte[] { prefix }).Concat(key.ToArray()).ToArray(), value.ToArray());
        }

        public override void DeleteInternal(TKey key)
        {
            batch?.Delete( (new byte[] { prefix }).Concat(key.ToArray()).ToArray());
        }

        protected override IEnumerable<KeyValuePair<TKey, TValue>> FindInternal(byte[] key_prefix)
        {
            return db.Find( SliceBuilder.Begin(prefix).Add(key_prefix), (k, v) => new KeyValuePair<TKey, TValue>(k.ToArray().AsSerializable<TKey>(1), v.ToArray().AsSerializable<TValue>()));
        }

        protected override TValue GetInternal(TKey key)
        {
            return db.Get<TValue>(prefix,key);
        }

        protected override TValue TryGetInternal(TKey key)
        {
            var val = snapshot.GetValue((new byte[] { prefix }).Concat(key.ToArray()).ToArray()).ToArray(); 
            if (val == null||val.Length == 0)
            {
                return null;
            }
            return val.AsSerializable<TValue>();
        }

        protected override void UpdateInternal(TKey key, TValue value)
        {
            batch?.Put((new byte[] { prefix }).Concat(key.ToArray()).ToArray(),  value.ToArray());
        }
    }
}
