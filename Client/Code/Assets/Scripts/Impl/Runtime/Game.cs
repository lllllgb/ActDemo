using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using ResourcesSystem = AosBaseFramework.ResourcesSystem;

namespace AosHotfixRunTime
{
    public static class Game
    {
        public static IObjectPoolManager PoolMgr { get; private set; }
        public static IEventManager EventMgr { get; private set; }
        public static INetworkManager NetworkMgr { get; private set; }
        public static IResourcesManager ResourcesMgr { get; private set; }
        public static IFsmManager FsmMgr { get; private set; }
        public static IProcedureManager ProcedureMgr { get; private set; }
        public static IControllerManager ControllerMgr { get; private set; }
        public static IWindowsManager WindowsMgr { get; private set; }
        public static IObjectManager ObjectMgr { get; private set; }
        public static ITimerManager TimerMgr { get; private set; }

        //public static GameObject PoolObjectRoot { get; private set; }

        public static void Init()
        {
            InitGameModule();

            ResourcesSystem.Init();
            //依赖
            ResourcesMgr.LoadManifest();
            ResourcesMgr.LoadBundleByType(EABType.Shader, "shaderCollect");

            ACT.ActionManager.Instance.LoadFileDelegate = LoadAction;
            //UI
            //ResourcesMgr.LoadBundleByType(EABType.UI, "UIRoot");
            //GameObject tmpUIRoot = Hotfix.Instantiate(ResourcesMgr.GetAssetByType<GameObject>(EABType.UI, "UIRoot"));
            //GameObject.DontDestroyOnLoad(tmpUIRoot);
            //CameraHelper.InitUICamera(tmpUIRoot);
            //WindowsMgr.SetWindowsRoot(CameraHelper.UICanvasRootGo.transform);

            ////主相机
            ResourcesMgr.LoadBundleByType(EABType.Misc, "CameraControl");
            GameObject tmpMainCameraGo = Hotfix.Instantiate(ResourcesMgr.GetAssetByType<GameObject>(EABType.Misc, "CameraControl"));
            GameObject.DontDestroyOnLoad(tmpMainCameraGo);
            CameraHelper.Init(tmpMainCameraGo);

            ////HUD相机
            //ResourcesMgr.LoadBundleByType(EABType.Misc, "HUD");
            //GameObject tmpHudGo = ResourcesMgr.GetAssetByType<GameObject>(EABType.Misc, "HUD");
            //tmpHudGo = Hotfix.Instantiate(tmpHudGo);
            //GameObject.DontDestroyOnLoad(tmpHudGo);
            //CameraHelper.InitHudCamera(tmpHudGo);

            LoadTbl();

            //流程
            ProcedureMgr.Initialize(FsmMgr);
            ProcedureMgr.AddProcedure<ProcedureTestAction>();
            ProcedureMgr.AddProcedure<ProcedureCheckVersion>();
            ProcedureMgr.AddProcedure<ProcedureLogin>();
            ProcedureMgr.AddProcedure<ProcedureChangeScene>();
            ProcedureMgr.AddProcedure<ProcedureMain>();

            ProcedureMgr.StartProcedure<ProcedureTestAction>();
        }

        static byte[] LoadAction(string actionName)
        {
            return AosBaseFramework.FileHelper.LoadBytesFile($"{actionName}.bytes", AosBaseFramework.PathHelper.EBytesFileType.Action);
        }

        static void LoadTbl()
        {
            UnitBaseManager.instance.Load(string.Empty);
        }

        public static void InitGameModule()
        {
            GameModuleManager.CreateModule<ObjectPoolManager>();
            PoolMgr = GameModuleManager.GetModule<IObjectPoolManager>();
            GameModuleManager.CreateModule<EventManager>();
            EventMgr = GameModuleManager.GetModule<IEventManager>();
            GameModuleManager.CreateModule<NetworkManager>();
            NetworkMgr = GameModuleManager.GetModule<INetworkManager>();
            GameModuleManager.CreateModule<ResourcesManager>();
            ResourcesMgr = GameModuleManager.GetModule<IResourcesManager>();
            GameModuleManager.CreateModule<FsmManager>();
            FsmMgr = GameModuleManager.GetModule<IFsmManager>();
            GameModuleManager.CreateModule<ProcedureManager>();
            ProcedureMgr = GameModuleManager.GetModule<IProcedureManager>();
            GameModuleManager.CreateModule<ControllerManager>();
            ControllerMgr = GameModuleManager.GetModule<IControllerManager>();
            GameModuleManager.CreateModule<WindowsManager>();
            WindowsMgr = GameModuleManager.GetModule<IWindowsManager>();
            GameModuleManager.CreateModule<ObjectManager>();
            ObjectMgr = GameModuleManager.GetModule<IObjectManager>();
            GameModuleManager.CreateModule<TimerManager>();
            TimerMgr = GameModuleManager.GetModule<ITimerManager>();
        }
    }
}
