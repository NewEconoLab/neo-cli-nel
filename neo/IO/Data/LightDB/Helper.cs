using System;
using System.Collections.Generic;
using System.Linq;
using LTISnapshot = LightDB.ISnapShot;
using LTIKeyIterator = LightDB.IKeyIterator;

namespace Neo.IO.Data.LightDB
{
    public static class Helper
    {
        public static void Delete(this WriteBatch batch, byte prefix, ISerializable key)
        {
            batch.Delete(SliceBuilder.Begin(prefix).Add(key));
        }

        public static IEnumerable<T> Find<T>(this DB db, LTISnapshot snapshot, byte prefix) where T : class, ISerializable, new()
        {
            return Find(db, snapshot, SliceBuilder.Begin(prefix), (k, v) => v.ToArray().AsSerializable<T>());
        }

        public static IEnumerable<T> Find<T>(this DB db, LTISnapshot snapshot, Slice prefix, Func<Slice, Slice, T> resultSelector)
        {
            LTIKeyIterator it = snapshot.CreateKeyIterator(new byte[] { }, prefix.buffer);
            {
                while ( it.MoveNext())
                {
                    byte[] key = it.Current;
                    byte[] y = prefix.ToArray();
                    if (key.Length < y.Length) break;
                    if (!key.Take(y.Length).SequenceEqual(y)) break;

                    byte[] value = snapshot.GetValue(new byte[] { },key).value;

                    yield return resultSelector(new Slice(key), new Slice(value));
                }
            }
        }

        public static T Get<T>(this DB db, ReadOptions options, byte prefix, ISerializable key) where T : class, ISerializable, new()
        {
            return db.Get(options, SliceBuilder.Begin(prefix).Add(key)).ToArray().AsSerializable<T>();
        }
    }
}
