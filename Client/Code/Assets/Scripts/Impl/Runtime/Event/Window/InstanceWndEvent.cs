using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class InstanceWndEvent
    {
        public class StartInstanceEvent : GameEventArgs
        {
            public static readonly int EventID = typeof(StartInstanceEvent).GetHashCode();

            public override int Id { get { return EventID; } }

        }
    }
}
