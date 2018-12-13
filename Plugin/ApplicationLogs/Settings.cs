using Microsoft.Extensions.Configuration;
using Neo.Network.P2P;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Neo.Plugins
{
    internal class Settings
    {
        public string Path { get; }
        public string Conn { get; }
        public string Db { get; }
        public string Coll { get; }
        public string[] MongoDbIndex { get; }

        public static Settings Default { get; private set; }

        private Settings(IConfigurationSection section)
        {
            this.Path = string.Format(section.GetSection("Path").Value, Message.Magic.ToString("X8"));
            this.Conn = section.GetSection("Conn").Value;
            this.Db = section.GetSection("Db").Value;
            this.Coll = section.GetSection("Coll").Value;
            this.MongoDbIndex = section.GetSection("MongoDbIndexs").GetChildren().Select(p => p.Value).ToArray();
            if (!string.IsNullOrEmpty(this.Conn) && !string.IsNullOrEmpty(this.Conn) && !string.IsNullOrEmpty(this.Conn))
            {
                //创建索引
                for (var i = 0; i < this.MongoDbIndex.Length; i++)
                {
                    SetMongoDbIndex(this.MongoDbIndex[i]);
                }
            }
        }

        public static void Load(IConfigurationSection section)
        {
            Default = new Settings(section);
        }

        public void SetMongoDbIndex(string mongoDbIndex)
        {
            JObject joIndex = JObject.Parse(mongoDbIndex);
            JArray indexs = (JArray)joIndex["indexs"];
            for(var i = 0;i<indexs.Count;i++)
            {
                string indexName = (string)indexs[i]["indexName"];
                string indexDefinition = indexs[i]["indexDefinition"].ToString();
                bool isUnique = false;
                if (indexs[i]["isUnique"]!=null)
                    isUnique = (bool)indexs[i]["isUnique"];
                MongoHelper.SetIndex(this.Conn, this.Db, this.Coll, indexDefinition, indexName, isUnique);
            }
        }
    }
}
