using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosBaseFramework
{
    public abstract class UIBase
    {
        protected abstract string ResName { get; }

        //父节点
        protected Transform mParentNodeTrans;
        public Transform ParentNodeTrans
        {
            set { mParentNodeTrans = value; }
            get { return mParentNodeTrans; }
        }

        private GameObject mGameObject;
        protected GameObject RootGo { get { return mGameObject; } }

        private bool mIsInit = false;
        private bool mIsShowed = false;
        public bool IsShowed { get { return mIsShowed; } }

        private void Init()
        {
            if (mIsInit)
            {
                return;
            }

            mGameObject = GameObject.Instantiate(Resources.Load<GameObject>(ResName));

            if (mGameObject && mParentNodeTrans)
            {
                mGameObject.transform.SetParent(mParentNodeTrans, false);

                RectTransform tmpWndRT = mGameObject.transform as RectTransform;
                if (null != tmpWndRT)
                {
                    tmpWndRT.anchorMin = Vector2.zero;
                    tmpWndRT.anchorMax = Vector2.one;
                    tmpWndRT.offsetMin = Vector2.zero;
                    tmpWndRT.offsetMax = Vector2.zero;
                }

                mGameObject.SetActive(false);
                mIsInit = true;
                AfterInit();
            }
        }

        public void Show()
        {
            if (mIsShowed)
            {
                return;
            }

            if (!mIsInit)
            {
                Init();
            }

            if (mGameObject && !mGameObject.activeSelf)
            {
                mGameObject.SetActive(true);
            }

            mIsShowed = true;
            AfterShow();
        }

        public virtual void Update(float deltaTime)
        {

        }

        public void Close()
        {
            if (!mIsShowed)
            {
                return;
            }

            BeforeClose();

            if (mGameObject)
            {
                mGameObject.SetActive(false);
            }

            mIsShowed = false;
        }

        protected static void RegisterEventClick(GameObject go, UGUIEventListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIEventListener.Get(go).onClick += handle;
        }

        protected abstract void AfterInit();

        protected abstract void AfterShow();

        protected abstract void BeforeClose();


    }
}
