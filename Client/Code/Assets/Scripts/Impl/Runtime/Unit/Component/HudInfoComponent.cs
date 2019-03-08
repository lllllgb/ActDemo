using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class UnitHudInfoComponent : UnitComponentBase, UnitComponentBase.IInit0
    {
        HudInfoBase mHudInfo;

        public void Init()
        {
            CreateHudInfo(Parent.UnitType);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (null != mHudInfo)
            {
                mHudInfo.Update(deltaTime);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            ObjectPoolBase tmpObjPool = GetPoolByType(Parent.UnitType);

            if (null != tmpObjPool)
            {
                tmpObjPool.Unspawn(mHudInfo);
            }

            mHudInfo = null;
        }

        HudInfoBase CreateHudInfo(EUnitType unitType)
        {
            ObjectPoolBase tmpObjPool = GetPoolByType(unitType);

            return null == tmpObjPool ? null : tmpObjPool.Spawn2() as HudInfoBase;
        }

        ObjectPoolBase GetPoolByType(EUnitType unitType)
        {
            ObjectPoolBase tmpObjPool = null;

            switch (unitType)
            {
                case EUnitType.EUT_LocalPlayer:
                    tmpObjPool = Game.PoolMgr.GetObjectPool<HudInfoPlayer>() as ObjectPoolBase;
                    break;
                default:
                    tmpObjPool = Game.PoolMgr.GetObjectPool<HudInfoMonster>() as ObjectPoolBase;
                    break;
            }

            return tmpObjPool;
        }
    }
}
