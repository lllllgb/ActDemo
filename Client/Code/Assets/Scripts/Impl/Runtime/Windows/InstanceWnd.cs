using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;
using UnityEngine.EventSystems;

namespace AosHotfixRunTime
{
    public class InstanceWnd : WindowBase
    {
        public override string BundleName { get { return "InstanceWnd"; } }

        protected override bool IsLoadAsync => false;


        GameObject mDescPanel;
        Text mDescLab;

        static Dictionary<int, int> sFlag2IDDict = new Dictionary<int, int>()
        {
            {1 << 16 | 0, 101},
            {1 << 16 | 1, 102},
            {1 << 16 | 2, 103},
            {2 << 16 | 0, 104},
            {2 << 16 | 1, 104},
            {2 << 16 | 2, 104},
        };

        int mLevelFlag = 1;
        int mDiffFlag = 0;
        int mInstanceID = 0;

        protected override void AfterInit()
        {
            base.AfterInit();

            RegistLevelClick(Find("Button_Level"), 1);
            RegistLevelClick(Find("Button_Level2"), 2);
            mDescPanel = Find("DescPanelRoot");
            mDescLab = Find<Text>(mDescPanel, "Text_Desc");
            RegisterEventClick(Find(mDescPanel, "Button_Normal"), OnNormalBtnClick);
            RegisterEventClick(Find(mDescPanel, "Button_Hard"), OnDifficultyBtnClick);
            RegisterEventClick(Find(mDescPanel, "Button_Hell"), OnHellBtnClick);
            RegisterEventClick(Find(mDescPanel, "Button_Enter"), OnStartBtnClick);

            RegisterEventClick(Find("Button_Back"), OnBackBtnClick);

        }

        protected override void AfterShow()
        {
            base.AfterShow();

            SetActive(mDescPanel, false);
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();
        }

        protected override void BeforeDestory()
        {
            base.BeforeDestory();
        }

        private void RegistLevelClick(GameObject go, int flag)
        {
            UGUIEventListener.Get(go).onClick = arg => 
            {
                mLevelFlag = flag;
                mDiffFlag = 0;
                RefreshInstanceID();
                ShowDescPanel(true);
            };
        }

        private void ShowDescPanel(bool visible)
        {
            SetActive(mDescPanel, visible);

            if (!visible)
            {
                return;
            }

            RefreshDesc();
        }

        private void RefreshInstanceID()
        {
            sFlag2IDDict.TryGetValue(mLevelFlag << 16 | mDiffFlag, out mInstanceID);
        }

        private void RefreshDesc()
        {
            InstanceBase tmpInstanceBase = InstanceBaseManager.instance.Find(mInstanceID);

            if (null != tmpInstanceBase)
            {
                mDescLab.text = tmpInstanceBase.Desc;
            }
        }

        private void OnNormalBtnClick(PointerEventData arg)
        {
            mDiffFlag = 0;
            RefreshInstanceID();
            RefreshDesc();
        }

        private void OnDifficultyBtnClick(PointerEventData arg)
        {
            mDiffFlag = 1;
            RefreshInstanceID();
            RefreshDesc();
        }

        private void OnHellBtnClick(PointerEventData arg)
        {
            mDiffFlag = 2;
            RefreshInstanceID();
            RefreshDesc();
        }

        private void OnBackBtnClick(PointerEventData arg)
        {
            ShowDescPanel(false);
        }

        private void OnStartBtnClick(PointerEventData arg)
        {
            PVEGameBuilder.Instance.InstanceID = mInstanceID;

            var tmpEvent = ReferencePool.Fetch<InstanceWndEvent.StartInstanceEvent>();
            Game.EventMgr.FireNow(this, tmpEvent);
        }
    }
}

