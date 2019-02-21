using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace AosBaseFramework
{
    public class VersionData
    {
        public uint ResDescCrc;
        public bool initalize;//用于判断是否初始化了

        public byte major;//主版本号
        public byte minor;//子版本
        public int revision;//修正号

        public string version
        {
            get
            {
                return major + "." + minor + "." + revision;
            }
        }

        public static VersionData LoadVersionData(byte[] data)
        {
            VersionData returnData = new VersionData();

            if (data == null)
                return returnData;

            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            returnData.major = br.ReadByte();
            returnData.minor = br.ReadByte();
            returnData.revision = br.ReadInt32();
            returnData.ResDescCrc = br.ReadUInt32();
            returnData.initalize = true;

            br.Close();
            ms.Close();

            return returnData;
        }
    }
}
