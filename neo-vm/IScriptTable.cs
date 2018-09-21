namespace Neo.VM
{
    public interface IScriptEX
    {
        byte[] script
        {
            get;
        }
        bool isNative
        {
            get;
        }
        string nativeTag
        {
            get;
        }
        bool RunNative(ExecutionEngine engine, ExecutionContext context);

    }
    public interface IScriptTable
    {
        IScriptEX GetScript(byte[] script_hash);
    }
}
