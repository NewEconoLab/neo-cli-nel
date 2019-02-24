using NEL.Common;
using NEL.Peer.Tcp;
using NEL.Pipeline;
using NEL.Simple.SDK;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

namespace Neo.IO.Data.LightDB
{
    public class DB
    {
        byte[] tableId = new byte[] { };

        public static DataCache<string, Param> dataCache = new DataCache<string, Param>();

        public static ConcurrentDictionary<UInt64, ConcurrentQueue<Snapshot>> snapshotpool = new ConcurrentDictionary<UInt64, ConcurrentQueue<Snapshot>>();

        private IModulePipeline actor;

        private Snapshot _curSnapshot;
        public Snapshot CurSnapshot
        {
            get
            {
                if (_curSnapshot == null || _curSnapshot.state == false)
                    return CreatNewSnapshot();
                return _curSnapshot;
            }
        }

        public void Open(string address,string port,string path)
        {
            var logger = new Logger();
            var systemC = PipelineSystem.CreatePipelineSystemV1(logger);
            systemC.RegistModule("neo-cli-nel", new DBModule(address, port, path));
            systemC.OpenNetwork(new PeerOption());
            systemC.Start();
            actor = systemC.GetPipeline(null, "this/neo-cli-nel");
        }

        public Snapshot CreatNewSnapshot()
        {
            var snapshot = new Snapshot(actor);
            _curSnapshot = snapshot;
            if(!snapshotpool.ContainsKey(snapshot.snapid))
                snapshotpool[snapshot.snapid] = new ConcurrentQueue<Snapshot>();
            snapshotpool[snapshot.snapid].Enqueue(snapshot);
            return snapshot;
        }

        public WriteBatch CreateNewWriteBatch()
        {
            var writeBatch = new WriteBatch(actor);
            return writeBatch;
        }
    }
}
