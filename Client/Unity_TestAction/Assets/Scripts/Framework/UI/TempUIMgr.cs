using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AosBaseFramework
{
    public class TempUIMgr : Singleton<TempUIMgr>
    {
        private GameObject mUIRoot;
        private GameObject mUIParent;

        private List<UIBase> mUIList = new List<UIBase>();

        public void Init()
        {
            mUIRoot = GameObject.Instantiate(Resources.Load<GameObject>("UIRoot"));
            mUIParent = Utility.GameObj.Find(mUIRoot, "CanvasRoot");
        }

        public void Reset()
        {
            if (mUIRoot)
            {
                GameObject.Destroy(mUIRoot);
            }

            mUIList.Clear();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0, max = mUIList.Count; i < max; ++i)
            {
                UIBase tmpUI = mUIList[i];

                if (tmpUI.IsShowed)
                {
                    tmpUI.Update(deltaTime);
                }
            }
        }

        public T GetUI<T>() where T : UIBase
        {
            UIBase tmpUI = null;

            for (int i = 0, max = mUIList.Count; i < max; ++i)
            {
                tmpUI = mUIList[i];

                if (tmpUI.GetType() == typeof(T))
                    return tmpUI as T;
            }

            tmpUI = Activator.CreateInstance<T>();
            tmpUI.ParentNodeTrans = mUIParent.transform;
            mUIList.Add(tmpUI);

            return tmpUI as T;
        }

        public T ShowUI<T>() where T : UIBase
        {
            T tmpUI = GetUI<T>();
            tmpUI.Show();

            return tmpUI;
        }

        public void CloseUI<T>() where T : UIBase
        {
            GetUI<T>().Close();
        }
    }
}
