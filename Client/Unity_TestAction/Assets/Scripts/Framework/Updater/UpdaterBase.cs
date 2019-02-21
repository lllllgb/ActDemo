using System;
using UnityEngine;
using System.IO;

namespace AosBaseFramework
{
    public abstract class UpdaterBase
    {
        public enum EPosType
        {
            ApkInner,
            Local,
            Server,
        }

        private UpdaterManager mUpdaterMgr;
        protected UpdaterManager UpdaterMgr { get { return mUpdaterMgr; } }

        private UpdaterManager.EUpdaterType mUpdaterType;
        public UpdaterManager.EUpdaterType UpdaterType { get { return mUpdaterType; } }

        private string mUrl = string.Empty;
        protected string URL { get { return mUrl; } }

        private string mLocalPath = string.Empty;
        protected string LocalPath { get { return mLocalPath; } }

        protected abstract string VersionFile { get; }

        private VersionData mServerVersionData;
        protected VersionData ServerVersionData { get { return mServerVersionData; } }

        private VersionData mLocalVersionData;
        protected VersionData LocalVersionData { get { return mLocalVersionData; } }

        private VersionData mApkVersionData;
        protected VersionData ApkVersionData { get { return mApkVersionData; } }

        private Action<UpdaterBase> mCheckFinishHandle;
        protected Action<UpdaterBase> CheckFinishHandle { get { return mCheckFinishHandle; } }

        private Action<UpdaterBase> mBeginHandle;
        protected Action<UpdaterBase> BeginHandle { get { return mBeginHandle; } }

        private Action<UpdaterBase> mFailHandle;
        protected Action<UpdaterBase> FailHandle { get { return mFailHandle; } }

        private Action<UpdaterBase> mDownloadProgressHandle;
        protected Action<UpdaterBase> DownloadProgressHandle { get { return mDownloadProgressHandle; } }

        private Action<UpdaterBase> mFinishHandle;
        protected Action<UpdaterBase> FinishHandle { get { return mFinishHandle; } }

        private UnityWebRequestAsync mWebRequestAsync;
        protected UnityWebRequestAsync WebRequestAsync { get { return mWebRequestAsync; } set { mWebRequestAsync = value; } }


        private long mTotalSize = 1;
        public long TotalSize { get { return mTotalSize; } set { mTotalSize = value; } }

        public abstract int Progress { get; }

        public UpdaterBase(UpdaterManager updaterMgr, UpdaterManager.EUpdaterType eUpdaterType)
        {
            mUpdaterMgr = updaterMgr;
            mUpdaterType = eUpdaterType;
        }

        public virtual void Initialize(string url, string localPath)
        {
            mUrl = url;
            mLocalPath = localPath;
        }

        public virtual void StartCheck(Action<UpdaterBase> checkFinish, Action<UpdaterBase> begin, 
            Action<UpdaterBase> fail, Action<UpdaterBase> downloadProgress, Action<UpdaterBase> finish)
        {
            mCheckFinishHandle = checkFinish;
            mBeginHandle = begin;
            mFailHandle = fail;
            mDownloadProgressHandle = downloadProgress;
            mFinishHandle = finish;

            DownloadVersionFileAsync();
        }

        public virtual void StartDownload()
        {
        }

        public virtual void Update(float deltaTime)
        {
            if (null != mWebRequestAsync)
            {
                mWebRequestAsync.Update();
            }
        }

        public virtual void Reset()
        {
            mServerVersionData = null;
            mLocalVersionData = null;
            mApkVersionData = null;

            mBeginHandle = null;
            mFailHandle = null;
            mFinishHandle = null;

            if (null != mWebRequestAsync)
            {
                mWebRequestAsync.Dispose();
                mWebRequestAsync = null;
            }

            mTotalSize = 1;
        }

        protected virtual void Failed()
        {
            mFailHandle?.Invoke(this);
        }

        //下载版本描述
        protected virtual async void DownloadVersionFileAsync()
        {
            using (UnityWebRequestAsync webRequestAsync = Game.ObjectPool.Fetch<UnityWebRequestAsync>())
            {
                mWebRequestAsync = webRequestAsync;
                string tmpUrl = mUrl + VersionFile;
                bool tmpResult = await webRequestAsync.DownloadAsync(tmpUrl);
                mWebRequestAsync = null;

                if (tmpResult)
                {
                    LoadVersionFile(webRequestAsync.Request.downloadHandler.data);
                }
                else
                {
                    Failed();
                }
            }
        }

        //加载版本描述
        protected virtual void LoadVersionFile(byte[] data)
        {
            //服务器上的VersionData
            mServerVersionData = VersionData.LoadVersionData(data);

            //加载包内VersionData
            TextAsset tmpApkTextAsset = Resources.Load<TextAsset>(VersionFile);
            if (tmpApkTextAsset != null)
            {
                mApkVersionData = VersionData.LoadVersionData(tmpApkTextAsset.bytes);
            }

            //加载本地VersionData
            string tmpLocalVersionPath = $"{mLocalPath}/{VersionFile}";
            if (File.Exists(tmpLocalVersionPath))
            {
                byte[] localData = FileHelper.ReadFile(tmpLocalVersionPath);
                mLocalVersionData = VersionData.LoadVersionData(localData);
            }

            if (!mServerVersionData.initalize || !mApkVersionData.initalize)
            {
                Logger.LogError("加载code的Version:" + VersionFile + "失败");
                Failed();

                return;
            }

            //需要强更客户端
            if (mApkVersionData.major != mServerVersionData.major)
            {
                ForceUpdateClient();
                return;
            }

            do
            {
                //包内版本数据相同
                if (mApkVersionData.ResDescCrc == mServerVersionData.ResDescCrc)
                {
                    TextAsset tmpTextAsset = Resources.Load(mApkVersionData.ResDescCrc.ToString()) as TextAsset;

                    if (null != tmpTextAsset)
                    {
                        LoadDescribe(tmpTextAsset.bytes, EPosType.ApkInner);
                    }
                    else
                    {
                        Failed();
                    }

                    break;
                }

                string tmpFileListPath = $"{mLocalPath}/{mServerVersionData.ResDescCrc}";

                //本地版本数据相同且资源存在
                if (null != mLocalVersionData && mLocalVersionData.ResDescCrc == mServerVersionData.ResDescCrc && File.Exists(tmpFileListPath))
                {
                    byte[] tmpLocalDescData = FileHelper.ReadFile(tmpFileListPath);

                    if (null != tmpLocalDescData)
                    {
                        LoadDescribe(tmpLocalDescData, EPosType.Local);
                    }
                    else
                    {
                        Failed();
                    }

                    break;
                }

                Save(data, VersionFile);
                DownloadDescribeFileAsync();

            } while (false);
        }

        //下载资源文件
        protected virtual async void DownloadDescribeFileAsync()
        {
            using (UnityWebRequestAsync webRequestAsync = Game.ObjectPool.Fetch<UnityWebRequestAsync>())
            {
                mWebRequestAsync = webRequestAsync;
                string tmpUrl = mUrl + mServerVersionData.ResDescCrc;
                bool tmpResult = await webRequestAsync.DownloadAsync(tmpUrl);
                mWebRequestAsync = null;

                if (tmpResult)
                {
                    Save(webRequestAsync.Request.downloadHandler.data, mServerVersionData.ResDescCrc.ToString());

                    LoadDescribe(webRequestAsync.Request.downloadHandler.data, EPosType.Server);
                }
                else
                {
                    Failed();
                }
            }
        }

        //加载资源文件
        protected virtual void LoadDescribe(byte[] data, EPosType ePosType)
        {

        }

        protected void Save(byte[] data, string fileName)
        {
            if (data == null)
                return;

            string path = $"{mLocalPath}/{fileName}";
            string newPath = path + ".tmp";
            FileHelper.WriteFile(newPath, data);

            if (File.Exists(path))
                File.Delete(path);

            File.Move(newPath, path);
        }

        private void ForceUpdateClient()
        {
            if (GameSetup.instance.clientUrl.Length > 0)
            {
                string str = GameSetup.instance.clientUrl[GameSetup.instance.clientUrl.Length - 1].ToString();
                if (str != "/")
                    GameSetup.instance.clientUrl += "/";
            }
        }
    }
}
