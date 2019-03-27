using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyEditor
{
	public class BundleInfo
	{
		public List<string> ParentPaths = new List<string>();
	}

	public enum PlatformType
	{
		Android,
		IOS,
		PC,
	}

	public class BuildEditor : EditorWindow
	{
		private readonly Dictionary<string, BundleInfo> dictionary = new Dictionary<string, BundleInfo>();

		private PlatformType platformType = PlatformType.PC;
		private BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;

		[MenuItem("Tools/打包工具")]
		public static void ShowWindow()
		{
			GetWindow(typeof(BuildEditor));
		}

		private void OnGUI()
		{
			if (GUILayout.Button("标记"))
			{
				SetPackingTagAndAssetBundle();
			}

			this.platformType = (PlatformType)EditorGUILayout.EnumPopup(platformType);
			this.buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("BuildAssetBundleOptions(可多选): ", this.buildAssetBundleOptions);

			if (GUILayout.Button("开始打包"))
			{
				BuildHelper.Build(this.platformType, this.buildAssetBundleOptions);
			}
		}

		private void SetPackingTagAndAssetBundle()
		{
			ClearALLPackingTagAndAssetBundle();

            SetIndependentBundleAndAtlas("Assets/Bundles/Independent");
            
            this.dictionary.Clear();
            SetShareBundleAndAtlas("Assets/Bundles/Icon");
            SetShareBundleAndAtlas("Assets/Bundles/Audio");
            SetShareBundleAndAtlas("Assets/Bundles/Atlas");
            SetShareBundleAndAtlas("Assets/Bundles/Effect");
            SetShareBundleAndAtlas("Assets/Bundles/Misc");
            SetShareBundleAndAtlas("Assets/Bundles/Unit");
            SetShareBundleAndAtlas("Assets/Bundles/UI");
            SetShareBundleAndAtlas("Assets/Bundles/Scene");

            AssetDatabase.SaveAssets();
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
		}

		// 会将目录下的每个prefab引用的资源强制打成一个包，不分析共享资源
		private static void SetIndependentBundleAndAtlas(string dir)
		{
			List<string> paths = EditorResHelper.GetPrefabsAndScenes(dir);
			foreach (string path in paths)
			{
				string path1 = path.Replace('\\', '/');
				Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

				AssetImporter importer = AssetImporter.GetAtPath(path1);
				if (importer == null || go == null)
				{
					Debug.LogError("error: " + path1);
                    
					continue;
				}
				importer.assetBundleName = $"{Path.GetDirectoryName(path1)}/{go.name}.unity3d";

				List<string> pathes = CollectDependencies(path1);

				foreach (string pt in pathes)
				{
					string extension = Path.GetExtension(pt);
					if (extension == ".cs" || extension == ".dll")
					{
						continue;
					}
					if (pt.Contains("Resources"))
					{
						continue;
					}
					if (pt == path1)
					{
						continue;
					}

					SetBundleAndAtlas(pt, go.name);
				}
			}
		}

		private static List<string> CollectDependencies(string o)
		{
			string[] paths = AssetDatabase.GetDependencies(o);

			Debug.Log($"{o} dependecies: " + paths.ToList().ListToString());
            
			return paths.ToList();
		}

		// 目录下每个prefab打个包，分析共享资源，共享资源打个包
		private void SetShareBundleAndAtlas(string dir)
		{
			List<string> paths = EditorResHelper.GetAllResourcePath(dir, true);

			foreach (string path in paths)
			{
				string path1 = path.Replace('\\', '/');
				Object go = AssetDatabase.LoadAssetAtPath<Object>(path1);

				SetBundle(path1, go.name);

				List<string> pathes = CollectDependencies(path1);
				foreach (string pt in pathes)
				{
					string extension = Path.GetExtension(pt);
					if (extension == ".cs" || extension == ".dll")
					{
						continue;
					}
					//if (pt.Contains("Resources"))
					//{
					//	continue;
					//}
					if (pt == path1)
					{
						continue;
					}

					// 不存在则记录下来
					if (!this.dictionary.ContainsKey(pt))
					{
						Debug.Log($"{path1}----{pt}");
						BundleInfo bundleInfo = new BundleInfo();
						bundleInfo.ParentPaths.Add(path1);
						this.dictionary.Add(pt, bundleInfo);

						SetAtlas(pt, go.name);

						continue;
					}

					// 依赖的父亲不一样
					BundleInfo info = this.dictionary[pt];
					if (info.ParentPaths.Contains(path1))
					{
						continue;
					}
					info.ParentPaths.Add(path1);

					//DirectoryInfo dirInfo = new DirectoryInfo(dir);
					//string dirName = dirInfo.Name;

					SetBundleAndAtlas(pt, $"{Path.GetFileNameWithoutExtension(pt)}_share");
				}
			}
		}

		private static void ClearALLPackingTagAndAssetBundle()
		{
            ClearPackingTagAndAssetBundle("Assets/Bundles");
            ClearPackingTagAndAssetBundle("Assets/Res");
            ClearPackingTagAndAssetBundle("Assets/Shaders");
            ClearPackingTagAndAssetBundle("Assets/Scripts");
        }

        private static void ClearPackingTagAndAssetBundle(string path)
        {
            List<string> tmpResList = EditorResHelper.GetAllResourcePath(path, true);

            foreach (string pt in tmpResList)
            {
                if (Path.GetExtension(pt).Equals(".cs"))
                {
                    continue;
                }
                
                AssetImporter tmpImporter = AssetImporter.GetAtPath(pt);

                if (tmpImporter == null)
                {
                    continue;
                }

                tmpImporter.assetBundleName = "";

                if (tmpImporter is TextureImporter)
                {
                    (tmpImporter as TextureImporter).spritePackingTag = "";
                }
            }
        }


        private static void SetBundle(string path, string name)
		{
			AssetImporter importer = AssetImporter.GetAtPath(path);
			if (importer == null)
			{
				return;
			}

			//Debug.Log(path);
			string bundleName = "";
			if (name == "")
			{
				return;
			}
			if (importer.assetBundleName != "")
			{
				return;
			}
            
			bundleName = $"{Path.GetDirectoryName(path)}/{name}.unity3d";
			importer.assetBundleName = bundleName;
		}

		private static void SetAtlas(string path, string name)
		{
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			if (textureImporter == null)
			{
				return;
			}

			if (textureImporter.spritePackingTag != "")
			{
				return;
			}

			textureImporter.spritePackingTag = name;
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
		}

		private static void SetBundleAndAtlas(string path, string name)
		{
			AssetImporter importer = AssetImporter.GetAtPath(path);
			if (importer == null)
			{
				return;
			}

			//Debug.Log(path);
			string bundleName = "";
			if (name == "")
			{
				return;
			}
			if (importer.assetBundleName != "")
			{
				return;
			}

            if (importer is ShaderImporter)
            {
                bundleName = "assets/bundles/shaderCollect.unity3d";
            }
            else
            {
                bundleName = $"{Path.GetDirectoryName(path)}/{name}.unity3d";
            }

            importer.assetBundleName = bundleName;

			TextureImporter textureImporter = importer as TextureImporter;
			if (textureImporter == null)
			{
				return;
			}

			if (textureImporter.spritePackingTag != "")
			{
				return;
			}

			textureImporter.spritePackingTag = name;
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
		}
	}
}
