using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{
    public class ActEffectMgr
    {
        List<IActEffect> mEffectList = new List<IActEffect>();

        public ActEffectMgr()
        {
        }

        public void PlayEffect(string name, float duration, Transform parent, Vector3 pos, Quaternion rotation)
        {
            //Debug.Log($"playeffect {name} -> {duration}");
            var tmpActEffectSpawn = ActionSystem.Instance.SpawnEffectDelegate;

            if (null == tmpActEffectSpawn)
            {
                Debug.Log("未设置孵化特效代理 ActionHelper.SpawnEffectDelegate");
                return;
            }

            IActEffect tmpActEffect = tmpActEffectSpawn();
            tmpActEffect.Init(name, duration, parent, pos, rotation);
            tmpActEffect.Play();
            mEffectList.Add(tmpActEffect);
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

        public void Clear()
        {
            foreach (var elem in mEffectList)
            {
                elem.Dispose();
            }

            mEffectList.Clear();
        }
    }
}
