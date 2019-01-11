using Neo.IO;
using Neo.IO.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neo.Ledger
{
    class NeoVersion : StateBase, ICloneable<NeoVersion>
    {
        public string SystemVersion;

        public override int Size => base.Size + SystemVersion.Length + sizeof(uint);

        NeoVersion ICloneable<NeoVersion>.Clone()
        {
            return new NeoVersion
            {
                SystemVersion = SystemVersion
            };
        }

        void ICloneable<NeoVersion>.FromReplica(NeoVersion replica)
        {
            SystemVersion = replica.SystemVersion;
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(SystemVersion);
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["SystemVersion"] = SystemVersion;
            return json;
        }
    }
}
