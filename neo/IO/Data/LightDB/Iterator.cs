
using NEL.Pipeline;
using NEL.Simple.SDK;
using System.Threading.Tasks;

namespace Neo.IO.Data.LightDB
{
    public class Iterator
    {
        private byte[] tableId = new byte[] { };

        private IModulePipeline actor;

        private string id;

        public Iterator(IModulePipeline _actor)
        {
            actor = _actor;
        }

        public async Task CreateIterator(string snapshotid)
        {
            NetMessage netMessage = Protocol_CreateIterator.CreateSendMsg(snapshotid);
            actor.Tell(netMessage.ToBytes());
            await DB.dataCache.Get(netMessage.Cmd + netMessage.ID);
            id = Protocol_UseSnapShot.PraseRecvMsg(netMessage).ToString();
        }

        public async Task<byte[]> Current()
        {
            NetMessage netMessage = Protocol_IteratorCurrent.CreateSendMsg(id);
            actor.Tell(netMessage.ToBytes());
            return await DB.dataCache.Get(netMessage.Cmd + netMessage.ID);
        }

        public async Task MoveToNext()
        {
            NetMessage netMessage = Protocol_IteratorNext.CreateSendMsg(id);
            actor.Tell(netMessage.ToBytes());
            await DB.dataCache.Get(netMessage.Cmd+netMessage.ID);
        }

        public async Task Reset()
        {
            NetMessage netMessage = Protocol_IteratorReset.CreateSendMsg(id);
            actor.Tell(netMessage.ToBytes());
            await DB.dataCache.Get(netMessage.Cmd+ netMessage.ID);
        }
    }
}
