using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AosBaseFramework
{
    public class ResUpdater : UpdaterBase
    {
        private string mVersionFile = FileHelper.ResVersionFileCRC.ToString();
        protected override string VersionFile
        {
            get
            {
                return mVersionFile;
            }
        }

        private Dictionary<uint, long> mResInfoDict;
        private Queue<uint> mBeDownloadResKey = new Queue<uint>();
        private long mDownloadedSize = 0;

        public override int Progress
        {
            get
            {
                return (int)(mDownloadedSize * 100f / TotalSize);
            }
        }

        public ResUpdater(UpdaterManager updaterMgr, UpdaterManager.EUpdaterType eUpdaterType) :
            base(updaterMgr, eUpdaterType)
        {
        }

        public override void StartDownload()
        {
            base.StartDownload();
            
            DownloadResListAsync();
        }

        public override void Reset()
        {
            base.Reset();

            mBeDownloadResKey.Clear();
            mDownloadedSize = 0;
        }

        protected override void DownloadDescribeFileAsync()
        {
            base.DownloadDescribeFileAsync();
        }

        protected override void LoadDescribe(byte[] data, EPosType ePosType)
        {
            base.LoadDescribe(data, ePosType);

            Dictionary<uint, long> tmpNewestDescDict = ResourcesSystem.LoadResDescInfo(data);
            mResInfoDict = tmpNewestDescDict;

            //包内资源 不需要作比对
            if (EPosType.ApkInner == ePosType)
            {
                Finish();
                return;
            }

            TextAsset tmpTextAsset = Resources.Load<TextAsset>(ApkVersionData.ResDescCrc.ToString());
            if (null == tmpTextAsset)
            {
                Logger.LogError($"包内找不到资源描述文件 {ApkVersionData.ResDescCrc}");
                Failed();
                return;
            }

            Dictionary<uint, long> tmpApkResDescDict = ResourcesSystem.LoadResDescInfo(tmpTextAsset.bytes);
            List<uint> tmpInLocalResKeyList = new List<uint>();

            var tmpIter = tmpNewestDescDict.GetEnumerator();

            while (tmpIter.MoveNext())
            {
                var tmpKV = tmpIter.Current;
                var tmpResInfo = ResourcesSystem.ResFlag2Info(tmpKV.Value);

                long tmpApkResFlag = 0;
                //此资源在初始包资源里
                if (tmpApkResDescDict.TryGetValue(tmpKV.Key, out tmpApkResFlag))
                {
                    //此资源hash值未变动/资源未变更
                    if (tmpResInfo.FileHash == (uint)tmpApkResFlag)
                    {
                        continue;
                    }
                }

                //更改此资源位置描述 ->本地
                tmpInLocalResKeyList.Add(tmpKV.Key);

                //如此资源不存在于本地 需从资源服下载
                if (!File.Exists($"{LocalPath}/{tmpResInfo.FileHash}"))
                {
                    mBeDownloadResKey.Enqueue(tmpKV.Key);
                    TotalSize += tmpResInfo.ResSize;
                }
                
                tmpResInfo.Dispose();
            }
            
            //更改资源位置描述 -> 本地
            for (int i = 0, max = tmpInLocalResKeyList.Count; i < max; ++i)
            {
                uint tmpKey = tmpInLocalResKeyList[i];
                long tmpResFlag = tmpNewestDescDict[tmpKey];
                var tmpResInfo = ResourcesSystem.ResFlag2Info(tmpResFlag);
                tmpResInfo.ResPos = ResourcesSystem.EResPos.Persistent;

                tmpNewestDescDict[tmpKey] = ResourcesSystem.ResInfo2Flag(tmpResInfo);
                tmpResInfo.Dispose();
            }

            if (TotalSize > 1)
            {
                CheckFinishHandle?.Invoke(this);
            }
            else
            {
                Finish();
            }
        }

        private void Finish()
        {
            UpdaterMgr.ResInfoDict = mResInfoDict;
            FinishHandle?.Invoke(this);
        }

        private async void DownloadResListAsync()
        {
            BeginHandle?.Invoke(this);

            while (true)
            {
                if (mBeDownloadResKey.Count == 0)
                {
                    break;
                }

                uint tmpKey = mBeDownloadResKey.Dequeue();
                long tmpResFlag = 0;

                if (!mResInfoDict.TryGetValue(tmpKey, out tmpResFlag))
                {
                    Logger.LogError("");
                    break;
                }

                var tmpResInfo = ResourcesSystem.ResFlag2Info(tmpResFlag);

                while (true)
                {
                    try
                    {
                        using (UnityWebRequestAsync webReqAsync = Game.ObjectPool.Fetch<UnityWebRequestAsync>())
                        {
                            WebRequestAsync = webReqAsync;
                            string tmpUrl = URL + tmpResInfo.FileHashStr;
                            bool tmpResult = await webReqAsync.DownloadAsync(tmpUrl);

                            if (tmpResult)
                            {
                                Save(webReqAsync.Request.downloadHandler.data, tmpResInfo.FileHashStr);
                            }
                            else
                            {
                                Failed();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"download res error! {tmpResInfo.FileHashStr} -> {e}");
                        continue;
                    }

                    break;
                }
                
                mDownloadedSize += tmpResInfo.ResSize;
                DownloadProgressHandle?.Invoke(this);
                
                WebRequestAsync = null;
                tmpResInfo.Dispose();
            }
            
            Finish();
        }
    }
}
