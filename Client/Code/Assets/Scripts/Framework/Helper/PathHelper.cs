
using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace AosBaseFramework
{
    public static class PathHelper
    {
        public enum EBytesFileType
        {
            Table,
            Action,
            Video,
        }

        public const string GAME_FOLDER = "GSYZ";
#if UNITY_EDITOR
        public const string RUN_TIME_RES_PATH = "../Products/runtimeRes";
#else
        public const string RUN_TIME_RES_PATH = "../runtimeRes";
#endif

        public const string BUNDLE_FOLDER = "{0}/StreamingAssets";

        //非发布AssetBundle路径
        private static string smNoPublicAssetBundlePath = string.Empty;
        //文件类型-文件夹名
        private static Dictionary<EBytesFileType, string> smByteFileType2FoldNameDict = new Dictionary<EBytesFileType, string>()
        {
            { EBytesFileType.Table, "table"},
            { EBytesFileType.Action, "action"},
            { EBytesFileType.Video, "video"},
        };

        private static string msPersistentPath = string.Empty;
        /// <summary>
        ///应用程序外部资源路径存放路径(热更新资源路径)
        /// </summary>
        public static string AppHotfixResPath { get { return msPersistentPath; } }

        private static string msStreamingAssetsPath = string.Empty;
        public static string AppStreamAssetsPath { get { return Application.streamingAssetsPath; } }


        public static void Initalize()
        {
            msStreamingAssetsPath = Application.streamingAssetsPath;

#if UNITY_EDITOR || UNITY_STANDALONE
            msPersistentPath = $"C:/{GAME_FOLDER}";

            if (!Directory.Exists(msPersistentPath))
            {
                Directory.CreateDirectory(msPersistentPath);
            }
#else
            msPersistentPath = $"{Application.persistentDataPath}/{Application.productName}";
#endif

            if (!GameSetup.instance.IsPublish)
            {
#if UNITY_ANDROID
                smNoPublicAssetBundlePath = $"{RUN_TIME_RES_PATH}/Android/StreamingAssets";
#elif UNITY_IOS
                smNoPublicAssetBundlePath = $"{RUN_TIME_RES_PATH}/IOS/StreamingAssets";
#else
                smNoPublicAssetBundlePath = $"{RUN_TIME_RES_PATH}/PC/StreamingAssets";
#endif
            }
        }

        //bundle文件资源路径
        public static string BundleName2ResPath(string bundleName)
        {
            string tmpResPath = AppHotfixResPath;

            if (GameSetup.instance.IsPublish)
            {
                var tmpResInfo = ResourcesSystem.FindResInfo(bundleName);

                switch (tmpResInfo.ResPos)
                {
                    case ResourcesSystem.EResPos.StreamAsset:
                        {
                            tmpResPath = $"{AppStreamAssetsPath}/{tmpResInfo.FileHashStr}";
                            break;
                        }
                    case ResourcesSystem.EResPos.Persistent:
                        {
                            tmpResPath = $"{AppHotfixResPath}/{tmpResInfo.FileHashStr}";
                            break;
                        }
                    default:
                        {
                            Logger.LogError($"资源定位错误. assetBundle不可能出现在resources位置. name = {bundleName}");
                            break;
                        }
                }

                tmpResInfo.Dispose();
            }
            else
            {
                tmpResPath = $"{smNoPublicAssetBundlePath}/{bundleName}";
            }

            return tmpResPath;
        }

        //获取文件夹名称
        public static string GetBytesFileFoldNameByType(EBytesFileType fileType)
        {
            string tmpFoldName = string.Empty;
            smByteFileType2FoldNameDict.TryGetValue(fileType, out tmpFoldName);

            return tmpFoldName;
        }
    }
}
