using System;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine;
using UnityEngine.UI;
using AosBaseFramework;
using UnityEngine.EventSystems;

namespace AosHotfixRunTime
{
    public abstract class HudInfoBase : PoAttachGoBase
    {
        const string NAME_NODE = "Text_Name";
        const string HP_BAR_NODE = "Slider_Hp";
        const float UPDATE_DISPLAY_INTERVAL = 0.1f;

        protected override EABType ResType => EABType.Misc;

        Unit mOwner;

        RectTransform mRectTrans;
        Text mNameText;
        Slider mHpBar;
        
        float mUpdateDisplayDelta = 0;

        private bool mVisible = true;
        public bool Visible
        {
            get { return mVisible; }
            set
            {
                mVisible = value;
                Utility.GameObj.SetActive(mGameObject, mVisible);
            }
        }

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
            
            mGameObject.transform.SetParent(CameraMgr.Instance.HudInfoCanvasRootGo.transform, false);
            Utility.GameObj.SetLayer(mGameObject, GameLayer.Layer_HudInfo);
            mRectTrans = mGameObject.GetComponent<RectTransform>();
            mNameText = Utility.GameObj.Find<Text>(mGameObject, NAME_NODE);
            mHpBar = Utility.GameObj.Find<Slider>(mGameObject, HP_BAR_NODE);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            mVisible = true;
            Utility.GameObj.SetActive(mGameObject, mVisible);
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

        public virtual void InitInfo(Unit owner)
        {
            mOwner = owner;
            mUpdateDisplayDelta = 0f;

            if (null != mNameText && !mOwner.Name.Equals(mNameText.text))
            {
                mNameText.text = mOwner.Name;
            }

            mHpBar.value = 1f;
        }

        public virtual void Update(float deltaTime)
        {
            AdjustPos();
            UpdateDisplay(deltaTime);
        }

        void UpdateDisplay(float deltaTime)
        {
            mUpdateDisplayDelta += deltaTime;
            if (mUpdateDisplayDelta < UPDATE_DISPLAY_INTERVAL)
            {
                return;
            }

            mUpdateDisplayDelta = 0f;
            float tmpCurrHp = mOwner.GetAttrib(EPA.CurHP);
            float tmpMaxHp = mOwner.GetAttrib(EPA.MaxHP);
            float tmpHpRatio = tmpCurrHp / Mathf.Max(1, tmpMaxHp);

            if (null != mHpBar && !Utility.CompareFloatValue(tmpHpRatio, mHpBar.value))
            {
                mHpBar.value = tmpHpRatio;
            }
        }

        void AdjustPos()
        {
            if (null == mGameObject || null == mOwner.TopNode)
                return;

            Vector3 tmpVec3 = RectTransformUtility.WorldToScreenPoint(CameraMgr.Instance.MainCamera, mOwner.TopNode.transform.position);

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mRectTrans, tmpVec3, CameraMgr.Instance.HudCamera, out tmpVec3))
            {
                mGameObject.transform.position = tmpVec3;
            }
        }
    }
}
