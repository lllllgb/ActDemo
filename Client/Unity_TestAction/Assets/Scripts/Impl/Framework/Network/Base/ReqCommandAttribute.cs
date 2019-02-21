using System;

namespace AosHotfixFramework
{
    public class ReqCommandAttribute : Attribute
    {
        public ushort Opcode { get; set; }

        public ReqCommandAttribute(byte first, byte second)
        {
            Opcode = first;
            Opcode <<= 8;
            Opcode |= second;
        }
    }
}
