using UnityEngine;
using System;

namespace AosBaseFramework
{
	public static class Game
    {
        public const string ENTRY_CLASS_NAME = "AosHotfixRunTime.GameEntry";
        public const string INIT_METHOD_NAME = "Initialize";
        public const string UPDATE_METHOD_NAME = "Update";
        public const string LATE_UPDATE_METHOD_NAME = "LateUpdate";
        public const string FIXED_UPDATE_METHOD_NAME = "FixedUpdate";
        public const string CLOSE_METHOD_NAME = "Close";
        public const string APPLICATION_QUIT_METHOD_NAME = "ApplicationQuit";

        private static ObjectPool objectPool;

		public static ObjectPool ObjectPool
		{
			get
			{
				return objectPool ?? (objectPool = new ObjectPool());
			}
		}

        public static void Initialize()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            GameSetup.instance.Load();
            PathHelper.Initalize();
            TempUIMgr.Instance.Init();

            UpdaterManager.Instance.StartUpdate(OnUpdateFinish);
        }

        public static void Update()
        {
            float tmpDeltaTime = Time.deltaTime;
            
            ResourcesManager.Instance.Update(tmpDeltaTime);
            UpdaterManager.Instance.Update(tmpDeltaTime);
            TempUIMgr.Instance.Update(tmpDeltaTime);
        }

        public static void LateUpdate()
        {
        }

        public static void FixedUpdate()
        {
        }

        public static void Close()
        {
            objectPool = null;
        }

        public static void ApplicationQuit()
        {
        }

        static void OnUpdateFinish(bool succeed)
        {
            //更新失败
            if (!succeed)
            {
                return;
            }

            TempUIMgr.Instance.Reset();
            HotFixHelper.LoadHotfixAssembly(UpdaterManager.Instance.CodeData);

            AppFacade tmpAppFacade = AppFacade.Instance;
            IStaticMethod tmpInitMethod = null;
#if ILRuntime
            tmpInitMethod = new ILStaticMethod(HotFixHelper.Appdomain, ENTRY_CLASS_NAME, INIT_METHOD_NAME, 0);
            tmpAppFacade.UpdateMethod = new ILStaticMethod(HotFixHelper.Appdomain, ENTRY_CLASS_NAME, UPDATE_METHOD_NAME, 0);
            tmpAppFacade.LateUpdateMethod = new ILStaticMethod(HotFixHelper.Appdomain, ENTRY_CLASS_NAME, LATE_UPDATE_METHOD_NAME, 0);
            tmpAppFacade.FixedUpdateMethod = new ILStaticMethod(HotFixHelper.Appdomain, ENTRY_CLASS_NAME, FIXED_UPDATE_METHOD_NAME, 0);
            tmpAppFacade.CloseMethod = new ILStaticMethod(HotFixHelper.Appdomain, ENTRY_CLASS_NAME, CLOSE_METHOD_NAME, 0);
            tmpAppFacade.ApplicationQuitMethod = new ILStaticMethod(HotFixHelper.Appdomain, ENTRY_CLASS_NAME, APPLICATION_QUIT_METHOD_NAME, 0);
#else
            Type tmpType = HotFixHelper.HotFixAssembly.GetType(ENTRY_CLASS_NAME);
            tmpInitMethod = new MonoStaticMethod(tmpType, INIT_METHOD_NAME);
            tmpAppFacade.UpdateMethod = new MonoStaticMethod(tmpType, UPDATE_METHOD_NAME);
            tmpAppFacade.LateUpdateMethod = new MonoStaticMethod(tmpType, LATE_UPDATE_METHOD_NAME);
            tmpAppFacade.FixedUpdateMethod = new MonoStaticMethod(tmpType, FIXED_UPDATE_METHOD_NAME);
            tmpAppFacade.CloseMethod = new MonoStaticMethod(tmpType, CLOSE_METHOD_NAME);
            tmpAppFacade.ApplicationQuitMethod = new MonoStaticMethod(tmpType, APPLICATION_QUIT_METHOD_NAME);
#endif
            tmpInitMethod.Run();
        }
    }
}