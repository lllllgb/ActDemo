using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class PreLoader
    {
        Dictionary<int, List<string>> mAbType2NameDict = new Dictionary<int, List<string>>();
        Action mLoadedHandle;
        int mNeedLoadResCount;
        int mLoadedResCount;
        bool mIsRelease = false;

        Dictionary<int, List<string>> mAbType2NameLoadedResDict = new Dictionary<int, List<string>>();

        public PreLoader()
        {
        }

        public void AddPreLoadRes(EABType type, string bundleName)
        {
            List<string> tmpBundleNameList = null;

            if (!mAbType2NameDict.TryGetValue((int)type, out tmpBundleNameList))
            {
                tmpBundleNameList = new List<string>();
                mAbType2NameDict.Add((int)type, tmpBundleNameList);
            }

            tmpBundleNameList.Add(bundleName);
        }

        public void StartLoad(Action loadedHandle)
        {
            mLoadedHandle = loadedHandle;
            mNeedLoadResCount = 0;
            mLoadedResCount = 0;
            mIsRelease = false;

            //先把需要加载的资源数统计 不然会出现有些资源是缓存的 导致立即成功 然后回调了
            foreach (var elem in mAbType2NameDict)
            {
                mNeedLoadResCount += elem.Value.Count;
            }

            foreach (var elem in mAbType2NameDict)
            {
                for (int i = 0, max = elem.Value.Count; i < max; ++i)
                {
                    LoadResAsync((EABType)elem.Key, elem.Value[i]);
                }
            }
        }

        public void Release()
        {
            var tmpResMgr = Game.ResourcesMgr;

            foreach (var elem in mAbType2NameDict)
            {
                elem.Value.Clear();
            }

            foreach (var elem in mAbType2NameLoadedResDict)
            {
                for (int i = 0, max = elem.Value.Count; i < max; ++i)
                {
                    tmpResMgr.UnLoadBundleByType((EABType)elem.Key, elem.Value[i]);
                }

                elem.Value.Clear();
            }

            mIsRelease = true;
            mLoadedHandle = null;
            mLoadedResCount = 0;
            mNeedLoadResCount = 0;
        }


        async void LoadResAsync(EABType abType, string bundleName)
        {
            var tmpResMgr = Game.ResourcesMgr;

            await tmpResMgr.LoadBundleByTypeAsync(abType, bundleName);

            if (mIsRelease)
            {
                tmpResMgr.UnLoadBundleByType(abType, bundleName);
                return;
            }

            //Logger.Log($"成功加载bundle -> {bundleName} 已成功 -> {mLoadedResCount} 需加载 -> {mNeedLoadResCount}");
            //已加载的bundle记录
            List<string> tmpBundleNameList = null;

            if (!mAbType2NameLoadedResDict.TryGetValue((int)abType, out tmpBundleNameList))
            {
                tmpBundleNameList = new List<string>();
                mAbType2NameLoadedResDict.Add((int)abType, tmpBundleNameList);
            }

            tmpBundleNameList.Add(bundleName);

            //
            ++mLoadedResCount;

            if (mLoadedResCount >= mNeedLoadResCount)
            {
                mLoadedHandle?.Invoke();
                mLoadedHandle = null;
            }
        }
    }
}
