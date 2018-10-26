﻿using System;
using System.IO;

namespace Neo.VM
{
    public class ExecutionContext : IDisposable
    {
        public readonly byte[] Script;
        internal readonly BinaryReader OpReader;
        internal readonly int RVCount;
        private readonly ICrypto crypto;
        public ExecutionStackRecord EvaluationStack { get; } = new ExecutionStackRecord();
        public RandomAccessStack<StackItem> AltStack { get; } = new RandomAccessStack<StackItem>();
        public int InstructionPointer
        {
            get
            {
                return (int)OpReader.BaseStream.Position;
            }
            set
            {
                OpReader.BaseStream.Seek(value, SeekOrigin.Begin);
            }
        }

        public OpCode NextInstruction => (OpCode)Script[OpReader.BaseStream.Position];

        private byte[] _script_hash = null;
        public byte[] ScriptHash
        {
            get
            {
                if (_script_hash == null)
                    _script_hash = crypto.Hash160(Script);
                return _script_hash;
            }
        }

        internal ExecutionContext(ExecutionEngine engine, byte[] script, int rvcount)
        {
            this.Script = script;
            this.RVCount = rvcount;
            this.OpReader = new BinaryReader(new MemoryStream(script, false));
            this.crypto = engine.Crypto;
        }

        public void Dispose()
        {
            OpReader.Dispose();
        }
    }
}
