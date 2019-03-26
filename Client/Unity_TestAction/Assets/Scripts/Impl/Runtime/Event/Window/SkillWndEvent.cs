using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class SkillWndEvent
    {
        public class SkillSetChange : GameEventArgs
        {
            public static readonly int EventID = typeof(SkillSetChange).GetHashCode();

            public override int Id { get { return EventID; } }
        }
    }
}
