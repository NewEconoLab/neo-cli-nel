using NEL.Pipeline;
using NEL.Simple.SDK;
using System;
using System.Text;

namespace Neo.IO.Data.LightDB
{
    public class WriteBatch
    {
        private byte[] tableId = new byte[] { };

        private IModulePipeline actor;

        public UInt64 Wbid { get; private set; }

        public int count = 0;

        public WriteBatch(IModulePipeline _actor)
        {
            actor = _actor;
            NetMessage netMessage = Protocol_CreateWriteBatch.CreateSendMsg("");
            actor.Tell(netMessage.ToBytes());
            var p = DB.dataCache.Get(netMessage.Cmd + netMessage.ID).Result;
            Wbid = p.wbid;
        }

        public bool Put(Slice key, Slice value)
        {
            Console.WriteLine(key.buffer[0]);
            NetMessage netMessage = Protocol_Put.CreateSendMsg(Wbid, key.buffer, value.buffer, key.buffer.ToHexString()+ Wbid);
            actor.Tell(netMessage.ToBytes());
            var p = DB.dataCache.Get(netMessage.Cmd + netMessage.ID).Result;
            return p.result;
        }

        public bool Delete(Slice key)
        {
            NetMessage netMessage = Protocol_Delete.CreateSendMsg(Wbid, key.buffer, key.buffer.ToHexString()+ Wbid);
            actor.Tell(netMessage.ToBytes());
            var p = DB.dataCache.Get(netMessage.Cmd+netMessage.ID).Result;
            return p.result;
        }

        public bool Write()
        {
            NetMessage netMessage = Protocol_Write.CreateSendMsg(Wbid, Wbid.ToString());
            actor.Tell(netMessage.ToBytes());
            var p = DB.dataCache.Get(netMessage.Cmd+netMessage.ID).Result;
            return p.result;
        }
    }
}
