using NEL.Common;
using NEL.Peer.Tcp;
using NEL.Pipeline;
using NEL.Simple.SDK;
using System.Threading.Tasks;
using System;

namespace Neo.IO.Data.LightDB
{
    public class DB
    {
        byte[] tableId = new byte[] { };

        public static DataCache<string, byte[]> dataCache = new DataCache<string, byte[]>();

        private IModulePipeline actor;

        public void Open(string address,string port,string path)
        {
            var logger = new Logger();
            var systemC = PipelineSystem.CreatePipelineSystemV1(logger);
            systemC.RegistModule("neo-cli-nel", new DBModule(address, port, path));
            systemC.OpenNetwork(new PeerOption());
            systemC.Start();
            actor = systemC.GetPipeline(null, "this/neo-cli-nel");
        }

        public async Task<WriteBatch> CreateWriteBatch()
        {
            var writeBatch = new WriteBatch(actor);
            await writeBatch.CreateWriteBatch();
            return writeBatch;
        }

        public async Task<Snapshot> CreatSnapshot()
        {
            var snapshot = new Snapshot(actor);
            await snapshot.CreatSnapshot();
            return snapshot;
        }


    }
}
