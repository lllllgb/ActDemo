using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class UnitCtrlEvent
    {
        public class CurrHitedMonsterChange : GameEventArgs
        {
            public static readonly int EventID = typeof(CurrHitedMonsterChange).GetHashCode();

            public override int Id { get { return EventID; } }

            public Monster Data { get; set; }
        }
    }
}
