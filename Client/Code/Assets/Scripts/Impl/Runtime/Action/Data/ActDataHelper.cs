using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Data1.Helper
{
    public static class ActDataHelper
    {
        public static Data1.Action GetAction(this ActionGroup actionGroup, int idx)
        {
            return idx < actionGroup.ActionList.Count ? actionGroup.ActionList[idx] : null;
        }

        public static int GetActionIdx(this ActionGroup actionGroup, string actionID)
        {
            actionID = NormalizeActionID(actionID);

            int idx = -1;

            for (int i = 0, max = actionGroup.ActionList.Count; i < max; i++)
            {
                Data1.Action action = actionGroup.ActionList[i];
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
    }
}
