using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AosBaseFramework
{
    public static class FileHelper
    {
        public const string RES_VERSION_FILE = "resVersion";
        public const string RES_DESC_FILE = "resDescribe";

        private static uint msResVersionFileCrc = Crc.Crc32(RES_VERSION_FILE);
        public static uint ResVersionFileCRC
        {
            get { return msResVersionFileCrc; }
        }
        
        public const string CODE_VERSION_FILE = "code_version.bytes";

        private static uint msCodeVersionFileCrc = Crc.Crc32(CODE_VERSION_FILE);
        public static uint CodeVersionFileCrc
        {
            get { return msCodeVersionFileCrc; }
        }

        public static byte[] ReadFile(string filename)
        {
            FileStream fileStream = File.OpenRead(filename);
            byte[] array = new byte[fileStream.Length];
            fileStream.Read(array, 0, array.Length);
            fileStream.Close();
            fileStream.Dispose();
            return array;
        }

        public static void WriteFile(string filename, byte[] data)
        {
            FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
            fileStream.Dispose();
        }

        public static byte[] LoadBytesFile(string fileName, PathHelper.EBytesFileType fileType = PathHelper.EBytesFileType.Table)
        {
            byte[] tmpData = null;
            string tmpFileFoldName = PathHelper.GetBytesFileFoldNameByType(fileType);
            fileName = $"{tmpFileFoldName}/{fileName}";

            if (GameSetup.instance.IsPublish)
            {
                var tmpResInfo = ResourcesSystem.FindResInfo(fileName);

                switch (tmpResInfo.ResPos)
                {
                    case ResourcesSystem.EResPos.Resource:
                        {
                            TextAsset tmpTextAsset = Resources.Load<TextAsset>(tmpResInfo.FileHashStr);

                            if (null != tmpTextAsset)
                            {
                                tmpData = tmpTextAsset.bytes;
                            }
                            else
                            {
                                Logger.LogError($"加载bytes文件 {tmpResInfo.FileHashStr} 失败.");
                            }

                            break;
                        }
                    case ResourcesSystem.EResPos.Persistent:
                        {
                            tmpData = ReadFile($"{PathHelper.AppHotfixResPath}/{tmpResInfo.FileHashStr}");
                            break;
                        }
                    default:
                        {
                            Logger.LogError("资源定位错误. bytes文件不可能出现在StreamAsset位置.");
                            break;
                        }
                }

                tmpResInfo.Dispose();
            }
            else
            {
                tmpData = ReadFile($"{PathHelper.RUN_TIME_RES_PATH}/{fileName}");
            }

            return tmpData;  
        }
    }
}

