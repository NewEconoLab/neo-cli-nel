using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.IO.Data.LightDB
{
    public static class Helper
    {
        public static void Delete(this WriteBatch batch, byte prefix, ISerializable key)
        {
            batch.Delete(SliceBuilder.Begin(prefix).Add(key));
        }

        public static IEnumerable<T> Find<T>(this DB db, byte prefix) where T : class, ISerializable, new()
        {
            return Find(db, SliceBuilder.Begin(prefix), (k, v) => v.ToArray().AsSerializable<T>());
        }

        public static IEnumerable<T> Find<T>(this DB db, Slice prefix, Func<Slice, Slice, T> resultSelector)
        {
            var it = db.CurSnapshot.CreateNewIterator(prefix.buffer);
            {
                while ( it.MoveNext())
                {
                    byte[] key = it.Current;
                    byte[] y = prefix.ToArray();
                    if (key.Length < y.Length) break;
                    if (!key.Take(y.Length).SequenceEqual(y)) break;

                    Slice value = db.CurSnapshot.GetValue(key);

                    yield return resultSelector(new Slice(key), value);
                }
            }
        }

        public static T Get<T>(this DB db, byte prefix, ISerializable key) where T : class, ISerializable, new()
        {
            return db.CurSnapshot.GetValue(SliceBuilder.Begin(prefix).Add(key)).ToArray().AsSerializable<T>();
        }
    }
}
