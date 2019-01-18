using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using ResMgrBase = AosBaseFramework.ResourcesManager;

namespace AosHotfixFramework
{
    public enum EABType
    {
        Invalid,
        Unit,
        Scene,
        Effect,
        Atlas,
        UI,
        Skin,
        IconBuff,
        IconUnit,
        IconItem,
        IconSkill,
        Audio,
        Misc,
        Maze,
        UnitSpineInfo,
        Action,
        House,
        Farm,
        Shader,

        Max,
    }

    internal sealed class ResourcesManager : GameModuleBase, IResourcesManager
    {

        private ResourcesPool mResourcesPool;
        public ResourcesPool ResPool { get { return mResourcesPool; } }

        public ResourcesManager()
        {
            GameObject tmpGo = new GameObject("ResourcesPool");
            tmpGo.transform.position = new Vector3(100000f, 100000f);
            GameObject.DontDestroyOnLoad(tmpGo);
            mResourcesPool = new ResourcesPool(tmpGo, this);
        }

        internal override void Update(float deltaTime)
        {
        }

        internal override void LateUpdate(float deltaTime)
        {
        }
        
        internal override void Shutdown()
        {
        }

        public void LoadManifest()
        {
            ResMgrBase.Instance.LoadManifest();
        }

        public K GetAssetByType<K>(EABType abType, string bundleName) where K : class
        {
            return ResMgrBase.Instance.GetAsset<K>(GetBundleNameByType(abType, bundleName), bundleName);
        }

        public K GetAssetByType<K>(EABType abType, string bundleName, string assetName) where K : class
        {
            return ResMgrBase.Instance.GetAsset<K>(GetBundleNameByType(abType, bundleName), assetName);
        }
        
        public void UnLoadBundleByType(EABType type, string abName)
        {
            ResMgrBase.Instance.UnloadBundle(GetBundleNameByType(type, abName));
        }

        public void LoadBundleByType(EABType type, string abName)
        {
            ResMgrBase.Instance.LoadBundle(GetBundleNameByType(type, abName));
        }

        public async Task LoadBundleByTypeAsync(EABType abType, string abName)
        {
            await ResMgrBase.Instance.LoadBundleAsync(GetBundleNameByType(abType, abName));
        }


        public static string[] ABType2Path = new string[]
        {
            "{0}.unity3d",
            "assets/bundles/unit/{0}.unity3d",
            "assets/bundles/scene/{0}.unity3d",
            "assets/bundles/effect/{0}.unity3d",
            "assets/bundles/atlas/{0}.unity3d",
            "assets/bundles/ui/{0}.unity3d",
            "assets/bundles/skin/{0}.unity3d",
            "assets/bundles/icon/buff/{0}.unity3d",
            "assets/bundles/icon/unit/{0}.unity3d",
            "assets/bundles/icon/item/{0}.unity3d",
            "assets/bundles/icon/skill/{0}.unity3d",
            "assets/bundles/audio/{0}.unity3d",
            "assets/bundles/misc/{0}.unity3d",
            "assets/bundles/maze/{0}.unity3d",
            "assets/bundles/unitspineinfo/{0}.unity3d",
            "assets/bundles/action/{0}.unity3d",
            "assets/bundles/house/{0}.unity3d",
            "assets/bundles/farm/{0}.unity3d",
            "assets/bundles/{0}.unity3d",
        };

        public static string GetBundleNameByType(EABType type, string name)
        {
            return string.Format(ABType2Path[(int)type], name);
        }
    }
}
