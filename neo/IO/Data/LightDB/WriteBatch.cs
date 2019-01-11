using System;
using System.Collections.Generic;
using System.Text;
using LightDB;
using LTWriteTask = LightDB.WriteTask;
namespace Neo.IO.Data.LightDB
{
    public class WriteBatch : LTWriteTask
    {
        private byte[] tableId = new byte[] { };

        public void Put(Slice key, Slice value)
        {
            base.Put(tableId, key.buffer, DBValue.FromValue(DBValue.Type.Bytes,value.buffer));
        }

        public void Delete(Slice key)
        {
            base.Delete(tableId, key.buffer);
        }
    }
}
