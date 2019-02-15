using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class EffectManager
    {
        List<EffectObject> mEffectList = new List<EffectObject>();

        public EffectManager()
        {
        }

        public void Update(float deltaTime)
        {
            for (int i = mEffectList.Count - 1; i >= 0; --i)
            {
                var tmpEffectObj = mEffectList[i];
                tmpEffectObj.Update(deltaTime);

                if (tmpEffectObj.IsInvalid)
                {
                    tmpEffectObj.Dispose();
                    mEffectList.RemoveAt(i);
                }
            }
        }

        public void Release()
        {
            var tmpPool = Game.PoolMgr.GetObjectPool<EffectObject>() as ObjectPoolBase;

            for (int i = mEffectList.Count - 1; i >= 0; --i)
            {
                tmpPool.Unspawn(mEffectList[i]);
            }

            mEffectList.Clear();
        }

        public EffectObject PlayEffect(string effectName, float duration, Transform parent, Vector3 pos, Quaternion rotate)
        {
            EffectObject tmpEffectObj = Game.PoolMgr.GetObjectPool<EffectObject>().Spawn();
            tmpEffectObj.Init(effectName, duration, parent, pos, rotate);
            tmpEffectObj.Play();
            mEffectList.Add(tmpEffectObj);

            return tmpEffectObj;
        }

        public void RemoveEffect(EffectObject effectObject)
        {
            if (mEffectList.Remove(effectObject))
            {
                effectObject.Dispose();
            }
        }
    }
}
