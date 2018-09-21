using Neo.Core;
using Neo.IO.Caching;
using Neo.VM;

namespace Neo.SmartContract
{
    internal class CachedScriptTable : IScriptTable
    {
        private DataCache<UInt160, ContractState> contracts;

        public CachedScriptTable(DataCache<UInt160, ContractState> contracts)
        {
            this.contracts = contracts;
        }

        //byte[] IScriptTable.GetScript(byte[] script_hash)
        //{
        //    return contracts[new UInt160(script_hash)].Script;
        //}

        //改造为增强的合约
        IScriptEX IScriptTable.GetScript(byte[] script_hash)
        {
            var scriptstate = contracts[new UInt160(script_hash)];
            return new Blockchain.ScriptEX(scriptstate);
        }
        public ContractState GetContractState(byte[] script_hash)
        {
            return contracts[new UInt160(script_hash)];
        }
    }
}
