using LTLightDB = LightDB.LightDB;
using LTDBValue = LightDB.DBValue;
using LTSnapShot = LightDB.ISnapShot;
using Neo.Persistence.LightDB;
using Microsoft.AspNetCore.Http;
using Neo.IO.Json;
using Neo.Network.RPC;
using System.IO;
using Neo.IO.Data.LightDB;

namespace Neo.Plugins
{
    public class LogReader : Plugin, IRpcPlugin
    {
        private readonly LTLightDB db;

        public override string Name => "ApplicationLogs";

        public LogReader()
        {
            db = DB.Open(Path.GetFullPath(Settings.Default.Path), new Options { CreateIfMissing = true });
            System.ActorSystem.ActorOf(Logger.Props(System.Blockchain, db));
        }

        public JObject OnProcess(HttpContext context, string method, JArray _params)
        {
            if (method != "getapplicationlog") return null;
            UInt256 hash = UInt256.Parse(_params[0].AsString());
            LTSnapShot snapShot = db.UseSnapShot();
            LTDBValue lTDBValue =  snapShot.GetValue(new byte[] { Prefixes.DATA_ApplicationLog}, hash.ToArray());
            if (lTDBValue==null)
                throw new RpcException(-100, "Unknown transaction");
            return JObject.Parse((string)lTDBValue.typedvalue);
        }

        public override void Configure()
        {
            Settings.Load(GetConfiguration());
        }
    }
}
