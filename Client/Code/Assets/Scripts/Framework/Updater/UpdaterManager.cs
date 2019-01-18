using System;
using System.Collections.Generic;
using UnityEngine;

namespace AosBaseFramework
{
    public class UpdaterManager : Singleton<UpdaterManager>
    {
        public enum EUpdaterType
        {
            Code,
            Resources,
        }

        const string RETRY_TIP = "过程中发生错误 按下按钮我们将重新尝试";
        const string DOWNLOAD_TIP = "有 {0}m 的资源需要更新才能继续游戏 是否立即下载?";
        const string DOWNLOAD_TIP_LESS = "有小于1m的资源需要更新才能继续游戏 是否立即下载?";

        Action<bool> mFinishHandle;

        private byte[] mCodeData;
        public byte[] CodeData { get { return mCodeData; } set { mCodeData = value; } }

        private Dictionary<uint, long> mResInfoDict;
        public Dictionary<uint, long> ResInfoDict { get { return mResInfoDict; } set { mResInfoDict = value; } }

        private Queue<UpdaterBase> mUpdaters = new Queue<UpdaterBase>();
        private UpdaterBase mCurrUpdater;

        public void StartUpdate(Action<bool> finishHandle)
        {
            mFinishHandle = finishHandle;

            InnerStart();
        }

        public void Retry()
        {
            InnerStart();
        }

        public void Update(float deltaTime)
        {
            if (null != mCurrUpdater)
            {
                mCurrUpdater.Update(deltaTime);
            }
        }

        private void InnerStart()
        {

#if TEST_HOTFIX
            mCodeData = FileHelper.ReadFile(Application.streamingAssetsPath + "/Hotfix.dll");
#else

            if (GameSetup.instance.IsPublish && GameSetup.instance.IsCodeUpdate)
            {
                mUpdaters.Enqueue(new CodeUpdater(this, EUpdaterType.Code));
            }
#endif

            if (GameSetup.instance.IsPublish && GameSetup.instance.IsResUpdate)
            {
                mUpdaters.Enqueue(new ResUpdater(this, EUpdaterType.Resources));
            }

            CheckUpdater();
        }

        //检查剩余下载器
        private bool CheckUpdater()
        {
            if (mUpdaters.Count == 0)
            {
                mFinishHandle?.Invoke(true);
                return false;
            }

            mCurrUpdater = mUpdaters.Dequeue();
            mCurrUpdater.Initialize(GameSetup.instance.UpdateUrl, PathHelper.AppHotfixResPath);
            mCurrUpdater.StartCheck(OnUpdateCheckFinish, OnUpdateBegin, OnUpdateFail, OnUpdateProgress, OnUpdateFinish);

            var tmpUI = TempUIMgr.Instance.ShowUI<UpdateUI>();
            tmpUI.ShowChecking();

            return true;
        }

        //检测更新完成
        private void OnUpdateCheckFinish(UpdaterBase updater)
        {
            if (EUpdaterType.Code == updater.UpdaterType)
            {
                return;
            }

            var tmpUI = TempUIMgr.Instance.ShowUI<MessageBoxUI>();
            int tmpTotalSize = (int)(mCurrUpdater.TotalSize / 1024f);
            string tmpContent = tmpTotalSize >= 1 ? string.Format(DOWNLOAD_TIP, tmpTotalSize) : DOWNLOAD_TIP_LESS;
            tmpUI.SetShowData(MessageBoxUI.EStyle.L_R, tmpContent, MessageBoxUI.CANCEL_STR, OnCancelDownload, null, null,
                MessageBoxUI.CONFIRM_STR, OnConfirmDownload);
        }

        //放弃下载
        private void OnCancelDownload()
        {
            mFinishHandle?.Invoke(false);
        }

        //确认下载
        private void OnConfirmDownload()
        {
            mCurrUpdater.StartDownload();
        }

        //开始更新
        private void OnUpdateBegin(UpdaterBase updater)
        {
            if (EUpdaterType.Code == updater.UpdaterType)
            {
                return;
            }

            var tmpUI = TempUIMgr.Instance.GetUI<UpdateUI>();
            tmpUI.ShowUpdateing();
        }

        //更新失败
        private void OnUpdateFail(UpdaterBase updater)
        {
            var tmpUI = TempUIMgr.Instance.ShowUI<MessageBoxUI>();
            tmpUI.SetShowData(MessageBoxUI.EStyle.Middle, RETRY_TIP, null, null, MessageBoxUI.RETRY_STR, OnRetry);
        }

        //重试
        private void OnRetry()
        {
            Retry();
        }

        //更新进度
        private void OnUpdateProgress(UpdaterBase updater)
        {
            if (EUpdaterType.Code == updater.UpdaterType)
            {
                return;
            }

            var tmpUI = TempUIMgr.Instance.GetUI<UpdateUI>();
            tmpUI.SetUpdateProgress(updater.Progress);
        }

        //更新完成
        private void OnUpdateFinish(UpdaterBase updater)
        {
            CheckUpdater();
        }
    }
}
