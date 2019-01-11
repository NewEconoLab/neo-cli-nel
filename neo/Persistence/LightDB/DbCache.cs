using LTWriteTask = LightDB.WriteTask;
using LTDBValue = LightDB.DBValue;
using LTISnapshot = LightDB.ISnapShot;
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
        private LTISnapshot snapshot;
        private readonly ReadOptions options;
        private readonly LTWriteTask batch;
        private readonly byte prefix;

        public DbCache(DB db, LTISnapshot _snapshot, LTWriteTask batch, byte prefix)
        {
            this.db = db;
            snapshot = _snapshot ?? db.UseSnapShot();
            this.options = options ?? ReadOptions.Default;
            this.batch = batch;
            this.prefix = prefix;
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            batch?.Put(new byte[] { },(new byte[] { prefix }).Concat(key.ToArray()).ToArray(), LTDBValue.FromValue(LTDBValue.Type.Bytes, value.ToArray()));
        }

        public override void DeleteInternal(TKey key)
        {
            batch?.Delete(new byte[] { }, (new byte[] { prefix }).Concat(key.ToArray()).ToArray());
        }

        protected override IEnumerable<KeyValuePair<TKey, TValue>> FindInternal(byte[] key_prefix)
        {
            return db.Find(snapshot, SliceBuilder.Begin(prefix).Add(key_prefix), (k, v) => new KeyValuePair<TKey, TValue>(k.ToArray().AsSerializable<TKey>(1), v.ToArray().AsSerializable<TValue>()));
        }

        protected override TValue GetInternal(TKey key)
        {
            return db.Get<TValue>(options, prefix,key);
        }

        protected override TValue TryGetInternal(TKey key)
        {
            var val= snapshot.GetValue(new byte[] { }, (new byte[] { prefix }).Concat(key.ToArray()).ToArray())?.value;
            if (val == null||val.Length == 0)
            {
                return null;
            }
            return val.AsSerializable<TValue>();
        }

        protected override void UpdateInternal(TKey key, TValue value)
        {
            batch?.Put(new byte[] { }, (new byte[] { prefix }).Concat(key.ToArray()).ToArray(), LTDBValue.FromValue(LTDBValue.Type.Bytes, value.ToArray()));
        }
    }
}
