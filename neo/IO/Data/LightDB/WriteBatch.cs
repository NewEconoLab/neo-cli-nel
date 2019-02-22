using NEL.Pipeline;
using NEL.Simple.SDK;
using System.Text;
using System.Threading.Tasks;

namespace Neo.IO.Data.LightDB
{
    public class WriteBatch
    {
        private byte[] tableId = new byte[] { };

        private IModulePipeline actor;

        private string id;

        public WriteBatch(IModulePipeline _actor)
        {
            actor = _actor;
        }

        public async Task CreateWriteBatch()
        {
            NetMessage netMessage = Protocol_CreateWriteBatch.CreateSendMsg("");
            actor.Tell(netMessage.ToBytes());
            var bytes = await DB.dataCache.Get(netMessage.Cmd + netMessage.ID);
            id = Protocol_CreateWriteBatch.PraseRecvMsg(netMessage).ToString();
        }


        public async Task Put(Slice key, Slice value)
        {
            NetMessage netMessage = Protocol_Put.CreateSendMsg(id);
            actor.Tell(netMessage.ToBytes());
            var bytes = await DB.dataCache.Get(netMessage.Cmd + netMessage.ID);
        }

        public async Task Delete(Slice key)
        {
            NetMessage netMessage = Protocol_Delete.CreateSendMsg(id);
            actor.Tell(netMessage.ToBytes());
            var bytes = await DB.dataCache.Get(netMessage.Cmd+netMessage.ID);
        }
    }
}
