using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using AosHotfixFramework;
using ProcedureOwner = AosHotfixFramework.IFsm<AosHotfixFramework.IProcedureManager>;

namespace AosHotfixRunTime
{
    public class ProcedureChangeScene : ProcedureBase
    {
        ProcedureOwner mProcedureOwner;
        bool mIsSceneLoaded;

        Type mNextProcedureType;
        ProcedureBase mNextProcedureInstance;
        LoadingWnd mLoadingWnd;

        protected internal override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);

            mProcedureOwner = procedureOwner;
        }

        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            mLoadingWnd = Game.WindowsMgr.ShowWindow<LoadingWnd>();
            mIsSceneLoaded = false;
            LoadScene(ProcedureDataCache.ChangeSceneName);
        }

        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float deltaTime)
        {
            base.OnUpdate(procedureOwner, deltaTime);

            if (mLoadingWnd.Finish)
            {
                ChangeState(procedureOwner, mNextProcedureType);
                return;
            }

            if (mIsSceneLoaded)
            {
                bool tmpIsResLoaded = true;

                if (null != mNextProcedureInstance && mNextProcedureInstance is IProcedureLoadRes)
                {
                    tmpIsResLoaded = (mNextProcedureInstance as IProcedureLoadRes).LoadProgress() >= 100;
                }

                if (tmpIsResLoaded)
                {
                    mLoadingWnd.IsResLoaded = true;
                }
            }
            else
            {

            }
        }

        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            mIsSceneLoaded = false;
            mNextProcedureType = null;
            mNextProcedureInstance = null;
            ProcedureDataCache.ChangeSceneID = 0;
            ProcedureDataCache.NxProcedure = EProcedureType.Invalid;

            Game.WindowsMgr.CloseWindow<LoadingWnd>();
        }

        protected internal override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        async void LoadScene(string name)
        {
            await Game.ResourcesMgr.LoadBundleByTypeAsync(EABType.Scene, name);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync(name);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            mIsSceneLoaded = true;
            mNextProcedureType = ProcedureHelper.GetProcedureByType(ProcedureDataCache.NxProcedure);

            if (null == mNextProcedureType)
            {
                Logger.LogError("无效的跳转流程!");
                return;
            }

            mNextProcedureInstance = mProcedureOwner.GetStateByType(mNextProcedureType) as ProcedureBase;
            IProcedureLoadRes tmpProcedureLoader = mNextProcedureInstance as IProcedureLoadRes;

            if (null != tmpProcedureLoader)
            {
                tmpProcedureLoader.PreLoadRes();
            }
        }
    }
}
