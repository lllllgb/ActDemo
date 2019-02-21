using UnityEngine;
using System.IO;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Google.Protobuf;
using System;

namespace ACT
{
    public class ActionManager
    {
        protected static readonly ActionManager msInstance = new ActionManager();
        public static ActionManager Instance { get { return msInstance; } }

        Dictionary<int, ActData.UnitActionInfo> mUnitActionInfos = new Dictionary<int, ActData.UnitActionInfo>();

        protected ActionManager()
        {

        }

        public void Clear()
        {
            mUnitActionInfos.Clear();
        }

        ActData.UnitActionInfo Load(int id)
        {
            if (null == ActionSystem.Instance.LoadActionFileDelegate)
            {
                Debug.LogError("没有设置加载委托");
                return null;
            }

            string actionTypeBin = $"Action_{id}";
            byte[] tmpDatas = ActionSystem.Instance.LoadActionFileDelegate($"Action_{id}");

            using (MemoryStream stream = new MemoryStream(tmpDatas))
            {
                ActData.UnitActionInfo actionData = new ActData.UnitActionInfo();
                actionData.MergeFrom(stream);
                stream.Close();
                return actionData;
            }
        }

        public ActData.UnitActionInfo GetUnitActionInfo(int id)
        {
            ActData.UnitActionInfo ret;
            if (!mUnitActionInfos.TryGetValue(id, out ret))
            {
                ret = Load(id);
                mUnitActionInfos[id] = ret;
            }
            return ret;
        }
    }
}
