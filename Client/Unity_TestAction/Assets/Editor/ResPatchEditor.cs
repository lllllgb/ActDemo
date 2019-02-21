using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using AosBaseFramework;

namespace MyEditor
{
    public class ResPatchEditor : EditorWindow
    {
        private EPlatformType mPlatform;

        private byte mMajor = 1;//主版本号
        private byte mMinor = 0;//子版本号
        private int mSvn = 0;//svn版本号
        private string mLastPatchPath = string.Empty;
        private string mCurrPatchPath = string.Empty;

        [MenuItem("Tools/发布工具/补丁打包", false)]
        static void ShowUIEditor()
        {
            var window = GetWindow<ResPatchEditor>();
            window.Init();
            window.Show(true);
        }

        void Init()
        {
            titleContent = new GUIContent("补丁打包");
            position = new Rect(30, 30, 500, 220);
            mPlatform = EPlatformType.Android;
        }

        void OnGUI()
        {
            GUILayout.Space(20);
            //写下版本号
            GUILayout.BeginHorizontal();
            GUILayout.Label("主版本号：", GUILayout.Width(60));
            mMajor = (byte)EditorGUILayout.IntField("", mMajor, GUILayout.Width(50));
            GUILayout.Space(20);
            GUILayout.Label("子版本号：", GUILayout.Width(60));
            mMinor = (byte)EditorGUILayout.IntField("", mMinor, GUILayout.Width(50));
            GUILayout.Space(20);
            GUILayout.Label("svn版本号：", GUILayout.Width(60));
            mSvn = EditorGUILayout.IntField("", mSvn, GUILayout.Width(80));

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("打包平台：", GUILayout.Width(60));
            mPlatform = (EPlatformType)EditorGUILayout.EnumPopup("", mPlatform, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("最后一次补丁路径：", GUILayout.Width(60));
            mLastPatchPath = EditorGUILayout.TextField("", mLastPatchPath);

            if (GUILayout.Button("...", GUILayout.Width(40)))
            {
                mLastPatchPath = SelectPatchPath();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("补丁路径：", GUILayout.Width(60));
            mCurrPatchPath = EditorGUILayout.TextField("", mCurrPatchPath);

            if (GUILayout.Button("...", GUILayout.Width(40)))
            {
                mCurrPatchPath = SelectPatchPath();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(25);
            if (GUILayout.Button("开始打资源补丁"))
                DoPublish();
        }

        string SelectPatchPath()
        {
            string tmpSelectPath = EditorUtility.OpenFolderPanel("选择补丁路径", "", "");
            tmpSelectPath += "/";

            return tmpSelectPath;
        }

        void DoPublish()
        {
            if (string.IsNullOrEmpty(mLastPatchPath) || !Directory.Exists(mLastPatchPath) ||
                string.IsNullOrEmpty(mCurrPatchPath) || !Directory.Exists(mCurrPatchPath))
            {
                ShowNotification(new GUIContent("错误：路径为空或不存在此路径"));
                return;
            }

            string tmpRuntimeResFullPath = Path.GetFullPath(PathHelper.RUN_TIME_RES_PATH).Replace("\\", "/");
            string tmpBundlePath = $"{tmpRuntimeResFullPath}/{string.Format(PathHelper.BUNDLE_FOLDER, mPlatform)}/"; //源资源bundle路径
            string tmpTblFolder = PathHelper.GetBytesFileFoldNameByType(PathHelper.EBytesFileType.Table); //源资源tbl文件夹
            string tmpSkillActionFolder = PathHelper.GetBytesFileFoldNameByType(PathHelper.EBytesFileType.Action);//源资源SkillAction文件夹

            string tmpPatchPath = mCurrPatchPath + mPlatform + "/Patch_" + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");//当前补丁存放路径

            if (!Directory.Exists(tmpPatchPath))
            {
                Directory.CreateDirectory(tmpPatchPath);
            }

            //把bundle文件对应平台文件copy一份到StreamingAssets
            List<ResouceElement> tmpStreamingAssetsList = PublishUtility.GetResouceElements(tmpBundlePath, ResouceElement.EResType.AssetBundle, string.Empty);

            //处理tbl
            List<ResouceElement> tmpTblList = PublishUtility.GetResouceElements($"{tmpRuntimeResFullPath}/{tmpTblFolder}", ResouceElement.EResType.Bytes, tmpTblFolder);

            //游戏动作
            List<ResouceElement> tmpActionList = PublishUtility.GetResouceElements($"{tmpRuntimeResFullPath}/{tmpSkillActionFolder}", ResouceElement.EResType.Bytes, tmpSkillActionFolder);

            List<ResouceElement> tmpAllResList = new List<ResouceElement>();
            tmpAllResList.AddRange(tmpStreamingAssetsList);
            tmpAllResList.AddRange(tmpTblList);
            tmpAllResList.AddRange(tmpActionList);

            uint tmpVersionFileCrc = FileHelper.ResVersionFileCRC;
            uint tmpResDescFileCrc = 0;
            PublishUtility.GenResDescAndVersionFile(tmpPatchPath, tmpAllResList, FileHelper.RES_DESC_FILE, out tmpResDescFileCrc,
                tmpVersionFileCrc, mMajor, mMinor, mSvn);

            //差异比较
            string tmpPath = $"{mLastPatchPath}/{FileHelper.ResVersionFileCRC}";
            if (!File.Exists(tmpPath))
            {
                ShowNotification(new GUIContent($"错误：找不到最后一次补丁的版本描述文件 {FileHelper.ResVersionFileCRC}"));
                return;
            }

            byte[] tmpData = FileHelper.ReadFile(tmpPath);
            VersionData tmpLastVersionData = VersionData.LoadVersionData(tmpData);

            tmpPath = $"{mLastPatchPath}/{tmpLastVersionData.ResDescCrc}";
            if (!File.Exists(tmpPath))
            {
                ShowNotification(new GUIContent($"错误：找不到最后一次补丁的资源描述文件 {tmpLastVersionData.ResDescCrc}"));
                return;
            }

            tmpData = FileHelper.ReadFile(tmpPath);
            Dictionary<uint, long> tmpLastResDescInfo = ResourcesSystem.LoadResDescInfo(tmpData);
            List<ResouceElement> tmpDiffList = new List<ResouceElement>();

            for (int i = 0, max = tmpAllResList.Count; i < max; ++i)
            {
                ResouceElement tmpResElem = tmpAllResList[i];
                long tmpResFlag = 0;

                if (tmpLastResDescInfo.TryGetValue(tmpResElem.keyHash, out tmpResFlag))
                {
                    var tmpResInfo = ResourcesSystem.ResFlag2Info(tmpResFlag);

                    if (tmpResElem.fileHash == tmpResInfo.FileHash)
                    {
                        continue;
                    }
                }

                tmpDiffList.Add(tmpResElem);
            }

            PublishUtility.GenAssistDescFile(tmpDiffList, tmpPatchPath, $"{mMajor}.{mMinor}.{mSvn}", tmpVersionFileCrc.ToString(), tmpResDescFileCrc.ToString());
            PublishUtility.CopyRes2Path(tmpDiffList, tmpPatchPath, string.Empty);

            ShowNotification(new GUIContent($"打包补丁成功 差异化文件 {tmpDiffList.Count} 个"));
        }
    }
}

