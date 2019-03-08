using System.Collections;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine;
using UnityEngine.UI;
using AosBaseFramework;

namespace AosHotfixRunTime
{
    interface IPopupStart
    {
        void StartPopup();
    }

    interface IPopupStart0
    {
        void StartPopup(int value);
    }

    interface IPopupStart1
    {
        void StartPopup(string value);
    }

    public abstract class HudPopupBase : PoAttachGoBase
    {
        public const EABType RES_TYPE = EABType.Misc;
        //public virtual EPopupType PopupType { get; }

        protected override EABType ResType => RES_TYPE;

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
        public float Duration { get; set; }


        public void Initialize(Unit owner)
        {
            mOwner = owner;
        }

        public virtual void Update(float deltaTime)
        {
            AdjustPos();
        }

        void AdjustPos()
        {
            //if (null == mGameObject || null == mOwner.HudTopNode)
            //    return;

            //Vector3 tmpVec3 = RectTransformUtility.WorldToScreenPoint(GameDefine.CurrentCamera, mOwner.HudTopNode.transform.position);

            //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mRectTrans, tmpVec3, CameraHelper.HudCamera, out tmpVec3))
            //{
            //    mGameObject.transform.position = tmpVec3;
            //}
        }
    }
}
