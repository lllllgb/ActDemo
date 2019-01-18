using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ACT
{
    public static class ActionHelper
    {
        public static InputBox GInputBox { get; set; }

        public static List<IActUnit> UnitList = new List<IActUnit>();

        public static void LoopAllActUnits(Action<IActUnit> loopHandle)
        {
            for (int i = 0, max = UnitList.Count; i < max; ++i)
            {
                loopHandle?.Invoke(UnitList[i]);
            }
        }

        public static IActUnit LocalPlayer { get; set; }
    }
}