using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosBaseFramework;
using UnityEngine.EventSystems;
using AosHotfixFramework;
using ActData.Helper;

namespace AosHotfixRunTime
{
    public class SkillInput : ACT.ISkillInput
    {
        Image mIconImg;
        ImageLoader mIconLoader;
        Image mCDImg;

        bool mIsInit;
        SkillItem mSkillItem;
        bool mIsSkillReady;
        float mCD = 0;
        int mActiveAcionCache = -1;
        int mInterruptIndex = -1;
        int mSkillActionCache = -1;
        LocalPlayer mLocalPlayerCache;

        public void InitUI(GameObject skillGo)
        {
            mIconImg = Utility.GameObj.Find<Image>(skillGo, "Image_Icon");
            mCDImg = Utility.GameObj.Find<Image>(skillGo, "Image_CD");
            UGUIEventListener.Get(skillGo).onClick = OnClickSkill;

            mIconLoader = ReferencePool.Fetch<ImageLoader>();
        }

        public void Init(SkillItem skillItem)
        {
            mSkillItem = skillItem;

            if (null != mSkillItem)
            {
                mIsInit = true;
                mSkillItem.SkillInput = this;
                mCD = 0f;
                mCDImg.fillAmount = 0f;
                mIconLoader.Load(ImageLoader.EIconType.Skill, mSkillItem.SkillBase.Icon, mIconImg, null, false);
                mLocalPlayerCache = Game.ControllerMgr.Get<UnitController>().LocalPlayer;
                mSkillActionCache = mLocalPlayerCache.ActStatus.ActionGroup.GetActionIdx(mSkillItem.SkillBase.Action);
                mIsSkillReady = false;
                
            }
        }

        public void Update(float deltaTime)
        {
            if (!mIsInit)
            {
                return;
            }

            bool tmpCDDone = UpdateCD(deltaTime);
            bool tmpSkillLinked = UpdateSkillLink(deltaTime);

            mIsSkillReady = tmpCDDone && tmpSkillLinked;
        }

        public void Reset()
        {
            if (null != mSkillItem)
            {
                mSkillItem.SkillInput = null;
            }

            mSkillItem = null;
            mIsInit = false;
        }

        public void Release()
        {
            Reset();

            if (null != mIconLoader)
            {
                ReferencePool.Recycle(mIconLoader);
                mIconLoader = null;
            }
        }

        private bool UpdateCD(float deltaTime)
        {
            bool tmpFlag = false;

            if (mCD <= 0)
            {
                tmpFlag = true;
            }
            else
            {
                mCD -= deltaTime;
                mCDImg.fillAmount = Mathf.Max(mCD * 1000f / mSkillItem.SkillAttrBase.CD, 0f);

                if (mCD <= 0f)
                {

                }
            }

            return tmpFlag;
        }

        private bool UpdateSkillLink(float deltaTime)
        {
            bool tmpFlag = false;
            var tmpActiveAction = mLocalPlayerCache.ActStatus.ActiveAction;

            if (tmpActiveAction.ActionCache != mActiveAcionCache)
            {
                mInterruptIndex = -1;
                mActiveAcionCache = tmpActiveAction.ActionCache;

                for (int i = 0, max = tmpActiveAction.ActionInterrupts.Count; i < max; ++i)
                {
                    var tmpInterrupt = tmpActiveAction.ActionInterrupts[i];

                    if (tmpInterrupt.ActionCache == mSkillActionCache)
                    {
                        mInterruptIndex = i;
                        break;
                    }
                }
            }

            if (mInterruptIndex >= 0 && mLocalPlayerCache.ActStatus.GetInterruptEnabled(mInterruptIndex))
            {
                tmpFlag = true;
            }

            return tmpFlag;
        }

        private void OnClickSkill(PointerEventData arg)
        {
            if (!mIsInit || !mIsSkillReady)
            {
                return;
            }

            var tmpLocalPlayer = Game.ControllerMgr.Get<UnitController>().LocalPlayer;
            tmpLocalPlayer.LinkSkill(this, mInterruptIndex);
        }

        public void PlaySkill()
        {
            mCD = mSkillItem.SkillAttrBase.CD * 0.001f;
            mLocalPlayerCache.PlaySkill(mSkillItem, mSkillItem.SkillBase.Action);
        }

        public void OnHitTarget(ACT.IActUnit target)
        {
        }

        public void OnHit(ACT.IActUnit target)
        {
        }

        public void OnHurt(ACT.IActUnit target)
        {
        }
    }
}
