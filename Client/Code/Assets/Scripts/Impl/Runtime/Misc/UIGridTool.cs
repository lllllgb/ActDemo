using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;
using AosBaseFramework;

namespace AosHotfixRunTime
{
    public class UIGridTool
    {
        private GameObject mRootGo;
        private GameObject mTemplateGo;
        private List<GameObject> mElemList = new List<GameObject>();

        private int mCount;
        public int Count { get { return mCount; } }

        public UIGridTool(GameObject root, GameObject template, bool isGetChild = true)
        {
            if (null == root)
            {
                return;
            }

            mRootGo = root;
            mTemplateGo = template;
            mCount = root.transform.childCount;

            if (isGetChild)
            {
                for (int i = 0, max = mCount; i < max; ++i)
                {
                    mElemList.Add(root.transform.GetChild(i).gameObject);
                }
            }
        }

        public void GenerateElem(int count)
        {
            for (int i = 0, max = mCount > count ? mCount : count; i < max; ++i)
            {
                if (i >= mElemList.Count)
                {
                    Add();
                }
                else
                {
                    Utility.GameObj.SetActive(mElemList[i], i < count);
                }
            }

            mCount = count;
        }

        public GameObject Add()
        {
            if (null == mTemplateGo)
            {
                Logger.LogError("TemplateGo is null!");
                return null;
            }

            GameObject tmpNewElem = Hotfix.Instantiate(mTemplateGo);
            tmpNewElem.SetActive(true);
            tmpNewElem.transform.SetParent(mRootGo.transform, false);

            mElemList.Add(tmpNewElem);

            return tmpNewElem;
        }

        public void Add(GameObject go)
        {
            go.transform.SetParent(mRootGo.transform, false);

            mElemList.Add(go);
        }

        public void MoveToLast(int index)
        {
            if (index >= mElemList.Count)
            {
                return;
            }

            GameObject tmpElem = mElemList[index];
            mElemList.RemoveAt(index);
            mElemList.Add(tmpElem);
            tmpElem.transform.SetAsLastSibling();
        }

        public GameObject Get(int index)
        {
            return index >= mElemList.Count ? null : mElemList[index];
        }

        public void RemoveAt(int index)
        {
            mElemList.RemoveAt(index);
        }

        public void Remove(GameObject obj)
        {
            mElemList.Remove(obj);
            GameObject.Destroy(obj);
        }

        public void Clear()
        {
            int len = mElemList.Count;
            for (int i = 0; i < len; i++)
            {
                GameObject.Destroy(mElemList[i]);
            }

            mElemList.Clear();
        }
    }
}
