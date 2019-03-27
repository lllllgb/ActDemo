using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AosBaseFramework
{
	public class ABInfo : Disposer
    {
		private int refCount;
		public string Name { get; set; }

		public int RefCount
		{
			get
			{
				return this.refCount;
			}
			set
			{
				this.refCount = value;

                if (refCount < 0)
                {
                    Logger.LogError($"{this.Name} refcount: {value}");
                }
                else
                {
                    //Logger.Log($"{this.Name} refcount: {value}");
                }


            }
		}

		public AssetBundle AssetBundle { get; set; }

        public void Init(string name, AssetBundle ab)
        {
            this.Name = name;
            this.AssetBundle = ab;
            this.RefCount = 1;
            //Logger.Log($"加载 assetbundle: {this.Name}");
        }

        public override void Dispose()
		{
            //Logger.Log($"卸载 assetbundle: {this.Name}");
			this.AssetBundle?.Unload(true);
            
            base.Dispose();
        }
	}

    public class AssetsBundleLoaderAsync : Disposer
    {
        public string BundleName { get; set; }

        private AssetBundleCreateRequest mAbRequest;
        private TaskCompletionSource<AssetBundle> mTcsAb;
        public TaskCompletionSource<AssetBundle> TcsAssetsBundle { get { return mTcsAb; } }

        public bool Update()
        {
            if (!mAbRequest.isDone)
            {
                return false;
            }

            mTcsAb.SetResult(mAbRequest.assetBundle);

            return true;
        }

        public Task<AssetBundle> LoadAsync(string bundleName)
        {
            BundleName = bundleName;

            if (null == mAbRequest)
            {
                mAbRequest = AssetBundle.LoadFromFileAsync(PathHelper.BundleName2ResPath(bundleName));
            }

            if (null == mTcsAb)
            {
                mTcsAb = new TaskCompletionSource<AssetBundle>();
            }

            return mTcsAb.Task;
        }

        public override void Dispose()
        {
            mAbRequest = null;
            mTcsAb = null;

            base.Dispose();
        }
    }
    
    public class AssetsLoaderAsync : Disposer
    {
        private AssetBundle mAssetBundle;
        public AssetBundle AssetBundle { get { return mAssetBundle; } }

        public string AssetBundleName { get { return null == mAssetBundle ? string.Empty : mAssetBundle.name; } }

        private AssetBundleRequest mAbRequest;
        private TaskCompletionSource<bool> mTcsIsLoaded;
        public TaskCompletionSource<bool> TcsAssetLoaded { get { return mTcsIsLoaded; } }

        public void SetAssetBundle(AssetBundle ab)
        {
            this.mAssetBundle = ab;
        }

        public bool Update()
        {
            if (!this.mAbRequest.isDone)
            {
                return false;
            }

            mTcsIsLoaded.SetResult(true);

            return true;
        }

        public async Task<UnityEngine.Object[]> LoadAllAssetsAsync()
        {
            await InnerLoadAllAssetsAsync();

            return mAbRequest.allAssets;
        }

        private Task<bool> InnerLoadAllAssetsAsync()
        {
            if (null == mAbRequest)
            {
                mAbRequest = mAssetBundle.LoadAllAssetsAsync();
            }

            if (null == mTcsIsLoaded)
            {
                mTcsIsLoaded = new TaskCompletionSource<bool>();
            }

            return mTcsIsLoaded.Task;
        }

        public override void Dispose()
        {
            mAssetBundle = null;
            mAbRequest = null;
            mTcsIsLoaded = null;

            base.Dispose();
        }
    }

    internal sealed class ResourcesManager : Singleton<ResourcesManager>
    {
		public static AssetBundleManifest AssetBundleManifestObject { get; set; }

		private readonly Dictionary<string, UnityEngine.Object> mResourceCache = new Dictionary<string, UnityEngine.Object>();

		private readonly Dictionary<string, ABInfo> mLoadedBundles = new Dictionary<string, ABInfo>();

        private readonly Dictionary<string, ABInfo> mLoadingBundles = new Dictionary<string, ABInfo>();

        // lru缓存队列
        private readonly QueueDictionary<string, ABInfo> mLRUcacheDict = new QueueDictionary<string, ABInfo>();
        // 
        private readonly List<AssetsBundleLoaderAsync> mAssetsBundleLoaderAsynList = new List<AssetsBundleLoaderAsync>();
        //
        private readonly List<AssetsLoaderAsync> mAssetsLoaderAsynList = new List<AssetsLoaderAsync>();

        internal void Update(float deltaTime)
        {
            for (int i = mAssetsBundleLoaderAsynList.Count - 1; i >= 0; --i)
            {
                AssetsBundleLoaderAsync tmpAbLoader = mAssetsBundleLoaderAsynList[i];

                if (tmpAbLoader.Update())
                {
                    mAssetsBundleLoaderAsynList.RemoveAt(i);
                    tmpAbLoader.Dispose();
                }
            }

            for (int i = mAssetsLoaderAsynList.Count - 1; i >= 0; --i)
            {
                AssetsLoaderAsync tmpAssetLoader = mAssetsLoaderAsynList[i];

                if (tmpAssetLoader.Update())
                {
                    mAssetsLoaderAsynList.RemoveAt(i);
                    tmpAssetLoader.Dispose();
                }
            }
        }

        internal void LateUpdate(float deltaTime)
        {
        }

        internal void Shutdown()
        {
            foreach (var abInfo in this.mLoadedBundles)
            {
                abInfo.Value?.AssetBundle?.Unload(true);
            }

            this.mLoadedBundles.Clear();
            this.mLRUcacheDict.Clear();
            this.mResourceCache.Clear();
        }

        public void LoadManifest()
        {
            LoadOneBundle("StreamingAssets");
            AssetBundleManifestObject = GetAsset<AssetBundleManifest>("StreamingAssets", "AssetBundleManifest");
        }

        public K GetAsset<K>(string bundleName, string prefab) where K : class
		{
			string path = $"{bundleName}/{prefab}".ToLower();

			UnityEngine.Object resource = null;
			if (!this.mResourceCache.TryGetValue(path, out resource))
			{
				throw new Exception($"not found asset: {path}");
			}
			
			return resource as K;
		}


		public void UnloadBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.ToLower();
			
			this.UnloadOneBundle(assetBundleName);

			string[] dependencies = ResourcesHelper.GetSortedDependencies(assetBundleName);

			//Logger.Log($"-----------dep unload {assetBundleName} dep: {dependencies.ToList().ListToString()}");
			foreach (string dependency in dependencies)
			{
				this.UnloadOneBundle(dependency);
			}
		}

		private void UnloadOneBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.ToLower();
			
			//Logger.Log($"unload bundle {assetBundleName}");
			ABInfo abInfo;
			if (!this.mLoadedBundles.TryGetValue(assetBundleName, out abInfo))
			{
				throw new Exception($"not found assetBundle: {assetBundleName}");
			}

			--abInfo.RefCount;
			if (abInfo.RefCount > 0)
			{
				return;
			}
			
			this.mLoadedBundles.Remove(assetBundleName);
			
			// 缓存10个包
			this.mLRUcacheDict.Enqueue(assetBundleName, abInfo);
			if (this.mLRUcacheDict.Count > 10)
			{
				abInfo = this.mLRUcacheDict[this.mLRUcacheDict.FirstKey];
				this.mLRUcacheDict.Dequeue();
				abInfo.Dispose();
			}
			//Logger.Log($"cache count: {this.cacheDictionary.Count}");
		}
		
		/// <summary>
		/// 同步加载assetbundle
		/// </summary>
		/// <param name="assetBundleName"></param>
		/// <returns></returns>
		public void LoadBundle(string assetBundleName)
		{
			assetBundleName = assetBundleName.ToLower();

			string[] dependencies = ResourcesHelper.GetSortedDependencies(assetBundleName);

            //foreach (string dependency in dependencies)
            for(int i = dependencies.Length - 1; i >= 0; --i)
            {
                string dependency = dependencies[i];
                //Logger.Log($"-----------dep load {assetBundleName} dep: {dependency}");
                if (string.IsNullOrEmpty(dependency))
                {
                    continue;
                }
                this.LoadOneBundle(dependency);
            }

            this.LoadOneBundle(assetBundleName);
        }
		
		public void LoadOneBundle(string assetBundleName)
		{
			ABInfo abInfo;
			if (this.mLoadedBundles.TryGetValue(assetBundleName, out abInfo))
			{
				++abInfo.RefCount;
				return;
			}


			if (this.mLRUcacheDict.ContainsKey(assetBundleName))
			{
				abInfo = this.mLRUcacheDict[assetBundleName];
				++abInfo.RefCount;
				this.mLoadedBundles[assetBundleName] = abInfo;
				this.mLRUcacheDict.Remove(assetBundleName);
				return;
			}

			AssetBundle assetBundle = AssetBundle.LoadFromFile(PathHelper.BundleName2ResPath(assetBundleName));

			if (!assetBundle.isStreamedSceneAssetBundle)
			{
				// 异步load资源到内存cache住
				UnityEngine.Object[] assets = assetBundle.LoadAllAssets();
				foreach (UnityEngine.Object asset in assets)
				{
					string path = $"{assetBundleName}/{asset.name}".ToLower();
					this.mResourceCache[path] = asset;
				}
			}
			
            ABInfo tmpABInfo = Game.ObjectPool.Fetch<ABInfo>();
            tmpABInfo.Init(assetBundleName, assetBundle);
            this.mLoadedBundles[assetBundleName] = tmpABInfo;
        }

        /// <summary>
        /// 异步加载assetbundle
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public async Task LoadBundleAsync(string assetBundleName)
		{
			assetBundleName = assetBundleName.ToLower();

            string[] dependencies = ResourcesHelper.GetSortedDependencies(assetBundleName);

            //Logger.Log($"-----------dep load {assetBundleName} dep: {dependencies.ToList().ListToString()}");
            foreach (string dependency in dependencies)
            {
                if (string.IsNullOrEmpty(dependency))
                {
                    continue;
                }
                await this.LoadOneBundleAsync(dependency);
            }

            await this.LoadOneBundleAsync(assetBundleName);
		}

        public async Task LoadOneBundleAsync(string assetBundleName)
        {
            ABInfo abInfo;
            if (this.mLoadedBundles.TryGetValue(assetBundleName, out abInfo))
            {
                ++abInfo.RefCount;
                return;
            }

            if (this.mLRUcacheDict.ContainsKey(assetBundleName))
            {
                abInfo = this.mLRUcacheDict[assetBundleName];
                ++abInfo.RefCount;
                this.mLoadedBundles[assetBundleName] = abInfo;
                this.mLRUcacheDict.Remove(assetBundleName);
                return;
            }

            AssetsBundleLoaderAsync tmpAbLoader = null;
            AssetsLoaderAsync tmpAssetsLoader = null;
            AssetBundle tmpAssetBundle = null;
            
            if (mLoadingBundles.TryGetValue(assetBundleName, out abInfo))
            {
                ++abInfo.RefCount;
                tmpAbLoader = FindAssetsBundleLoader(assetBundleName);

                if (null != tmpAbLoader)
                {
                    await tmpAbLoader.TcsAssetsBundle.Task;
                }
                
                tmpAssetsLoader = FindAssetsLoader(assetBundleName);

                if (null != tmpAssetsLoader)
                {
                    await tmpAssetsLoader.TcsAssetLoaded.Task;
                }


                return;
            }

            abInfo = Game.ObjectPool.Fetch<ABInfo>();
            abInfo.Init(assetBundleName, null);
            mLoadingBundles[assetBundleName] = abInfo;

            tmpAbLoader = Game.ObjectPool.Fetch<AssetsBundleLoaderAsync>();
            mAssetsBundleLoaderAsynList.Add(tmpAbLoader);
            tmpAssetBundle = await tmpAbLoader.LoadAsync(assetBundleName);

            if (!tmpAssetBundle.isStreamedSceneAssetBundle)
            {
                // 异步load资源到内存cache住
                tmpAssetsLoader = Game.ObjectPool.Fetch<AssetsLoaderAsync>();
                tmpAssetsLoader.SetAssetBundle(tmpAssetBundle);
                mAssetsLoaderAsynList.Add(tmpAssetsLoader);

                UnityEngine.Object[] tmpAssets = await tmpAssetsLoader.LoadAllAssetsAsync();

                //Logger.Log($"------------AssetBundleName = {assetBundleName}---------------");
                foreach (UnityEngine.Object asset in tmpAssets)
                {
                    string path = $"{assetBundleName}/{asset.name}".ToLower();

                    //Logger.Log($"assetName = {asset.name}");
                    this.mResourceCache[path] = asset;
                }

                //Logger.Log($"--------------------------------------------------------------");
            }

            abInfo.AssetBundle = tmpAssetBundle;
            mLoadingBundles.Remove(assetBundleName);
            mLoadedBundles[assetBundleName] = abInfo;
        }

        AssetsBundleLoaderAsync FindAssetsBundleLoader(string assetBundleName)
        {
            AssetsBundleLoaderAsync tmpAbLoader = null;

            for (int i = 0, max = mAssetsBundleLoaderAsynList.Count; i < max; ++i)
            {
                if (mAssetsBundleLoaderAsynList[i].BundleName.Equals(assetBundleName))
                {
                    tmpAbLoader = mAssetsBundleLoaderAsynList[i];
                    break;
                }
            }

            return tmpAbLoader;
        }

        AssetsLoaderAsync FindAssetsLoader(string assetBundleName)
        {
            AssetsLoaderAsync tmpAssetsLoaderAsync = null;

            for (int i = 0, max = mAssetsLoaderAsynList.Count; i < max; ++i)
            {
                if (mAssetsLoaderAsynList[i].AssetBundleName.Equals(assetBundleName))
                {
                    tmpAssetsLoaderAsync = mAssetsLoaderAsynList[i];
                    break;
                }
            }

            return tmpAssetsLoaderAsync;
        }
    }
}