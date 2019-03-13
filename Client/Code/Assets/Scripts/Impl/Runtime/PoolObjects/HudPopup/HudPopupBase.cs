using System.Collections;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine;
using UnityEngine.UI;
using AosBaseFramework;

namespace AosHotfixRunTime
{
    interface IPopupStart0
    {
        void StartPopup();
    }

    interface IPopupStart1<A>
    {
        void StartPopup(A a);
    }

    interface IPopupStart2<A, B>
    {
        void StartPopup(A a, B b);
    }

    interface IPopupStart3<A, B, C>
    {
        void StartPopup(A a, B b, C c);
    }

    public abstract class HudPopupBase : PoAttachGoBase
    {
        protected override EABType ResType => EABType.Misc;

        public virtual EHudPopupType PopupType { get; }

        protected override bool IsLoadResAsync => false;

        public override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnResLoaded()
        {
            base.OnResLoaded();

            if (null == mGameObject)
            {
                return;
            }
            
            mGameObject.transform.SetParent(CameraMgr.Instance.HudPopupCanvasRootGo.transform, false);
            Utility.GameObj.SetLayer(mGameObject, GameLayer.Layer_HudPopup);
            mRectTrans = mGameObject.GetComponent<RectTransform>();
            mPopupAnimation = Utility.GameObj.Find<Animation>(mGameObject, "Popup");
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        public override void OnUnspawn()
        {
            base.OnUnspawn();

            mOwner = null;
        }

        public override void OnRelease()
        {
            base.OnRelease();
        }


        //
        private Unit mOwner;
        protected Unit Owner { get { return mOwner; } }
        //
        private RectTransform mRectTrans;
        protected RectTransform RectTrans { get { return mRectTrans; } }
        //
        private Animation mPopupAnimation;
        protected Animation PopupAnimation { get { return mPopupAnimation; } }
        //
        public float Duration { get; protected set; }
        //
        public bool IsInvalid { get; protected set; }
        //
        private Vector3 mPopupPos = Vector3.zero;


        public void Initialize(Unit owner)
        {
            mOwner = owner;
            IsInvalid = false;
        }

        public virtual void Update(float deltaTime)
        {
            AdjustPos();

            Duration -= deltaTime;

            if (Duration <= 0f)
            {
                IsInvalid = true;
            }
        }

        protected void SetPopupPos(Vector3 popupPos)
        {
            mPopupPos = popupPos;
        }

        void AdjustPos()
        {
            if (null == mGameObject)
                return;

            Vector3 tmpVec3 = RectTransformUtility.WorldToScreenPoint(CameraMgr.Instance.MainCamera, mPopupPos);

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mRectTrans, tmpVec3, CameraMgr.Instance.HudCamera, out tmpVec3))
            {
                mGameObject.transform.position = tmpVec3;
            }
        }
    }
}
