using System.IO;
using Model;
using UnityEditor;

namespace MyEditor
{
	public static class BuildHelper
	{
		public static string BuildFolder = "../Products/runtimeRes/{0}/StreamingAssets/";

		public static void Build(PlatformType type, BuildAssetBundleOptions buildAssetBundleOptions)
		{
			BuildTarget buildTarget = BuildTarget.StandaloneWindows;

			switch (type)
			{
				case PlatformType.PC:
					buildTarget = BuildTarget.StandaloneWindows;
					break;
				case PlatformType.Android:
					buildTarget = BuildTarget.Android;
					break;
				case PlatformType.IOS:
					buildTarget = BuildTarget.iOS;
					break;
			}

			string fold = string.Format(BuildFolder, type);
            if (Directory.Exists(fold))
            {
                Directory.Delete(fold, true);
            }
            Directory.CreateDirectory(fold);

            UnityEngine.Debug.Log("开始资源打包");
			BuildPipeline.BuildAssetBundles(fold, buildAssetBundleOptions, buildTarget);

			//GenerateVersionInfo(fold);
			UnityEngine.Debug.Log("完成资源打包");
		}

		private static void GenerateVersionInfo(string dir)
		{
			VersionConfig versionProto = new VersionConfig();
			GenerateVersionProto(dir, versionProto, "");

			//using (FileStream fileStream = new FileStream(x$"{dir}/Version.txt", FileMode.Create))
			//{
			//	byte[] bytes = MongoHelper.ToJson(versionProto).ToByteArray();
			//	fileStream.Write(bytes, 0, bytes.Length);
			//}
		}

		private static void GenerateVersionProto(string dir, VersionConfig versionProto, string relativePath)
		{
			foreach (string file in Directory.GetFiles(dir))
			{
				if (file.EndsWith(".manifest"))
				{
					continue;
				}
				string md5 = MD5Helper.FileMD5(file);
				FileInfo fi = new FileInfo(file);
				long size = fi.Length;
				string filePath = relativePath == "" ? fi.Name : $"{relativePath}/{fi.Name}";

				versionProto.FileVersionInfos.Add(new FileVersionInfo
				{
					File = filePath,
					MD5 = md5,
					Size = size,
				});
			}

			foreach (string directory in Directory.GetDirectories(dir))
			{
				DirectoryInfo dinfo = new DirectoryInfo(directory);
				string rel = relativePath == "" ? dinfo.Name : $"{relativePath}/{dinfo.Name}";
				GenerateVersionProto($"{dir}/{dinfo.Name}", versionProto, rel);
			}
		}
	}
}
