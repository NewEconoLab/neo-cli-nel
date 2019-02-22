using NEL.Pipeline;
using NEL.Simple.SDK;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Neo.IO.Data.LightDB
{
    public sealed class DBModule : NEL.Pipeline.Module
    {
        private IModulePipeline actor;

        private string dBServerAddress;
        private string dBServerPort;
        private string dBServerPath;

        public DBModule(string _dBServerAddress, string _dBServerPort, string _dBServerPath)
        {
            dBServerAddress = _dBServerAddress;
            dBServerPort = _dBServerPort;
            dBServerPath = _dBServerPath;
        }

        public override void OnStart()
        {
            //链接数据库服务器
            actor = this.GetPipeline(string.Format("{0}:{1}/{2}", dBServerAddress, dBServerPort, dBServerPath));
        }

        public override void OnTell(IModulePipeline from, byte[] data)
        {
            if (from == null)
            {
                actor.Tell(data);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    NetMessage netMsg = NetMessage.Unpack(ms);
                    DB.dataCache.Add(netMsg.ID,netMsg.Params[0].value);
                }
            }
        }

        public override void OnTellLocalObj(IModulePipeline from, object obj)
        {

        }
    }
}
