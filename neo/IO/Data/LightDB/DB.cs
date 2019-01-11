using LTLightDB = LightDB.LightDB;
using LTDBCreateOption = LightDB.DBCreateOption;
using LTDBValue = LightDB.DBValue;
using LTISnapshot = LightDB.ISnapShot;
using LTSnapshot = LightDB.SnapShot;

namespace Neo.IO.Data.LightDB
{
    public class DB:LTLightDB
    {
        byte[] tableId = new byte[] { };

        public static DB Open(string name)
        {
            return Open(name,Options.Default);
        }

        public static DB Open(string name, Options options)
        {
            DB db = new DB();
            db.Open(name, new LTDBCreateOption() { MagicStr = "" });
            return db;
        }

        public bool TryGet(ReadOptions options, Slice key, out Slice value)
        {
            var snapshot = UseSnapShot();
            if (snapshot.TryGetValue(tableId, key.buffer, out LTDBValue dBValue))
            {
                value = new Slice(dBValue);
                return true;
            }
            else
            {
                value = default(Slice);
                return false;
            }
        }

        public Slice Get(ReadOptions options, Slice key)
        {
            var snapshot = UseSnapShot();
            var dbvalue = snapshot.GetValue(tableId,key.buffer);
            try
            {
                if (dbvalue == null||dbvalue.value.Length == 0)
                    throw new LightDBException("not found");
                return new Slice(dbvalue);
            }
            finally
            {

            }
        }


        public void Write(WriteOptions options, WriteBatch write_batch)
        {
            base.Write(write_batch);
        }
    }
}
