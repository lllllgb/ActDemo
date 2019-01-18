using System;

namespace AosHotfixFramework
{
    public class CommandHandlerAttribute : Attribute
    {
    }

    public class CommandHandleAttribute : Attribute
    {
        public ushort Opcode { get; set; }

        public CommandHandleAttribute(byte first, byte second)
        {
            Opcode = first;
            Opcode <<= 8;
            Opcode |= second;
        }
    }

    public class CommandModuleHandlerAttribute : Attribute
    {

    }

    public class CommandModuleHandleAttribute : Attribute
    {
        public ushort ModuleID { get; set; }

        public CommandModuleHandleAttribute(byte moduleID)
        {
            ModuleID = moduleID;
        }
    }
}