
using NEL.Pipeline;
using NEL.Simple.SDK;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Neo.IO.Data.LightDB
{
    public class Snapshot
    {
        private byte[] tableId = new byte[] { };

        private IModulePipeline actor;

        public UInt64 snapid { get; private set; }

        public bool state { get; private set; }

        public Snapshot(IModulePipeline _actor)
        {
            actor = _actor;
            state = true;
            //创建snapshot
            NetMessage netMessage = Protocol_UseSnapShot.CreateSendMsg("");
            actor.Tell(netMessage.ToBytes());
            var p = DB.dataCache.Get(netMessage.Cmd + netMessage.ID).Result;
            snapid = p.snapid;
        }


        public Iterator CreateNewIterator(byte[] beginkey =null,byte[] endKey = null)
        {
            var Iterator = new Iterator(actor, "", snapid,beginkey, endKey);
            return Iterator;
        }

        public Slice GetValue(Slice key)
        {
            //获取value
            NetMessage netMessage = Protocol_GetValue.CreateSendMsg(key.buffer, key.buffer.ToHexString()+ snapid, snapid);
            actor.Tell(netMessage.ToBytes());
            var p =  DB.dataCache.Get(netMessage.Cmd+netMessage.ID).Result;
            return new Slice(p.value);
        }

        public bool Dispose()
        {
            state = false;
            Snapshot snapshot;
            DB.snapshotpool[snapid].TryDequeue(out snapshot);
            if (DB.snapshotpool[snapid].Count != 0)
                return true;
            return true;
            //NetMessage netMessage = Protocol_DisposeSnapShot.CreateSendMsg(id);
            //actor.Tell(netMessage.ToBytes());
            //var p = DB.dataCache.Get(netMessage.Cmd + netMessage.ID).Result;
            //return p.result;
        }
    }
}
