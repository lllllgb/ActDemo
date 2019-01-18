using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectPool = AosBaseFramework.GameObjectPool;

namespace AosHotfixFramework
{
    public class ResourcesPool
    {
        private GameObject mRootGo;
        private IResourcesManager mResMgr;
        private Dictionary<EABType, GameObjectPool> mABType2GoPoolDict = new Dictionary<EABType, GameObjectPool>();

        public ResourcesPool(GameObject rootGo, IResourcesManager resMgr)
        {
            mRootGo = rootGo;
            mResMgr = resMgr;
        }

        public GameObject Spawn(EABType resType, string goName)
        {
            GameObject tmpGo = null;
            GameObjectPool tmpGoPool = null;

            if (mABType2GoPoolDict.TryGetValue(resType, out tmpGoPool))
            {
                tmpGo = tmpGoPool.Spawn(goName);
            }

            return tmpGo;
        }

        public void Despawn(EABType resType, string goName, GameObject go)
        {
            GameObjectPool tmpGoPool = null;

            if (!mABType2GoPoolDict.TryGetValue(resType, out tmpGoPool))
            {
                GameObject tmpGo = new GameObject(resType.ToString());
                tmpGo.transform.SetParent(mRootGo.transform, false);
                tmpGoPool = new GameObjectPool(tmpGo);
                mABType2GoPoolDict.Add(resType, tmpGoPool);
            }

            tmpGoPool.Despawn(goName, go);
        }

        public void ClearAll()
        {
            for (int i = (int)EABType.Invalid, max = (int)EABType.Max; i < max; ++i)
            {
                ClearByType((EABType)i);
            }

            GameObject.Destroy(mRootGo);
        }

        public void ClearByTypeFlag(int typeFlag)
        {
            for (int i = (int)EABType.Invalid, max = (int)EABType.Max; i < max; ++i)
            {
                if (((1 << i) & typeFlag) == 0)
                {
                    continue;
                }

                ClearByType((EABType)i);
            }
        }

        public void ClearByType(EABType eABType)
        {
            GameObjectPool tmpGoPool = null;

            if (mABType2GoPoolDict.TryGetValue(eABType, out tmpGoPool))
            {
                tmpGoPool.Clear((name, count) =>
                {
                    for (int i = 0; i < count; ++i)
                    {
                        mResMgr.UnLoadBundleByType(eABType, name);
                    }
                });

                mABType2GoPoolDict.Remove(eABType);
            }

        }
    }
}

