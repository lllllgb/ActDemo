using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class CameraActionEvent
    {
        public class ModifySceneMaskColor : GameEventArgs
        {
            public static readonly int EventID = typeof(ModifySceneMaskColor).GetHashCode();

            public override int Id { get { return EventID; } }

            public Color Data { get; set; }
        }
    }
}
