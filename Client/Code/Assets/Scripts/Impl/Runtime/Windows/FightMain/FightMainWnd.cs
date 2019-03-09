using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;
using UnityEngine.EventSystems;

namespace AosHotfixRunTime
{
    public class FightMainWnd : WindowBase
    {
        public override string BundleName { get { return "FightMainWnd"; } }

        protected override bool IsLoadAsync => false;

        const int SKILL_NUM = 6;

        private GameObject mLT;
        private GameObject mRoleInfo;
        private Image mRoleIconImg;
        private ImageLoader mRoleIconLoader;
        private Text mRoleNameLab;
        private Image mRoleHpImg;
        private Image mRoleMpImg;
        private Image mRoleDpImg;

        private GameObject mRT;
        private GameObject mEnemyInfo;
        private Image mEnemyIconImg;
        private ImageLoader mEnemyIconLoader;
        private Image mEnemyHpImg;
        private Image mEnemyDpImg;

        private GameObject mRB;
        private GameObject mSkillInfo;
        private SkillItem[] mSkillItems = new SkillItem[SKILL_NUM];

        private GameObject mLB;
        private UGUIJoystick mJoystick;

        protected override void AfterInit()
        {
            base.AfterInit();

            mLT = Find("LT");
            mRoleInfo = Find(mLT, "RoleInfo");
            mRoleNameLab = Find<Text>(mRoleInfo, "Text_Name");
            mRoleIconImg = Find<Image>(mRoleInfo, "Image_Icon");
            mRoleIconLoader = ReferencePool.Fetch<ImageLoader>();
            mRoleHpImg = Find<Image>(mRoleInfo, "Image_Hp");
            mRoleMpImg = Find<Image>(mRoleInfo, "Image_Mp");
            mRoleDpImg = Find<Image>(mRoleInfo, "Image_Dp");

            mRT = Find("RT");
            mEnemyInfo = Find(mRT, "EnemyInfo");
            mEnemyIconImg = Find<Image>(mEnemyInfo, "Image_Icon");
            mEnemyIconLoader = ReferencePool.Fetch<ImageLoader>();
            mEnemyHpImg = Find<Image>(mEnemyInfo, "Image_Hp");
            mEnemyDpImg = Find<Image>(mEnemyInfo, "Image_Dp");

            mLB = Find("LB");
            var tmpJoystickGo = Find(mLB, "UGUIJoystick");
            mJoystick = new UGUIJoystick();
            mJoystick.Init(tmpJoystickGo, Find<Image>(tmpJoystickGo, "Background"), Find<Image>(tmpJoystickGo, "Center"));

            mRB = Find("RB");
            mSkillInfo = Find(mRB, "SkillInfo");

            for (int i = 0, max = SKILL_NUM; i < max; ++i)
            {
                GameObject tmpSkillGo = Find(mSkillInfo, $"Image_Skill{i}");
                SkillItem tmpSkillItem = new SkillItem();
                tmpSkillItem.Init(tmpSkillGo);
                mSkillItems[i] = tmpSkillItem;
            }

            RegisterEventClick(Find(mSkillInfo, "Image_Atk"), OnAttackBtnClick);
            RegisterEventClick(Find(mSkillInfo, "Image_Jump"), OnJumpBtnClick);
            RegisterEventClick(Find(mSkillInfo, "Image_Defense"), OnDefenseBtnClick);
        }

        protected override void AfterShow()
        {
            base.AfterShow();
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (null != mJoystick)
            {
                mJoystick.Update(deltaTime);
            }
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();
        }

        protected override void BeforeDestory()
        {
            base.BeforeDestory();
        }

        private void OnAttackBtnClick(PointerEventData arg)
        {
        }

        private void OnDefenseBtnClick(PointerEventData arg)
        {
        }

        private void OnJumpBtnClick(PointerEventData arg)
        {
        }
    }
}
