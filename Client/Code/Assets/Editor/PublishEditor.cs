using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using AosBaseFramework;

namespace MyEditor
{
    public class PublishEditor : EditorWindow
    {
        private byte mMajor = 1;//主版本号
        private byte mMinor = 0;//子版本号
        private int mSvn = 0;//svn版本号
        
        private string mPatchPath = string.Empty;
        private EPlatformType mPlatform;

        [MenuItem("Tools/发布工具/完整打包（所有平台整包）", false)]
        static void ShowUIEditor()
        {
            PublishEditor window = GetWindow<PublishEditor>();
            window.Init();
            window.Show(true);
        }

        void Init()
        {
            titleContent = new GUIContent("完整打包");
            position = new Rect(30, 30, 500, 220);
            mPlatform = EPlatformType.PC;
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
            GUILayout.Label("补丁路径：", GUILayout.Width(60));
            mPatchPath = EditorGUILayout.TextField("", mPatchPath);
            if (GUILayout.Button("...", GUILayout.Width(40)))
                SelectPatchPath();
            GUILayout.EndHorizontal();

            GUILayout.Space(25);
            if (GUILayout.Button("开始打完整包"))
                DoPublish();
        }

        //选择补丁路径
        void SelectPatchPath()
        {
            mPatchPath = EditorUtility.OpenFolderPanel("选择补丁路径", "", "");
            mPatchPath += "/";
        }


        void DoPublish()
        {
            if (string.IsNullOrEmpty(mPatchPath))
            {
                ShowNotification(new GUIContent("补丁路径不能为空"));
                return;
            }

            if (!Directory.Exists(mPatchPath))
            {
                ShowNotification(new GUIContent("补丁路径不存在：" + mPatchPath));
                return;
            }

            string tmpRuntimeResFullPath = Path.GetFullPath(PathHelper.RUN_TIME_RES_PATH).Replace("\\", "/");
            string tmpBundlePath = $"{tmpRuntimeResFullPath}/{string.Format(PathHelper.BUNDLE_FOLDER, mPlatform)}/"; //源资源bundle路径
            string tmpTblFolder = PathHelper.GetBytesFileFoldNameByType(PathHelper.EBytesFileType.Table); //源资源tbl文件夹
            string tmpSkillActionFolder = PathHelper.GetBytesFileFoldNameByType(PathHelper.EBytesFileType.Action);//源资源SkillAction文件夹

            string tmpPublishResourcesPath = Application.dataPath.Replace("\\", "/") + "/Publish/Resources/";//发布存放.bytes的目录
            string tmpStreamingAssetsPath = Application.streamingAssetsPath.Replace("\\", "/") + "/";//发布存放bundle的路径
            string tmpPatchPath = mPatchPath + mPlatform + "/All_" + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");//当前补丁存放路径

            if (!Directory.Exists(tmpPatchPath))
                Directory.CreateDirectory(tmpPatchPath);

            if (!Directory.Exists(tmpPublishResourcesPath))
                Directory.CreateDirectory(tmpPublishResourcesPath);

            if (!Directory.Exists(tmpStreamingAssetsPath))
                Directory.CreateDirectory(tmpStreamingAssetsPath);

            PublishUtility.DeleteAllFile(tmpPublishResourcesPath);
            PublishUtility.DeleteAllFile(tmpStreamingAssetsPath);

            //把bundle文件对应平台文件copy一份到StreamingAssets
            List<ResouceElement> tmpStreamingAssetsList = PublishUtility.GetResouceElements(tmpBundlePath, ResouceElement.EResType.AssetBundle, string.Empty);
            PublishUtility.CopyRes2Path(tmpStreamingAssetsList, tmpStreamingAssetsPath, string.Empty);
            PublishUtility.CopyRes2Path(tmpStreamingAssetsList, tmpPatchPath, string.Empty);

            //处理tbl
            List<ResouceElement> tmpTblList = PublishUtility.GetResouceElements($"{tmpRuntimeResFullPath}/{tmpTblFolder}", ResouceElement.EResType.Bytes, tmpTblFolder);
            PublishUtility.CopyRes2Path(tmpTblList, tmpPublishResourcesPath, ".bytes");
            PublishUtility.CopyRes2Path(tmpTblList, tmpPatchPath, string.Empty);

            //游戏动作
            List<ResouceElement> tmpActionList = PublishUtility.GetResouceElements($"{tmpRuntimeResFullPath}/{tmpSkillActionFolder}", ResouceElement.EResType.Bytes, tmpSkillActionFolder);
            PublishUtility.CopyRes2Path(tmpActionList, tmpPublishResourcesPath, ".bytes");
            PublishUtility.CopyRes2Path(tmpActionList, tmpPatchPath, string.Empty);



//#if UNITY_EDITOR
//            Process.Start("Explorer.exe", tmpPatchPath.Replace("/", "\\"));
//#endif

            List<ResouceElement> tmpAllResList = new List<ResouceElement>();
            tmpAllResList.AddRange(tmpStreamingAssetsList);
            tmpAllResList.AddRange(tmpTblList);
            tmpAllResList.AddRange(tmpActionList);

            uint tmpVersionFileCrc = FileHelper.ResVersionFileCRC;
            uint tmpResDescFileCrc = 0;
            PublishUtility.GenResDescAndVersionFile(tmpPatchPath, tmpAllResList, FileHelper.RES_DESC_FILE, out tmpResDescFileCrc, 
                tmpVersionFileCrc, mMajor, mMinor, mSvn, tmpPublishResourcesPath);
            PublishUtility.GenAssistDescFile(tmpAllResList, tmpPatchPath, $"{mMajor}.{mMinor}.{mSvn}", tmpVersionFileCrc.ToString(), tmpResDescFileCrc.ToString());

            ShowNotification(new GUIContent("打包成功"));

            AssetDatabase.Refresh();
        }
    }
}

