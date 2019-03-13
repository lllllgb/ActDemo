using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class HudPopupComponent : UnitComponentBase, UnitComponentBase.IInit0
    {
        List<HudPopupBase> mHudPopupList = new List<HudPopupBase>();

        public void Init()
        {
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = mHudPopupList.Count - 1; i >= 0; --i)
            {
                var tmpHudPopup = mHudPopupList[i];
                tmpHudPopup.Update(deltaTime);

                if (tmpHudPopup.IsInvalid)
                {
                    ObjectPoolBase tmpPool = GetPoolByType(tmpHudPopup.PopupType);

                    if (null != tmpPool)
                    {
                        tmpPool.Unspawn(tmpHudPopup);
                    }

                    mHudPopupList.RemoveAt(i);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = mHudPopupList.Count - 1; i >= 0; --i)
            {
                var tmpHudPopup = mHudPopupList[i];
                ObjectPoolBase tmpPool = GetPoolByType(tmpHudPopup.PopupType);

                if (null != tmpPool)
                {
                    tmpPool.Unspawn(tmpHudPopup);
                }
            }

            mHudPopupList.Clear();
        }

        public void Popup(EHudPopupType popupType)
        {
            HudPopupBase tmpHudPopup = GetPopupInstance(popupType);

            if (null == tmpHudPopup)
            {
                return;
            }

            if (tmpHudPopup is IPopupStart0)
            {
                (tmpHudPopup as IPopupStart0).StartPopup();
            }
        }

        public void Popup<A>(EHudPopupType popupType, A a)
        {
            HudPopupBase tmpHudPopup = GetPopupInstance(popupType);

            if (null == tmpHudPopup)
            {
                return;
            }

            if (tmpHudPopup is IPopupStart1<A>)
            {
                (tmpHudPopup as IPopupStart1<A>).StartPopup(a);
            }
        }

        public void Popup<A, B>(EHudPopupType popupType, A a, B b)
        {
            HudPopupBase tmpHudPopup = GetPopupInstance(popupType);

            if (null == tmpHudPopup)
            {
                return;
            }

            if (tmpHudPopup is IPopupStart2<A, B>)
            {
                (tmpHudPopup as IPopupStart2<A, B>).StartPopup(a, b);
            }
        }

        public void Popup<A, B, C>(EHudPopupType popupType, A a, B b, C c)
        {
            HudPopupBase tmpHudPopup = GetPopupInstance(popupType);

            if (null == tmpHudPopup)
            {
                return;
            }

            if (tmpHudPopup is IPopupStart3<A, B, C>)
            {
                (tmpHudPopup as IPopupStart3<A, B, C>).StartPopup(a, b, c);
            }
        }

        HudPopupBase GetPopupInstance(EHudPopupType popupType)
        {
            ObjectPoolBase tmpPool = GetPoolByType(popupType);

            if (null == tmpPool)
            {
                return null;
            }

            var tmpHudPopupBase = tmpPool.Spawn2() as HudPopupBase;

            if (null != tmpHudPopupBase)
            {
                tmpHudPopupBase.Initialize(Parent);
                mHudPopupList.Add(tmpHudPopupBase);
            }

            return tmpHudPopupBase;
        }

        ObjectPoolBase GetPoolByType(EHudPopupType popupType)
        {
            ObjectPoolBase tmpObjPool = null;

            switch (popupType)
            {
                case EHudPopupType.Damage:
                    tmpObjPool = Game.PoolMgr.GetObjectPool<HudPopupDamage>() as ObjectPoolBase;
                    break;
                default:

                    break;
            }

            return tmpObjPool;
        }
    }
}
