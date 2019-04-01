using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class UnitEvent
    {
        public class HpModify : GameEventArgs
        {
            public static readonly int EventID = typeof(HpModify).GetHashCode();

            public override int Id { get { return EventID; } }

            public Unit Data { get; set; }
        }

        public class MpModify : GameEventArgs
        {
            public static readonly int EventID = typeof(MpModify).GetHashCode();

            public override int Id { get { return EventID; } }

            public Unit Data { get; set; }
        }

        public class DpModify : GameEventArgs
        {
            public static readonly int EventID = typeof(DpModify).GetHashCode();

            public override int Id { get { return EventID; } }

            public Unit Data { get; set; }
        }

        public class Dead : GameEventArgs
        {
            public static readonly int EventID = typeof(Dead).GetHashCode();

            public override int Id { get { return EventID; } }

            public Unit Data { get; set; }
        }
    }
}
