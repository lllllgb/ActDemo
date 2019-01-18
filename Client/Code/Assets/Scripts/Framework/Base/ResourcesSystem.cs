using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AosBaseFramework
{
    public static class ResourcesSystem
    {
        public enum EResType
        {
            Bytes,
            AssetBundle,
        }

        public enum EResPos
        {
            Resource,
            StreamAsset,
            Persistent,
        }

        public class ResInfo : Disposer
        {
            public uint FileHash;
            public string FileHashStr;
            public EResPos ResPos;
            public ushort ResSize;
        }

        // flag = |8|16 - size|8 - resPos|32 - fileHash|
        private static Dictionary<uint, long> mResKey2InfoFlagDict = new Dictionary<uint, long>();
        private static Dictionary<string, long> mResName2InfoFlagDict = new Dictionary<string, long>();

        public static void Init()
        {
            if (GameSetup.instance.IsPublish)
            {
                if (GameSetup.instance.IsResUpdate)
                {
                    mResKey2InfoFlagDict = UpdaterManager.Instance.ResInfoDict;
                }
                else
                {
                    LoadInnerRes();
                }
            }
        }

        private static void LoadInnerRes()
        {
            TextAsset tmpTextAsset = Resources.Load<TextAsset>(FileHelper.ResVersionFileCRC.ToString());

            if (null == tmpTextAsset)
            {
                Logger.LogError($"加载文件版本信息文件 {FileHelper.ResVersionFileCRC} 失败！");
                return;
            }

            VersionData tmpResVersionData = VersionData.LoadVersionData(tmpTextAsset.bytes);
            tmpTextAsset = Resources.Load<TextAsset>(tmpResVersionData.ResDescCrc.ToString());

            if (null == tmpTextAsset)
            {
                Logger.LogError($"加载文件列表信息文件 {tmpResVersionData.ResDescCrc} 失败！");
                return;
            }

            mResKey2InfoFlagDict = LoadResDescInfo(tmpTextAsset.bytes);
        }

        //查找资源信息
        public static ResInfo FindResInfo(string fileName)
        {
            long tmpInfoFlag = 0;

            if (!mResName2InfoFlagDict.TryGetValue(fileName, out tmpInfoFlag))
            {
                uint tmpResKey = Crc.Crc32(fileName);

                if (mResKey2InfoFlagDict.TryGetValue(tmpResKey, out tmpInfoFlag))
                {
                    mResName2InfoFlagDict.Add(fileName, tmpInfoFlag);
                }
                else
                {
                    Logger.LogError($"资源系统找不到资源 -> {fileName}");
                }
            }

            return ResFlag2Info(tmpInfoFlag);
        }

        //转换
        public static ResInfo ResFlag2Info(long resFlag)
        {
            ResInfo tmpResInfo = Game.ObjectPool.Fetch<ResInfo>();
            tmpResInfo.FileHash = (uint)resFlag;
            tmpResInfo.FileHashStr = tmpResInfo.FileHash.ToString();
            tmpResInfo.ResPos = (EResPos)((resFlag >> 32) & 0xff);
            tmpResInfo.ResSize = (ushort)((resFlag >> 40) & 0xffff);

            return tmpResInfo;
        }

        //转换
        public static long ResInfo2Flag(ResInfo resInfo)
        {
            return ResFlagBy(resInfo.ResSize, resInfo.ResPos, resInfo.FileHash);
        }

        //转换
        public static long ResFlagBy(ushort resSize, EResPos resPos, uint resHash)
        {
            return ((long)resSize) << 40 | ((long)resPos) << 32 | resHash;
        }

        //解析文件列表描述数据
        public static Dictionary<uint, long> LoadResDescInfo(byte[] data)
        {
            if (null == data)
            {
                Logger.LogError("读取文件列表描述数据为空");
                return null;
            }

            mResKey2InfoFlagDict.Clear();
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            int tmpFileCount = br.ReadInt32();
            Dictionary<uint, long> tmpDict = new Dictionary<uint, long>(tmpFileCount);

            for (int i = 0; i < tmpFileCount; i++)
            {
                uint tmpHashKey = br.ReadUInt32();
                uint tmpHashValue = br.ReadUInt32();
                byte tmpResType = br.ReadByte();
                ushort tmpResSize = br.ReadUInt16();

                EResPos tmpResPos = EResPos.Resource;

                if (tmpResType == (byte)EResType.AssetBundle)
                {
                    tmpResPos = EResPos.StreamAsset;
                }

                if (tmpDict.ContainsKey(tmpHashKey))
                {
                    Logger.LogError($"读取文件列表描述错误. 存在相同的 hashKey = {tmpHashKey}");
                    continue;
                }

                tmpDict.Add(tmpHashKey, ResFlagBy(tmpResSize, tmpResPos, tmpHashValue));
            }

            return tmpDict;
        }
    }
}
