using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AosBaseFramework;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AosHotfixFramework
{
    public abstract class WindowBase
    {
        public interface IInitData<A>
        {
            void InitData(A a);
        }

        public interface IInitData2<A, B>
        {
            void InitData(A a, B b);
        }

        [Flags]
        public enum EWindowType
        {
            Invalid = 0,
            None = 1 << 0,
            Main = 1 << 1,   //主UI
            Normal = 1 << 2, //普通UI
            Tips = 1 << 3, //提示
            Guide = 1 << 4, //引导
            Loading = 1 << 5, //加载
            SystemNotice = 1 << 6, //系统公告

            ALL = None | Main | Normal | Tips | Guide | Loading | SystemNotice,
            AllButMain = None | Normal | Tips | Guide | Loading | SystemNotice,
        }

        //父节点
        protected Transform mParentNodeTrans;
        public Transform ParentNodeTrans
        {
            set { mParentNodeTrans = value; }
            get { return mParentNodeTrans; }
        }

        //自身
        protected GameObject mGameObejct;
        //是否显示
        protected bool mIsVisible;
        public bool IsVisible
        {
            get { return mIsVisible; }
        }
        //是否初始化
        protected bool mIsInited;
        //是否在加载完后显示
        protected bool mIsShowAfterLoaded;
        //显示后操作
        protected Action<WindowBase> mShowedHandle;
        //是否已销毁
        protected bool mIsDestroyed;
        public bool IsDestroyed
        {
            get { return mIsDestroyed; }
        }
        //父窗口
        protected WindowBase mParentWindow;
        //子窗口
        protected List<WindowBase> mChildWindows = new List<WindowBase>();
        //该UI画布
        protected Dictionary<Canvas, int> mCanvas2OrderDict = new Dictionary<Canvas, int>();
        //窗口类型
        protected EWindowType mWindowType = EWindowType.Normal;
        public EWindowType WindowType
        {
            get { return mWindowType; }

            protected set { mWindowType = value; }
        }
        //是否异步加载
        protected virtual bool IsLoadAsync { get { return true; } }
        //排序
        protected int mOrder;
        public int Order
        {
            get { return mOrder; }
        }
        //bundle
        public abstract string BundleName { get; }

        protected virtual void InitSubWindow() { }

        protected virtual void AfterInit() { }

        protected virtual void AfterShow() { }

        protected virtual void BeforeClose() { }

        protected virtual void BeforeDestory() { }

        protected virtual void Update(float deltaTime) { }

        protected virtual void UpdateOneSecond(float deltaTime) { }

        protected virtual void Update10Frame(float deltaTime) { }

        protected virtual void ReSortOrder() { }

        private async void LoadWindowAsync(Action loadedHandle = null)
        {
            IResourcesManager tmpResMgr = GameModuleManager.GetModule<IResourcesManager>();
            await tmpResMgr.LoadBundleByTypeAsync(EABType.UI, BundleName);
            
            if (null != loadedHandle)
            {
                loadedHandle();
                loadedHandle = null;
            }

            if (mIsDestroyed || mIsInited)
            {
                tmpResMgr.UnLoadBundleByType(EABType.UI, BundleName);
                return;
            }
            
            GameObject tmpGo = Hotfix.Instantiate(tmpResMgr.GetAssetByType<GameObject>(EABType.UI, BundleName));
            InitWindow(tmpGo);

            if (mIsShowAfterLoaded)
                Show();
        }

        private void LoadWindowSync(Action loadedHandle = null)
        {
            IResourcesManager tmpResMgr = GameModuleManager.GetModule<IResourcesManager>();
            tmpResMgr.LoadBundleByType(EABType.UI, BundleName);

            GameObject tmpGo = Hotfix.Instantiate(tmpResMgr.GetAssetByType<GameObject>(EABType.UI, BundleName));
            InitWindow(tmpGo);
            loadedHandle?.Invoke();
        }

        private void InitWindow(GameObject uiObj)
        {
            if (null == uiObj || mIsInited)
            {
                return;
            }

            //基本初始
            uiObj.transform.SetParent(mParentNodeTrans, false);
            RectTransform tmpWndRT = uiObj.transform as RectTransform;
            if (null != tmpWndRT)
            {
                tmpWndRT.anchorMin = Vector2.zero;
                tmpWndRT.anchorMax = Vector2.one;
                tmpWndRT.offsetMin = Vector2.zero;
                tmpWndRT.offsetMax = Vector2.zero;
            }

            //收集canvas
            mCanvas2OrderDict.Clear();
            Canvas[] tmpCanvasList = uiObj.GetComponentsInChildren<Canvas>(true);
            for (int i = 0, max = tmpCanvasList.Length; i < max; ++i)
            {
                Canvas tmpCanvas = tmpCanvasList[i];

                if (tmpCanvas.overrideSorting)
                    mCanvas2OrderDict[tmpCanvas] = tmpCanvas.sortingOrder;
            }
            
            mGameObejct = uiObj;

            AfterInit();
            InitSubWindow();
            SetVisible(mIsShowAfterLoaded);
            mIsInited = true;
        }

        internal void SetCanvsSortOrder(int order)
        {
            mOrder = order;

            if (mIsVisible)
            {
                SortCanvas();
                ReSortOrder();
            }
        }

        private void SortCanvas()
        {
            foreach (var elem in mCanvas2OrderDict)
            {
                int tmpNewOrder = mOrder + elem.Value;

                if (elem.Key && elem.Key.sortingOrder != tmpNewOrder)
                    elem.Key.sortingOrder = tmpNewOrder;
            }
        }

        internal void InternalUpdate(float deltaTime)
        {
            Update(deltaTime);

            for (int i = 0, max = mChildWindows.Count; i < max; ++i)
            {
                WindowBase tmpChildWindow = mChildWindows[i];

                if (tmpChildWindow.IsVisible && !tmpChildWindow.IsDestroyed)
                {
                    tmpChildWindow.InternalUpdate(deltaTime);
                }
            }
        }

        internal void InternalUpdateOneSecond(float deltaTime)
        {
            UpdateOneSecond(deltaTime);

            for (int i = 0, max = mChildWindows.Count; i < max; ++i)
            {
                WindowBase tmpChildWindow = mChildWindows[i];

                if (tmpChildWindow.IsVisible && !tmpChildWindow.IsDestroyed)
                {
                    tmpChildWindow.InternalUpdateOneSecond(deltaTime);
                }
            }
        }

        internal void InternalUpdate10Frame(float deltaTime)
        {
            Update10Frame(deltaTime);

            for (int i = 0, max = mChildWindows.Count; i < max; ++i)
            {
                WindowBase tmpChildWindow = mChildWindows[i];

                if (tmpChildWindow.IsVisible && !tmpChildWindow.IsDestroyed)
                {
                    tmpChildWindow.InternalUpdate10Frame(deltaTime);
                }
            }
        }

        private void SetVisible(bool visible)
        {
            mIsVisible = visible;

            if (mGameObejct)
            {
                mGameObejct.SetActive(visible);
            }
        }

        public void PreLoad(Action loadedHandle = null)
        {
            if (mIsInited)
            {
                loadedHandle?.Invoke();
                return;
            }

            if (IsLoadAsync)
            {
                LoadWindowAsync(loadedHandle);
            }
            else
            {
                LoadWindowSync(loadedHandle);
            }
        }

        public void Show(Action<WindowBase> showedHandle = null)
        {
            if (null != showedHandle)
            {
                mShowedHandle += showedHandle;
            }

            if (!mIsInited)
            {
                if (IsLoadAsync)
                {
                    mIsShowAfterLoaded = true;
                    LoadWindowAsync();
                    return;
                }
                else
                {
                    LoadWindowSync();
                }
            }

            SetVisible(true);

            //若此为子窗口 设其深度与父窗口一致
            if (null != mParentWindow)
            {
                SetCanvsSortOrder(mParentWindow.Order + WindowsManager.DepthBetweenParent);
            }
            SortCanvas();

            AfterShow();

            if (null != mShowedHandle)
            {
                mShowedHandle(this);
                mShowedHandle = null;
            }
        }

        public void Close()
        {
            mShowedHandle = null;

            if (!mIsInited)
            {
                mIsShowAfterLoaded = false;
                return;
            }

            if (!mIsVisible)
            {
                return;
            }

            for (int i = 0, max = mChildWindows.Count; i < max; ++i)
            {
                WindowBase tmpChildWindow = mChildWindows[i];
                tmpChildWindow.Close();
            }

            BeforeClose();

            SetVisible(false);
        }

        public void Destroy()
        {
            BeforeDestory();
            mIsInited = false;
            mIsDestroyed = true;

            if (mGameObejct)
            {
                GameObject.Destroy(mGameObejct);
                mGameObejct = null;
                IResourcesManager tmpResMgr = GameModuleManager.GetModule<IResourcesManager>();
                tmpResMgr.UnLoadBundleByType(EABType.UI, BundleName);
            }
        }

        public T GetChildWindow<T>() where T : WindowBase
        {
            for (int i = 0, max = mChildWindows.Count; i < max; ++i)
            {
                WindowBase tmpChildWindow = mChildWindows[i];

                if (null != tmpChildWindow && tmpChildWindow.GetType() == typeof(T))
                    return tmpChildWindow as T;
            }

            return null;
        }

        protected T CreateChildWindow<T>(Transform parentNode, GameObject childGo = null) where T : WindowBase
        {
            WindowBase tmpChildWindow = GetChildWindow<T>();

            if (null == tmpChildWindow)
            {
                tmpChildWindow = Activator.CreateInstance<T>();
                tmpChildWindow.mParentWindow = this;
                tmpChildWindow.mParentNodeTrans = parentNode;
                tmpChildWindow.InitWindow(childGo);
                mChildWindows.Add(tmpChildWindow);
            }

            return tmpChildWindow as T;
        }

        protected T Find<T>(string name) where T : MonoBehaviour
        {
            return Utility.GameObj.Find<T>(mGameObejct, name);
        }

        protected static T Find<T>(GameObject go, string name) where T : MonoBehaviour
        {
            return Utility.GameObj.Find<T>(go, name);
        }

        protected GameObject Find(string name)
        {
            return Utility.GameObj.Find(mGameObejct, name);
        }

        protected static GameObject Find(GameObject go, string name)
        {
            return Utility.GameObj.Find(go, name);
        }

        public static void SetActive(GameObject go, bool active)
        {
            Utility.GameObj.SetActive(go, active);
        }

        public static void SetActive<T>(T instance, bool active) where T : UnityEngine.Component
        {
            Utility.GameObj.SetActive<T>(instance, active);
        }

        protected static void RegisterEventClick(GameObject go, UGUIEventListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIEventListener.Get(go).onClick = handle;
        }

        protected static void RegisterEventDoubleClick(GameObject go, UGUIEventListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIEventListener.Get(go).onDoubleClick = handle;
        }

        protected static void RegisterEventClickDown(GameObject go, UGUIEventListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIEventListener.Get(go).onDown = handle;
        }

        protected static void RegisterEventClickUp(GameObject go, UGUIEventListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIEventListener.Get(go).onUp = handle;
        }
        protected static void RegisterEventClickEnter(GameObject go, UGUIEventListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIEventListener.Get(go).onEnter = handle;
        }
        protected static void RegisterEventClickExit(GameObject go, UGUIEventListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIEventListener.Get(go).onExit = handle;
        }
        protected static void RegisterEventClickBeginDrag(GameObject go, UGUIDrogListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIDrogListener.Get(go).onBeginDrag = handle;
        }
        protected static void RegisterEventClickDrag(GameObject go, UGUIDrogListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIDrogListener.Get(go).onDrag = handle;
        }
        protected static void RegisterEventClickEndDrag(GameObject go, UGUIDrogListener.VoidDelegate handle)
        {
            if (null == go || null == handle)
                return;

            UGUIDrogListener.Get(go).onEndDrag = handle;
        }
    }
}
