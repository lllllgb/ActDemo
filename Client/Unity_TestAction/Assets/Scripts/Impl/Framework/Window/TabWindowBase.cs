using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using AosBaseFramework;

namespace AosHotfixFramework
{
    public abstract class TabWindowBase : WindowBase
    {
        const string NORMAL_OBJ_NAME = "Image_Page_Normal";
        const string ACTIVE_OBJ_NAME = "Image_Page_Active";
        const string DISABLE_OBJ_NAME = "Image_Page_Disable";

        private WindowBase mCurrentChildWnd = null;

        public WindowBase CurrentChildWnd
        {
            get { return mCurrentChildWnd; }
            set { mCurrentChildWnd = value; }
        }

        public class TabItemElem
        {
            public GameObject rootObj;
            public GameObject normalObj;
            public GameObject activeObj;
            public GameObject disableObj;

            bool mIsLocked = false;
            public bool isLocked { get { return mIsLocked; } }

            public void SetActive(bool flag)
            {
                if (mIsLocked)
                    return;
                
                Utility.GameObj.SetActive(normalObj, !flag);
                Utility.GameObj.SetActive(activeObj, flag);
            }

            public void SetLock(bool flag)
            {
                mIsLocked = flag;
                Utility.GameObj.SetActive(normalObj, !flag);
                Utility.GameObj.SetActive(activeObj, false);
                Utility.GameObj.SetActive(disableObj, flag);
            }
        }

        protected Dictionary<int, WindowBase> mRelationDict = new Dictionary<int, WindowBase>();
        protected List<TabItemElem> mToggleList = new List<TabItemElem>();
        protected int mCurrentTabIndex = -1;
        protected Func<int, WindowBase, bool> mCheckTabEnableHandle;
        protected Action<int, WindowBase> mClickSucceedHandle;
        protected Func<int, WindowBase, bool> mCheckTabShowHandle;
        protected List<int> mKeys = new List<int>();

        /// <summary>
        /// 设置导航子窗口
        /// </summary>
        /// <param name="go">父节点</param>
        /// <param name="succeedHandle">点击成功回调</param>
        /// <param name="checkTabEnableHandle">点击检测</param>
        /// <param name="checkTabShowHandle">显示检测</param>
        protected void SetTabRoot(GameObject go, Action<int, WindowBase> succeedHandle = null, 
            Func<int, WindowBase, bool> checkTabEnableHandle = null, Func<int, WindowBase, bool> checkTabShowHandle = null)
        {
            if (null == go)
                return;

            mCheckTabEnableHandle = checkTabEnableHandle;
            mClickSucceedHandle = succeedHandle;
            mCheckTabShowHandle = checkTabShowHandle;

            for (int i = 0, max = go.transform.childCount; i < max; ++i)
            {
                TabItemElem tempTabItem = new TabItemElem();
                Transform tempTrans = go.transform.GetChild(i);
                tempTabItem.rootObj = tempTrans.gameObject;
                tempTabItem.normalObj = Utility.GameObj.Find(tempTabItem.rootObj, NORMAL_OBJ_NAME);
                tempTabItem.activeObj = Utility.GameObj.Find(tempTabItem.rootObj, ACTIVE_OBJ_NAME);
                tempTabItem.disableObj = Utility.GameObj.Find(tempTabItem.rootObj, DISABLE_OBJ_NAME);
                tempTabItem.SetLock(false);
                mToggleList.Add(tempTabItem);
                OnClickTab(tempTrans.gameObject, i);
            }
        }

        protected void AddRelation(int index, WindowBase childWindow)
        {
            if (null == childWindow)
                return;
            if (mRelationDict.ContainsKey(index))
                mRelationDict[index] = childWindow;
            else
                mRelationDict.Add(index, childWindow);

            if (!mKeys.Contains(index))
                mKeys.Add(index);
        }

        protected void CheckTabEnable()
        {
            if (null == mCheckTabEnableHandle)
                return;

            for (int i = 0, max = mKeys.Count; i < max; ++i)
            {
                int tempKey = mKeys[i];

                if (tempKey < 0 || tempKey >= mToggleList.Count)
                    continue;

                TabItemElem tempItem = mToggleList[tempKey];
                tempItem.SetLock(!mCheckTabEnableHandle(tempKey, mRelationDict[tempKey]));
            }
        }

        protected void CheckTabShow()
        {
            for (int i = 0; i < mKeys.Count; i++)
            {
                int key = mKeys[i];
                WindowBase childWnd = mRelationDict[key];
                if (mCheckTabShowHandle != null)
                {
                    bool flag = mCheckTabShowHandle(key, childWnd);

                    if (key >= 0 && key < mToggleList.Count)
                    {
                        Utility.GameObj.SetActive(mToggleList[key].rootObj, flag);
                    }
                }
            }
        }

        public void TabIndex(int index)
        {
            if (index < 0 || index >= mToggleList.Count || mCurrentTabIndex == index)
                return;

            mCurrentTabIndex = index;

            for (int i = 0; i < mToggleList.Count; i++)
            {
                bool tempFlag = (i == index);
                mToggleList[i].SetActive(tempFlag);
            }

            if (mRelationDict.ContainsKey(index))
                TabSucceed(mRelationDict[index]);
        }

        void OnClickTab(GameObject go, int index)
        {
            if (go == null)
                return;

            UGUIEventListener.Get(go).onClick = delegate (PointerEventData eventData)
            {
                if (index == mCurrentTabIndex || !mRelationDict.ContainsKey(index))
                    return;

                WindowBase tempWindow = mRelationDict[index];
                TabItemElem tempTabItem = mToggleList[index];
                
                if (mCheckTabEnableHandle != null)
                {
                    if (!mCheckTabEnableHandle(index, tempWindow))
                        return;
                }

                for (int i = 0; i < mToggleList.Count; i++)
                {
                    mToggleList[i].SetActive(i == index);
                }

                mCurrentTabIndex = index;
                TabSucceed(tempWindow);
            };
        }

        void TabSucceed(WindowBase childWindow)
        {
            if (null == childWindow)
                return;

            mCurrentChildWnd = childWindow;
            childWindow.Show();

            for (int i = 0; i < mKeys.Count; i++)
            {
                WindowBase elem = mRelationDict[mKeys[i]];
                if (elem != childWindow)
                    elem.Close();
            }

            if (null != mClickSucceedHandle)
                mClickSucceedHandle(mCurrentTabIndex, childWindow);
        }

        public void Reset()
        {
            mCurrentTabIndex = -1;
            mCurrentChildWnd = null;

            for (int i = 0, max = mToggleList.Count; i < max; i++)
            {
                mToggleList[i].SetActive(false);
            }
        }
    }
}
