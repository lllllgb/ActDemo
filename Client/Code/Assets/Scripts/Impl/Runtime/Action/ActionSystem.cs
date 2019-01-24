using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ACT
{
    public class ActionSystem
    {
        protected static readonly ActionSystem msInstance = new ActionSystem();
        public static ActionSystem Instance { get { return msInstance; } }
        

        //输入
        public InputBox GInputBox { get; set; }

        public static List<IActUnit> UnitList = new List<IActUnit>();

        public void LoopAllActUnits(Action<IActUnit> loopHandle)
        {
            for (int i = 0, max = UnitList.Count; i < max; ++i)
            {
                loopHandle?.Invoke(UnitList[i]);
            }
        }

        public IActUnit LocalPlayer { get; set; }

        //加载动作资源代理
        public Func<string, byte[]> LoadActionFileDelegate { get; set; }

        //孵化特效代理
        public Func<IActEffect> SpawnEffectDelegate { get; set; }

        //特效管理
        private ActEffectMgr msActEffectMgr = new ActEffectMgr();
        public ActEffectMgr EffectMgr { get { return msActEffectMgr; } }

        //攻击定义管理
        private HitDefinitionMgr mHitDefinitionMgr = new HitDefinitionMgr();
        public HitDefinitionMgr HitDefMgr { get { return mHitDefinitionMgr; } }


        protected ActionSystem()
        {
        }

        public void Init()
        {
        }

        public void Update(float deltaTime)
        {
            mHitDefinitionMgr.Update(deltaTime);
        }

        public void Release()
        {
        }
    }
}