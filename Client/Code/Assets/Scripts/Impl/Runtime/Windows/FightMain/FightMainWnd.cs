using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;
using UnityEngine.EventSystems;
using ActData.Helper;

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
        private SkillInput[] mSkillInputs = new SkillInput[SKILL_NUM];

        private GameObject mLB;
        private UGUIJoystick mJoystick;

        private bool mIsAtkBtnDown;
        private float mTryReleaseAtkTime;

        public FightMainWnd()
        {
            WindowType = EWindowType.Main;
        }

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
                SkillInput tmpSkillInput = new SkillInput();
                tmpSkillInput.InitUI(tmpSkillGo);
                mSkillInputs[i] = tmpSkillInput;
            }

            GameObject tmpAtkGo = Find(mSkillInfo, "Image_Atk");
            RegisterEventClickDown(tmpAtkGo, OnAtkBtnDown);
            RegisterEventClickUp(tmpAtkGo, OnAtkBtnUp);
            RegisterEventClick(Find(mSkillInfo, "Image_Jump"), OnJumpBtnClick);
            RegisterEventClick(Find(mSkillInfo, "Image_Defense"), OnDefenseBtnClick);
            RegisterEventClick(Find(mRB, "Button_SkillSystem"), OnSkillModuleBtnClick);
        }

        protected override void AfterShow()
        {
            base.AfterShow();

            RefreshPlayerInfo();
            RefreshMonsterInfo();
            InitSkill();

            Game.EventMgr.Subscribe(UnitCtrlEvent.CurrHitedMonsterChange.EventID, OnEventCurrHitedMonsterChange);
            Game.EventMgr.Subscribe(UnitEvent.HpModify.EventID, OnEventUnitHpModify);
            Game.EventMgr.Subscribe(UnitEvent.MpModify.EventID, OnEventUnitMpModify);
            Game.EventMgr.Subscribe(UnitEvent.DpModify.EventID, OnEventUnitDpModify);
            Game.EventMgr.Subscribe(SkillWndEvent.SkillSetChange.EventID, OnEventChangeSkillSet);
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (null != mJoystick)
            {
                mJoystick.Update(deltaTime);
            }

            if (mIsAtkBtnDown)
            {
                mTryReleaseAtkTime += deltaTime;

                if (mTryReleaseAtkTime > 0.1f)
                {
                    TryReleaseAction(ACT.EOperation.EO_Attack);
                }
            }

            for (int i = 0, max = mSkillInputs.Length; i < max; ++i)
            {
                mSkillInputs[i].Update(deltaTime);
            }
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();

            for (int i = 0, max = mSkillInputs.Length; i < max; ++i)
            {
                mSkillInputs[i].Reset();
            }

            Game.EventMgr.Unsubscribe(UnitCtrlEvent.CurrHitedMonsterChange.EventID, OnEventCurrHitedMonsterChange);
            Game.EventMgr.Unsubscribe(UnitEvent.HpModify.EventID, OnEventUnitHpModify);
            Game.EventMgr.Unsubscribe(UnitEvent.MpModify.EventID, OnEventUnitMpModify);
            Game.EventMgr.Unsubscribe(UnitEvent.DpModify.EventID, OnEventUnitDpModify);
            Game.EventMgr.Unsubscribe(SkillWndEvent.SkillSetChange.EventID, OnEventChangeSkillSet);
        }

        protected override void BeforeDestory()
        {
            base.BeforeDestory();

            for (int i = 0, max = mSkillInputs.Length; i < max; ++i)
            {
                mSkillInputs[i].Release();
            }
        }

        private void OnAtkBtnDown(PointerEventData arg)
        {
            mIsAtkBtnDown = true;
            mTryReleaseAtkTime = 1f;
        }

        private void OnAtkBtnUp(PointerEventData arg)
        {
            mIsAtkBtnDown = false;
        }

        private void OnDefenseBtnClick(PointerEventData arg)
        {
            
        }

        private void OnSkillModuleBtnClick(PointerEventData arg)
        {
            Game.WindowsMgr.ShowWindow<SkillWnd>();
        }

        private void OnJumpBtnClick(PointerEventData arg)
        {
            TryReleaseAction(ACT.EOperation.EO_Jump);
        }

        private void TryReleaseAction(ACT.EOperation operation)
        {
            LocalPlayer tmpLocalPlayer = Game.ControllerMgr.Get<UnitController>().LocalPlayer;
            var tmpInterruptIdx = tmpLocalPlayer.ActStatus.ActiveAction.GetActionInterruptIdx(ACT.EOperation.EO_Attack);

            if (-1 != tmpInterruptIdx)
            {
                tmpLocalPlayer.LinkSkill(null, tmpInterruptIdx);
            }
        }

        private void RefreshPlayerInfo()
        {
            Unit tmpLocalPlayer = Game.ControllerMgr.Get<UnitController>().LocalPlayer;
            mRoleNameLab.text = tmpLocalPlayer.Name;

            UnitBase tmpUnitBase = UnitBaseManager.instance.Find(tmpLocalPlayer.UnitID);
            if (null != tmpUnitBase)
            {
                mRoleIconLoader.Load(ImageLoader.EIconType.Unit, tmpUnitBase.Icon, mRoleIconImg, null, false);
            }

            RefreshRoleHP(tmpLocalPlayer);
            RefreshRoleMP(tmpLocalPlayer);
            RefreshRoleDP(tmpLocalPlayer);
        }

        private void RefreshRoleHP(Unit role)
        {
            mRoleHpImg.fillAmount = role.GetAttr(EPA.CurHP) / Mathf.Max(1f, role.GetAttr(EPA.MaxHP));
        }

        private void RefreshRoleMP(Unit role)
        {
            mRoleMpImg.fillAmount = role.GetAttr(EPA.CurMP) / Mathf.Max(1f, role.GetAttr(EPA.MaxMP));
        }

        private void RefreshRoleDP(Unit role)
        {
            mRoleDpImg.fillAmount = role.GetAttr(EPA.CurDP) / Mathf.Max(1f, role.GetAttr(EPA.MaxDP));
        }

        private void RefreshMonsterInfo()
        {
            Unit tmpEenemy = Game.ControllerMgr.Get<UnitController>().CurrHitedMonster;

            if (null == tmpEenemy)
            {
                SetActive(mEnemyInfo, false);
            }
            else
            {
                SetActive(mEnemyInfo, true);

                UnitBase tmpUnitBase = UnitBaseManager.instance.Find(tmpEenemy.UnitID);
                if (null != tmpUnitBase)
                {
                    mEnemyIconLoader.Load(ImageLoader.EIconType.Unit, tmpUnitBase.Icon, mEnemyIconImg, null, false);
                }

                RefreshEnemyHP(tmpEenemy);
                RefreshEnemyDP(tmpEenemy);
            }
            
        }

        private void RefreshEnemyHP(Unit enemy)
        {
            mEnemyHpImg.fillAmount = enemy.GetAttr(EPA.CurHP) / Mathf.Max(1f, enemy.GetAttr(EPA.MaxHP));
        }

        private void RefreshEnemyDP(Unit enemy)
        {
            mEnemyDpImg.fillAmount = enemy.GetAttr(EPA.CurDP) / Mathf.Max(1f, enemy.GetAttr(EPA.MaxDP));
        }

        //初始化技能
        private void InitSkill()
        {
            var tmpPlayerCtrl = Game.ControllerMgr.Get<PlayerController>();

            for (int i = 0, max = mSkillInputs.Length; i < max; ++i)
            {
                mSkillInputs[i].Init(tmpPlayerCtrl.GetSkillLink(i));
            }
        }

        private void OnEventCurrHitedMonsterChange(object sender, GameEventArgs arg)
        {
            RefreshMonsterInfo();
        }

        private void OnEventUnitHpModify(object sender, GameEventArgs arg)
        {
            var tmpHpModifyEvent = arg as UnitEvent.HpModify;
            var tmpUnitCtrl = Game.ControllerMgr.Get<UnitController>();
            var tmpLocalPlayer = tmpUnitCtrl.LocalPlayer;
            var tmpCurrEnemy = tmpUnitCtrl.CurrHitedMonster;

            if (tmpHpModifyEvent.Data == tmpLocalPlayer)
            {
                RefreshRoleHP(tmpLocalPlayer);
            }
            else if (tmpHpModifyEvent.Data == tmpCurrEnemy)
            {
                RefreshEnemyHP(tmpCurrEnemy);
            }
        }

        private void OnEventUnitMpModify(object sender, GameEventArgs arg)
        {
            var tmpHpModifyEvent = arg as UnitEvent.MpModify;
            var tmpUnitCtrl = Game.ControllerMgr.Get<UnitController>();
            var tmpLocalPlayer = tmpUnitCtrl.LocalPlayer;

            if (tmpHpModifyEvent.Data == tmpLocalPlayer)
            {
                RefreshRoleMP(tmpLocalPlayer);
            }
        }

        private void OnEventUnitDpModify(object sender, GameEventArgs arg)
        {
            var tmpHpModifyEvent = arg as UnitEvent.DpModify;
            var tmpUnitCtrl = Game.ControllerMgr.Get<UnitController>();
            var tmpLocalPlayer = tmpUnitCtrl.LocalPlayer;
            var tmpCurrEnemy = tmpUnitCtrl.CurrHitedMonster;

            if (tmpHpModifyEvent.Data == tmpLocalPlayer)
            {
                RefreshRoleDP(tmpLocalPlayer);
            }
            else if (tmpHpModifyEvent.Data == tmpCurrEnemy)
            {
                RefreshEnemyDP(tmpCurrEnemy);
            }
        }

        private void OnEventChangeSkillSet(object sender, GameEventArgs arg)
        {
            InitSkill();
        }
    }
}
