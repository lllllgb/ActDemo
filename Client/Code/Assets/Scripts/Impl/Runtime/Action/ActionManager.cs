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

        Dictionary<int, Data1.UnitActionInfo> mUnitActionInfos = new Dictionary<int, Data1.UnitActionInfo>();

        public Func<string, byte[]> LoadFileDelegate;

        protected ActionManager()
        {

        }

        public void Clear()
        {
            mUnitActionInfos.Clear();
        }

        Data1.UnitActionInfo Load(int id)
        {
            if (null == LoadFileDelegate)
            {
                Debug.LogError("没有设置加载委托");
                return null;
            }

            string actionTypeBin = $"Action_{id}";
            byte[] tmpDatas = LoadFileDelegate($"Action_{id}");

            using (MemoryStream stream = new MemoryStream(tmpDatas))
            {
                Data1.UnitActionInfo actionData = new Data1.UnitActionInfo();
                actionData.MergeFrom(stream);
                stream.Close();
                return actionData;
            }
        }

        public Data1.UnitActionInfo GetUnitActionInfo(int id)
        {
            Data1.UnitActionInfo ret;
            if (!mUnitActionInfos.TryGetValue(id, out ret))
            {
                ret = Load(id);
                mUnitActionInfos[id] = ret;
            }
            return ret;
        }
    }
}
