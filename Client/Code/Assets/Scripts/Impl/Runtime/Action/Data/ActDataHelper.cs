using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ActData.Helper
{
    public static class ActDataHelper
    {
        public static ActData.Action GetAction(this ActionGroup actionGroup, int idx)
        {
            return idx < actionGroup.ActionList.Count ? actionGroup.ActionList[idx] : null;
        }

        public static int GetActionIdx(this ActionGroup actionGroup, string actionID)
        {
            //actionID = NormalizeActionID(actionID);

            int idx = -1;

            for (int i = 0, max = actionGroup.ActionList.Count; i < max; i++)
            {
                ActData.Action action = actionGroup.ActionList[i];
                idx++;
                if (action.Id == actionID)
                    return idx;
            }
            return idx;
        }

        public static string NormalizeActionID(string actionID)
        {
            for (int i = 1; i < actionID.Length; i++)
            {
                if (!char.IsDigit(actionID[i]))
                    return actionID.Substring(0, i);
            }
            return actionID;
        }

        public static int GetActionInterruptIdx(this Action action, ACT.EOperation operation)
        {
            int tmpInterruptIdx = -1;

            for (int i = 0, max = action.ActionInterrupts.Count; i < max; ++i)
            {
                if (action.ActionInterrupts[i].InputKey1 == (int)operation)
                {
                    tmpInterruptIdx = i;
                    break;
                }
            }

            return tmpInterruptIdx;
        }
    }
}
