using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EWindowType = AosHotfixFramework.WindowBase.EWindowType;

namespace AosHotfixFramework
{
    internal sealed class WindowsManager : GameModuleBase, IWindowsManager
    {
        Transform mWindowRootTrans;
        List<WindowBase> mWindowsList = new List<WindowBase>();
        Dictionary<EWindowType, List<WindowBase>> mWinType2Windows = new Dictionary<EWindowType, List<WindowBase>>();
        //窗口排序间隔
        const int SortOrderBetweenWindow = 20;
        //子窗口与父窗口深度间隔
        public const int DepthBetweenParent = 2;
        //窗口类型对应的排序深度
        public static Dictionary<EWindowType, int> sWindowType2Order = new Dictionary<EWindowType, int>()
        {
            [EWindowType.None] = 0,
            [EWindowType.Main] = 200,
            [EWindowType.Normal] = 400,
            [EWindowType.Tips] = 600,
            [EWindowType.Guide] = 800,
            [EWindowType.Loading] = 1000,
            [EWindowType.SystemNotice] = 1200,
        };

        internal override int Priority
        {
            get
            {
                return 0;
            }
        }

        private float mDeltaTime = 0;
        private int mDeltaFrame = 0;
        private float mDeltaTime2 = 0;

        internal override void Update(float deltaTime)
        {
            mDeltaTime += deltaTime;
            ++mDeltaFrame;
            mDeltaTime2 += deltaTime;
            bool tmpIsSecondTime = mDeltaTime >= 1f;
            bool tmpIs10Frame = mDeltaFrame >= 10;

            for (int i = 0, max = mWindowsList.Count; i < max; ++i)
            {
                WindowBase tmpWindow = mWindowsList[i];

                if (tmpWindow.IsVisible && !tmpWindow.IsDestroyed)
                {
                    tmpWindow.InternalUpdate(deltaTime);

                    if (tmpIsSecondTime)
                    {
                        tmpWindow.InternalUpdateOneSecond(mDeltaTime);
                    }

                    if (tmpIs10Frame)
                    {
                        tmpWindow.InternalUpdate10Frame(mDeltaTime2);
                    }
                }
            }

            if (tmpIsSecondTime)
            {
                mDeltaTime = 0f;
            }

            if (tmpIs10Frame)
            {
                mDeltaFrame = 0;
                mDeltaTime2 = 0;
            }
        }

        internal override void LateUpdate(float deltaTime)
        {
        }

        internal override void Shutdown()
        {
            for (int i = 0, max = mWindowsList.Count; i < max; ++i)
            {
                WindowBase tmpWindow = mWindowsList[i];
                tmpWindow.Destroy();
            }

            mWindowsList.Clear();
        }

        public void SetWindowsRoot(Transform rootTrans)
        {
            mWindowRootTrans = rootTrans;
        }

        public T FindWindow<T>() where T : WindowBase
        {
            for (int i = 0, max = mWindowsList.Count; i < max; ++i)
            {
                WindowBase tmpWindow = mWindowsList[i];

                if (tmpWindow.GetType() == typeof(T))
                    return tmpWindow as T;
            }

            return default(T);
        }

        public T CreateWindow<T>() where T : WindowBase
        {
            T tmpWindow = FindWindow<T>();

            if (null == tmpWindow)
            {
                tmpWindow = Activator.CreateInstance<T>();
                tmpWindow.ParentNodeTrans = mWindowRootTrans;
                mWindowsList.Add(tmpWindow);
            }

            return tmpWindow;
        }

        public T PreLoadWindow<T>(Action loadedHandle = null) where T : WindowBase
        {
            T tmpWindow = CreateWindow<T>();
            tmpWindow.PreLoad(loadedHandle);

            return tmpWindow;
        }

        public T ShowWindow<T>(Action<WindowBase> showedHandle = null) where T : WindowBase
        {
            T tmpWindow = CreateWindow<T>();
            AddWindowInOrderList(tmpWindow);
            tmpWindow.Show(showedHandle);

            return tmpWindow;
        }

        public T ShowWindow<T, A>(A a, Action<WindowBase> showedHandle = null) where T : WindowBase
        {
            T tmpWindow = CreateWindow<T>();
            AddWindowInOrderList(tmpWindow);

            var tmpInitData = tmpWindow as WindowBase.IInitData<A>;
            if (null != tmpInitData)
            {
                tmpInitData.InitData(a);
            }

            tmpWindow.Show(showedHandle);

            return tmpWindow;
        }

        public T ShowWindow<T, A, B>(A a, B b, Action<WindowBase> showedHandle = null) where T : WindowBase
        {
            T tmpWindow = CreateWindow<T>();
            AddWindowInOrderList(tmpWindow);

            var tmpInitData = tmpWindow as WindowBase.IInitData2<A, B>;
            if (null != tmpInitData)
            {
                tmpInitData.InitData(a, b);
            }

            tmpWindow.Show(showedHandle);

            return tmpWindow;
        }

        public void CloseWindow<T>() where T : WindowBase
        {
            T tmpWindow = FindWindow<T>();

            if (null != tmpWindow)
            {
                RemoveWindowInOrderList(tmpWindow);
                tmpWindow.Close();
            }
        }

        public void CloseWindow(EWindowType windowType)
        {
            for (int i = 0, max = mWindowsList.Count; i < max; ++i)
            {
                WindowBase tmpWindow = mWindowsList[i];

                if (EWindowType.Invalid != (tmpWindow.WindowType & windowType) && tmpWindow.IsVisible)
                {
                    RemoveWindowInOrderList(tmpWindow);
                    tmpWindow.Close();
                }
            }
        }

        public void DestroyWindow<T>() where T : WindowBase
        {
            T tmpWindow = FindWindow<T>();

            if (null != tmpWindow)
            {
                tmpWindow.Destroy();
            }
        }

        void AddWindowInOrderList(WindowBase window)
        {
            List<WindowBase> tmpWindowList = null;

            if (!mWinType2Windows.TryGetValue(window.WindowType, out tmpWindowList))
            {
                tmpWindowList = new List<WindowBase>();
                mWinType2Windows.Add(window.WindowType, tmpWindowList);
            }

            //打开新窗口时 把已关闭的窗口或是同窗口移除掉
            for (int i = tmpWindowList.Count - 1; i >= 0; --i)
            {
                var tmpWnd = tmpWindowList[i];

                if (!tmpWnd.IsVisible || tmpWnd == window)
                {
                    tmpWindowList.RemoveAt(i);
                }
            }

            int tmpOrder = 0;
            sWindowType2Order.TryGetValue(window.WindowType, out tmpOrder);
            tmpWindowList.Add(window);

            for (int i = 0, max = tmpWindowList.Count; i < max; ++i)
            {
                var tmpWnd = tmpWindowList[i];
                tmpWnd.SetCanvsSortOrder(tmpOrder);
                tmpOrder += SortOrderBetweenWindow;
            }
        }

        void RemoveWindowInOrderList(WindowBase window)
        {
            List<WindowBase> tmpWindowList = null;

            if (!mWinType2Windows.TryGetValue(window.WindowType, out tmpWindowList))
            {
                return;
            }
            
            tmpWindowList.Remove(window);
        }
    }
}
