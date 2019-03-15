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
        int mDifficultyLevel = 1;

        protected override void AfterInit()
        {
            base.AfterInit();

            RegisterEventClick(Find("Button_Level"), OnLevelBtnClick);
            mDescPanel = Find("DescPanelRoot");
            RegisterEventClick(Find(mDescPanel, "Button_Normal"), OnNormalBtnClick);
            RegisterEventClick(Find(mDescPanel, "Button_Hard"), OnDifficultyBtnClick);
            RegisterEventClick(Find(mDescPanel, "Button_Hell"), OnHellBtnClick);
            RegisterEventClick(Find(mDescPanel, "Button_Enter"), OnStartBtnClick);
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

        private void OnLevelBtnClick(PointerEventData arg)
        {
            SetActive(mDescPanel, true);
        }

        private void OnNormalBtnClick(PointerEventData arg)
        {
            mDifficultyLevel = 1;
        }

        private void OnDifficultyBtnClick(PointerEventData arg)
        {
            mDifficultyLevel = 5;
        }

        private void OnHellBtnClick(PointerEventData arg)
        {
            mDifficultyLevel = 10;
        }

        private void OnStartBtnClick(PointerEventData arg)
        {
            PVEGameBuilder.Instance.DifficultyLevel = mDifficultyLevel;
            var tmpEvent = ReferencePool.Fetch<InstanceWndEvent.StartInstanceEvent>();
            tmpEvent.DifficultyLevel = mDifficultyLevel;
            Game.EventMgr.FireNow(this, tmpEvent);
        }
    }
}

