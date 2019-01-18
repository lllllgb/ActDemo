using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class EffectPlayer
    {
        List<EffectObject> mEffectList = new List<EffectObject>();

        public EffectPlayer()
        {
        }

        public void Update(float deltaTime)
        {
            var tmpPool = Game.PoolMgr.GetObjectPool<EffectObject>() as ObjectPoolBase;

            for (int i = mEffectList.Count - 1; i >= 0; --i)
            {
                var tmpEffectObj = mEffectList[i];
                tmpEffectObj.Duration -= deltaTime;

                if (tmpEffectObj.Duration <= 0f)
                {
                    tmpPool.Unspawn(tmpEffectObj);
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

        public EffectObject PlayEffect(Transform parent, string effectName, float duration, float offsetX, float offsetY)
        {
            EffectObject tmpEffectObj = Game.PoolMgr.GetObjectPool<EffectObject>().Spawn();
            tmpEffectObj.Initialize(effectName);
            tmpEffectObj.Duration = duration;
            tmpEffectObj.EffectTrans.SetParent(parent, false);
            tmpEffectObj.EffectTrans.localPosition = new Vector3(offsetX, offsetY, 0f);
            tmpEffectObj.Play();

            mEffectList.Add(tmpEffectObj);

            return tmpEffectObj;
        }

        public void RemoveEffect(EffectObject effectObject)
        {
            if (mEffectList.Remove(effectObject))
            {
                var tmpPool = Game.PoolMgr.GetObjectPool<EffectObject>() as ObjectPoolBase;
                tmpPool.Unspawn(effectObject);
            }
        }
    }
}
