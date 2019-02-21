using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{
    public abstract class ActEventArgs
    {
        /// <summary>
        /// 获取类型编号。
        /// </summary>
        public abstract int Id
        {
            get;
        }
    }

    public class ActEventMgr
    {
        protected static readonly ActEventMgr msInstance = new ActEventMgr();
        public static ActEventMgr Instance { get { return msInstance; } }

        public delegate void EventProcessFunc(ActEventArgs kArgs);
        Dictionary<int, EventProcessFunc> mkEvent2ProcessMap;

        protected ActEventMgr()
        {
            mkEvent2ProcessMap = new Dictionary<int, EventProcessFunc>();
        }

        public void SubscribeEvent(int eventID, ref EventProcessFunc kProcessFunc)
        {
            EventProcessFunc kFuncList;

            if (mkEvent2ProcessMap.TryGetValue(eventID, out kFuncList))
            {
                if (kFuncList != null && ContainProcess(ref kFuncList, ref kProcessFunc))
                {
                    Debug.LogError(string.Format("重复添加的处理函数[{0}+{1}]", kProcessFunc.Target, kProcessFunc.Method));
                }
                else
                {
                    mkEvent2ProcessMap[eventID] += kProcessFunc;
                }
            }
            else
            {
                mkEvent2ProcessMap.Add(eventID, kProcessFunc);
            }
        }

        public void UnSubscribeEvent(int uiEvent, ref EventProcessFunc kProcessFunc)
        {
            if (mkEvent2ProcessMap.ContainsKey(uiEvent))
            {
                mkEvent2ProcessMap[uiEvent] -= kProcessFunc;
            }
            else
            {
                Debug.LogError("未注册过的事件(" + uiEvent + ")");
            }
        }

        public void FireEvent(int uiEvent, ActEventArgs kArgs)
        {
            EventProcessFunc kFuncList;
            if (mkEvent2ProcessMap.TryGetValue(uiEvent, out kFuncList))
            {
                kFuncList?.Invoke(kArgs);
            }
        }

        public static bool ContainProcess(ref EventProcessFunc kFuncList, ref EventProcessFunc kProcessFunc)
        {
            foreach (EventProcessFunc kFunc in kFuncList.GetInvocationList())
            {
                if (kFunc == kProcessFunc)
                    return true;
            }

            return false;
        }
    }
}
