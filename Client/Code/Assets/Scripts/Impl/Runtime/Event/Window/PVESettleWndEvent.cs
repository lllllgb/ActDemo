using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class PVESettleWndEvent
    {
        public class BackMainEvent : GameEventArgs
        {
            public static readonly int EventID = typeof(BackMainEvent).GetHashCode();

            public override int Id { get { return EventID; } }

        }
    }
}
