using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Crc = AosBaseFramework.Crc;
using UnityEngine;

namespace MyEditor
{
    public enum EPlatformType
    {
        Android,
        IOS,
        PC,
    }

    public class ResouceElement
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
            PersistentDataPath,
        }

        public string filePath;
        public string key;
        public uint keyHash;
        public uint fileHash;
        public EResType resType;
        public ushort fileSize;
    }

    public static class PublishUtility
    {

        //拷贝资源
        public static void CopyRes2Path(List<ResouceElement> resElemList, string path, string suffix)
        {
            for (int i = 0; i < resElemList.Count; i++)
            {
                ResouceElement tmpElem = resElemList[i];
                string tmpTargetPath = $"{path}/{tmpElem.fileHash}{suffix}";
                File.Copy(tmpElem.filePath, tmpTargetPath);
            }
        }

        //删除指定目录所有文件
        public static void DeleteAllFile(string path)
        {
            List<string> tmpFiles = GetFiles(path, "*.*");

            while (tmpFiles.Count > 0)
            {
                string file = tmpFiles[0];
                File.Delete(file);
                tmpFiles.RemoveAt(0);
            }
        }

        //获取路径下所有文件
        public static List<string> GetFiles(string path, string searchPattern, params string[] Ignore)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError($"找不到目录 {path}");
                return new List<string>();
            }

            string[] tmpFiles = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
            List<string> tmpFilesList = new List<string>(tmpFiles);

            if (Ignore != null)
            {
                for (int i = 0; i < Ignore.Length; i++)
                {
                    string tmpIgnoreStr = Ignore[i].Trim();

                    for (int j = tmpFilesList.Count - 1; j >= 0; j--)
                    {
                        tmpFilesList[j] = tmpFilesList[j].Replace("\\", "/");
                        string tmpFilePath = tmpFilesList[j];

                        if (tmpFilePath.Contains(tmpIgnoreStr))
                            tmpFilesList.RemoveAt(j);
                    }
                }
            }

            return tmpFilesList;
        }

        //构建资源描述
        public static List<ResouceElement> GetResouceElements(string path, ResouceElement.EResType resType, string prefix)
        {
            List<ResouceElement> tmpElemList = new List<ResouceElement>();
            List<string> tmpFiles = GetFiles(path, "*.*", new string[] { ".manifest" });

            for (int i = 0; i < tmpFiles.Count; i++)
            {
                ResouceElement tmpElem = new ResouceElement();
                tmpElem.filePath = tmpFiles[i].Replace("\\", "/");
                tmpElem.key = prefix + tmpElem.filePath.Replace(path, "");
                tmpElem.keyHash = Crc.Crc32(tmpElem.key);
                tmpElem.fileHash = Crc.Crc32(tmpElem.key + ComputeMD5(tmpElem.filePath));
                tmpElem.resType = resType;

                var tmpFileInfo = new FileInfo(tmpFiles[i]);
                tmpElem.fileSize = (ushort)(tmpFileInfo.Length / 1024f);
                tmpElem.fileSize = tmpElem.fileSize > 0 ? tmpElem.fileSize : (ushort)1;

                tmpElemList.Add(tmpElem);
            }

            return tmpElemList;
        }

        //生成资源描述及版本描述文件
        public static void GenResDescAndVersionFile(string patchPath, 
            List<ResouceElement> resList, string resDescFileName, out uint resDescFileCrc,
            uint versionFileCrc, byte major, byte minor, int svn,
            string copyPath = null)
        {
            GenResDescFile(resList, patchPath, resDescFileName, out resDescFileCrc);
            GenVersionFile(patchPath, versionFileCrc, major, minor, svn, resDescFileCrc);

            if (!string.IsNullOrEmpty(copyPath))
            {
                File.Copy($"{patchPath}/{resDescFileCrc}", $"{copyPath}{resDescFileCrc}.bytes");
                File.Copy($"{patchPath}/{versionFileCrc}", $"{copyPath}{versionFileCrc}.bytes");
            }
        }

        //生成资源描述文件
        public static void GenResDescFile(List<ResouceElement> resList, string patchPath, string resDescFileName, out uint resDescFileCrc)
        {
            string tmpFileListPath = patchPath + "/" + resDescFileName;
            FileStream fs = new FileStream(tmpFileListPath, FileMode.Create, FileAccess.Write);
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(resList.Count);//写入数量

            for (int i = 0; i < resList.Count; i++)
            {
                ResouceElement resElement = resList[i];
                bw.Write(resElement.keyHash);//写入文件KeyHash
                bw.Write(resElement.fileHash);//写入文件FileHash
                bw.Write((byte)resElement.resType);
                bw.Write(resElement.fileSize);
            }

            bw.Flush();
            ms.WriteTo(fs);
            bw.Close();
            ms.Close();
            fs.Close();
            
            resDescFileCrc = Crc.Crc32(resDescFileName + ComputeMD5(tmpFileListPath));
            File.Move(tmpFileListPath, $"{patchPath}/{resDescFileCrc}");
        }

        //生成版本描述文件
        public static void GenVersionFile(string patchPath, uint versionFileCrc, byte major, byte minor, int svn, uint resDescFileCrc)
        {
            string versionFile = patchPath + "/" + versionFileCrc;
            FileStream versionFs = new FileStream(versionFile, FileMode.Create, FileAccess.Write);
            BinaryWriter versionBw = new BinaryWriter(versionFs);
            versionBw.Write(major);
            versionBw.Write(minor);
            versionBw.Write(svn);
            versionBw.Write(resDescFileCrc);
            versionBw.Flush();
            versionBw.Close();
            versionFs.Close();
        }

        //生成帮助文件
        public static void GenAssistDescFile(List<ResouceElement> resList, string patchPath, string versionStr, 
            string versionFileName, string resListDescFileName)
        {
            string describeFileName = Path.GetFileName(patchPath) + ".txt";
            string describePath = patchPath + "/../" + describeFileName;

            if (File.Exists(describePath))
                File.Delete(describePath);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < resList.Count; i++)
            {
                ResouceElement resElement = resList[i];
                sb.AppendLine("KeyHash:" + resElement.keyHash + "\tFileHash:" + resElement.fileHash + "\tFileSize:" + (int)resElement.fileSize + "\tResType:" + (int)resElement.resType + "\tKey:" + resElement.key);
            }

            sb.AppendLine();
            sb.AppendLine($"VersionFileCRC: {versionFileName}");
            sb.AppendLine($"Version: {versionStr}\tFileListRCR:{resListDescFileName}");

            StreamWriter sw = new StreamWriter(describePath);
            sw.Write(sb.ToString());
            sw.Flush();
            sw.Close();
        }

        //计算MD5
        public static string ComputeMD5(string fileName)
        {
            string tmpHashMD5 = string.Empty;

            //检查文件是否存在，如果文件存在则进行计算，否则返回空值
            if (System.IO.File.Exists(fileName))
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    //计算文件的MD5值
                    System.Security.Cryptography.MD5 tmpCalculator = System.Security.Cryptography.MD5.Create();
                    byte[] tmpBuffer = tmpCalculator.ComputeHash(fs);
                    tmpCalculator.Clear();
                    //将字节数组转换成十六进制的字符串形式
                    StringBuilder tmpStrBuilder = new StringBuilder();

                    for (int i = 0; i < tmpBuffer.Length; i++)
                    {
                        tmpStrBuilder.Append(tmpBuffer[i].ToString("x2"));
                    }

                    tmpHashMD5 = tmpStrBuilder.ToString();
                }//关闭文件流
            }//结束计算

            return tmpHashMD5;
        }
    }
}
