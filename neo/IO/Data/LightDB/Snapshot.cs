
using NEL.Pipeline;
using NEL.Simple.SDK;
using System.Threading.Tasks;

namespace Neo.IO.Data.LightDB
{
    public class Snapshot
    {
        private byte[] tableId = new byte[] { };

        private IModulePipeline actor;

        private string id;

        public Snapshot(IModulePipeline _actor)
        {
            actor = _actor;
        }

        public async Task CreatSnapshot()
        {
            //创建snapshot
            NetMessage netMessage = Protocol_UseSnapShot.CreateSendMsg("");
            actor.Tell(netMessage.ToBytes());
            await DB.dataCache.Get(netMessage.Cmd+netMessage.ID);
            id = Protocol_UseSnapShot.PraseRecvMsg(netMessage).ToString();
        }

        public async Task<Iterator> CreateIterator()
        {
            var Iterator = new Iterator(actor);
            await Iterator.CreateIterator(id);
            return Iterator;
        }

        /*
        public async Task<bool> TryGet(ReadOptions options, Slice key, out Slice value)
        {
            try
            {
                //获取value
                netMessage = Protocol_GetValue.CreateSendMsg(key.buffer, "getvalue");
                actor.Tell(netMessage.ToBytes());
                var bytes = await dataCache.Get(netMessage.ID);
                value = new Slice(bytes);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("error:"+e.Message);
                return false;
            }
        }
        */

        public async Task<Slice> Get(Slice key)
        {
            //获取value
            NetMessage netMessage = Protocol_GetValue.CreateSendMsg(key.buffer, id);
            actor.Tell(netMessage.ToBytes());
            var bytes = await DB.dataCache.Get(netMessage.Cmd+netMessage.ID);
            return new Slice(bytes);
        }

        public async Task Write(WriteBatch write_batch)
        {
            NetMessage netMessage = Protocol_Write.CreateSendMsg(id);
            actor.Tell(netMessage.ToBytes());
            await DB.dataCache.Get(netMessage.Cmd+netMessage.ID);
        }
    }
}
