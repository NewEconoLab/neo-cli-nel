using Akka.Actor;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.VM;
using System;
using System.Linq;
using LTLightDB = LightDB.LightDB;
using LTWriteTask = LightDB.WriteTask;
using LTDBValue = LightDB.DBValue;
using Neo.Persistence.LightDB;

namespace Neo.Plugins
{
    internal class Logger : UntypedActor
    {
        private readonly LTLightDB db;

        public Logger(IActorRef blockchain, LTLightDB db)
        {
            this.db = db;
            blockchain.Tell(new Blockchain.Register());
        }

        protected override void OnReceive(object message)
        {
            if (message is Blockchain.ApplicationExecuted e)
            {
                JObject json = new JObject();
                json["txid"] = e.Transaction.Hash.ToString();
                json["blockindex"] = e.BlockIndex;
                json["executions"] = e.ExecutionResults.Select(p =>
                {
                    JObject execution = new JObject();
                    execution["trigger"] = p.Trigger;
                    execution["contract"] = p.ScriptHash.ToString();
                    execution["vmstate"] = p.VMState;
                    execution["gas_consumed"] = p.GasConsumed.ToString();
                    try
                    {
                        execution["stack"] = p.Stack.Select(q => q.ToParameter().ToJson()).ToArray();
                    }
                    catch (InvalidOperationException)
                    {
                        execution["stack"] = "error: recursive reference";
                    }
                    execution["notifications"] = p.Notifications.Select(q =>
                    {
                        JObject notification = new JObject();
                        notification["contract"] = q.ScriptHash.ToString();
                        try
                        {
                            notification["state"] = q.State.ToParameter().ToJson();
                        }
                        catch (InvalidOperationException)
                        {
                            notification["state"] = "error: recursive reference";
                        }
                        return notification;
                    }).ToArray();
                    return execution;
                }).ToArray();
                if (!string.IsNullOrEmpty(Settings.Default.Conn) && !string.IsNullOrEmpty(Settings.Default.Db) && !string.IsNullOrEmpty(Settings.Default.Coll))
                {
                    //增加applicationLog输入到数据库
                    MongoHelper.InsetOne(Settings.Default.Conn, Settings.Default.Db, Settings.Default.Coll, MongoDB.Bson.BsonDocument.Parse(json.ToString()));

                    if (e.IsLastTransaction)
                    {
                        var blockindex = (int)e.BlockIndex;
                        json = new JObject();
                        json["counter"] = "notify";
                        string whereFliter = json.ToString();
                        json["lastBlockindex"] = blockindex;
                        string replaceFliter = json.ToString();
                        MongoHelper.ReplaceData(Settings.Default.Conn, Settings.Default.Db,"system_counter", whereFliter, MongoDB.Bson.BsonDocument.Parse(replaceFliter));
                    }
                }
                else
                {
                    LTWriteTask writetask = db.CreateWriteTask();
                    writetask.Put(new byte[] { Prefixes.DATA_ApplicationLog }, e.Transaction.Hash.ToArray(),LTDBValue.FromValue(LTDBValue.Type.String, json.ToString()));
                    db.Write(writetask);
                }
            }
        }

        public static Props Props(IActorRef blockchain, LTLightDB db)
        {
            return Akka.Actor.Props.Create(() => new Logger(blockchain, db));
        }
    }
}
