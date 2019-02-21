using System;
using System.Collections.Generic;
using System.Linq;

namespace AosBaseFramework
{
    public static class ResourcesHelper
	{
        public static string[] GetDependencies(string assetBundleName)
		{
			string[] dependencies = ResourcesManager.AssetBundleManifestObject.GetAllDependencies(assetBundleName);

			return dependencies;
		}

		public static string[] GetSortedDependencies(string assetBundleName)
		{
			Dictionary<string, int> info = new Dictionary<string, int>();
			List<string> parents = new List<string>();
			CollectDependencies(parents, assetBundleName, info);
			info.Remove(assetBundleName);
			string[] ss = info.OrderByDescending(x => x.Value).Select(x => x.Key).ToArray();
			return ss;
		}

		public static void CollectDependencies(List<string> parents, string assetBundleName, Dictionary<string, int> info)
		{
			parents.Add(assetBundleName);
            //获取当前bundle依赖
			string[] deps = GetDependencies(assetBundleName);

			foreach (string parent in parents)
			{
				if (!info.ContainsKey(parent))
				{
					info[parent] = 0;
				}

				info[parent] += deps.Length;
			}


			foreach (string dep in deps)
			{
				if (parents.Contains(dep))
				{
					throw new Exception($"包有循环依赖，请重新标记: {assetBundleName} {dep}");
				}
				CollectDependencies(parents, dep, info);
			}
			parents.RemoveAt(parents.Count - 1);
		}
	}
}
