using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class PlayerCtrlEvent
    {
        public class UpdateSkillCD : GameEventArgs
        {
            public static readonly int EventID = typeof(UpdateSkillCD).GetHashCode();

            public override int Id { get { return EventID; } }

            public int SkillID;
            public float CD;
        }
    }
}