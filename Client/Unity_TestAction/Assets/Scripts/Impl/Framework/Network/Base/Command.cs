using System;

namespace AosHotfixFramework
{
    public class CommandBaseBase
    {
        public byte First { get; set; }
        public byte Second { get; set; }
        public ushort Opcode { get; set; }
    }

    public class CommandBase<T> : CommandBaseBase
    {
        public readonly T Data = Activator.CreateInstance<T>();
    }
}