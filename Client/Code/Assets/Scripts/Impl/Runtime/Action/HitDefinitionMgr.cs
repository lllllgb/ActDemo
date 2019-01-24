using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{

    public partial class HitDefinitionMgr
    {
        HitDefinitionPool mHitDefPool;
        List<HitDefinition> mHitDefList;

        public HitDefinitionMgr()
        {
            mHitDefPool = new HitDefinitionPool();
            mHitDefList = new List<HitDefinition>(64);
        }

        public void CreateHitDefinition(ActData.AttackDef data, IActUnit owner, string action, SkillItem skillItem)
        {
            HitDefinition tmpHitDef = mHitDefPool.Spawn();
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
            Queue<HitDefinition> mHitDefinitions = new Queue<HitDefinition>();

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
