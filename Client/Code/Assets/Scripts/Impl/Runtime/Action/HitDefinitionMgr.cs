using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{

    public partial class HitDefinitionMgr
    {
        Transform mHitDefRoot;
        HitDefinitionPool mHitDefPool;
        List<HitDefinition> mHitDefList;

        public HitDefinitionMgr()
        {
            mHitDefRoot = new GameObject("HitDefinitionRoot").transform;
            GameObject.DontDestroyOnLoad(mHitDefRoot);
            mHitDefPool = new HitDefinitionPool();
            mHitDefList = new List<HitDefinition>(64);
        }

        public void CreateHitDefinition(ActData.AttackDef data, IActUnit owner, string action, ISkillItem skillItem)
        {
            HitDefinition tmpHitDef = mHitDefPool.Spawn();
            tmpHitDef.CacheTransform.SetParent(mHitDefRoot, false);
            tmpHitDef.Init(data, owner, action, skillItem, OnFinishHandle);
            mHitDefList.Add(tmpHitDef);
        }

        public void Update(float deltaTime)
        {
            for (int i = mHitDefList.Count - 1; i >= 0; --i)
            {
                mHitDefList[i].Update(deltaTime);
            }
        }

        public void Clear()
        {
            for (int i = mHitDefList.Count - 1; i >= 0; --i)
            {
                mHitDefList[i].Reset();
                mHitDefPool.DeSpawn(mHitDefList[i]);
            }

            mHitDefList.Clear();
        }

        public void Release()
        {
            Clear();
            mHitDefPool.Release();
        }

        private void OnFinishHandle(HitDefinition hitDefinition)
        {
            mHitDefList.Remove(hitDefinition);
            hitDefinition.Reset();
            mHitDefPool.DeSpawn(hitDefinition);
        }
    }

    public partial class HitDefinitionMgr
    {

        public class HitDefinitionPool
        {
            Transform mHitDefPoolRoot;
            Queue<HitDefinition> mHitDefinitions = new Queue<HitDefinition>();

            public HitDefinitionPool()
            {
                mHitDefPoolRoot = new GameObject("HitDefinitionPool").transform;
                GameObject.DontDestroyOnLoad(mHitDefPoolRoot);
                mHitDefPoolRoot.position = new Vector3(10000, 10000);
            }

            public HitDefinition Spawn()
            {
                HitDefinition tmpHitDef = null;

                if (mHitDefinitions.Count > 0)
                {
                    tmpHitDef = mHitDefinitions.Dequeue();
                }
                else
                {
                    tmpHitDef = new HitDefinition();
                }

                return tmpHitDef;
            }

            public void DeSpawn(HitDefinition hitDefinition)
            {
                hitDefinition.CacheTransform.SetParent(mHitDefPoolRoot, false);
                mHitDefinitions.Enqueue(hitDefinition);
            }

            public void Release()
            {
                while (mHitDefinitions.Count > 0)
                {
                    HitDefinition tmpHitDef = mHitDefinitions.Dequeue();
                    tmpHitDef.Release();
                }
            }
        }
    }
}
