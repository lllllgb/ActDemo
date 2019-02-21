using System;

namespace AosHotfixFramework
{
    public class RspCommandAttribute : Attribute
    {
        public ushort Opcode { get; set; }

        public RspCommandAttribute(byte first, byte second)
        {
            Opcode = first;
            Opcode <<= 8;
            Opcode |= second;
        }
    }
}
