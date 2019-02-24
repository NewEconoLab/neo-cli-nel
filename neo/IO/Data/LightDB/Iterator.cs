using NEL.Pipeline;
using NEL.Simple.SDK;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Neo.IO.Data.LightDB
{
    public class Iterator
    {
        private byte[] tableId = new byte[] { };

        private IModulePipeline actor;

        public UInt64 Itid { get; private set; }

        public Iterator(IModulePipeline _actor, string id,UInt64 snapid, byte[] beginKey = null, byte[] endKey = null)
        {
            actor = _actor;
            NetMessage netMessage = Protocol_CreateIterator.CreateSendMsg(snapid, beginKey??new byte[] { }, endKey??new byte[] { },id);
            actor.Tell(netMessage.ToBytes());
            var p = DB.dataCache.Get(netMessage.Cmd + netMessage.ID).Result;
        }

        public byte[] Current
        {
            get
            {
                NetMessage netMessage = Protocol_IteratorCurrent.CreateSendMsg(Itid, Itid.ToString());
                actor.Tell(netMessage.ToBytes());
                return DB.dataCache.Get(netMessage.Cmd + netMessage.ID).Result.value;
            }
        }

        public bool MoveNext()
        {
            NetMessage netMessage = Protocol_IteratorNext.CreateSendMsg(Itid, Itid.ToString());
            actor.Tell(netMessage.ToBytes());
            var p =DB.dataCache.Get(netMessage.Cmd+netMessage.ID).Result;
            return p.result;
        }

        public bool SeekToFirst()
        {
            NetMessage netMessage = Protocol_IteratorSeekToFirst.CreateSendMsg(Itid, Itid.ToString());
            actor.Tell(netMessage.ToBytes());
            var p = DB.dataCache.Get(netMessage.Cmd + netMessage.ID).Result;
            return p.result;
        }

        public bool Reset()
        {
            NetMessage netMessage = Protocol_IteratorReset.CreateSendMsg(Itid, Itid.ToString());
            actor.Tell(netMessage.ToBytes());
            var p = DB.dataCache.Get(netMessage.Cmd + netMessage.ID).Result;
            return p.result;
        }
    }
}
