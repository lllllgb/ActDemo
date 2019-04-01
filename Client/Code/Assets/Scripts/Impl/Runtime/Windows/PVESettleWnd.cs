using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;
using UnityEngine.EventSystems;

namespace AosHotfixRunTime
{
    public class PVESettleWnd : WindowBase, WindowBase.IInitData<bool>
    {
        public override string BundleName { get { return "PVESettleWnd"; } }

        protected override bool IsLoadAsync => false;

        private GameObject mVictoryGo;
        private GameObject mLoseGo;

        bool mIsVictory;

        protected override void AfterInit()
        {
            base.AfterInit();

            mVictoryGo = Find("Image_Victory");
            mLoseGo = Find("Image_Lose");

            RegisterEventClick(Find("Button_Back"), OnBackBtnClick);
            RegisterEventClick(Find("Button_Again"), OnAgainBtnClick);
        }

        public void InitData(bool a)
        {
            mIsVictory = a;
        }

        protected override void AfterShow()
        {
            base.AfterShow();

            SetActive(mVictoryGo, mIsVictory);
            SetActive(mLoseGo, !mIsVictory);
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();
        }

        protected override void BeforeDestory()
        {
            base.BeforeDestory();
        }

        private void OnBackBtnClick(PointerEventData arg)
        {
            Game.EventMgr.FireNow(this, ReferencePool.Fetch<PVESettleWndEvent.BackMainEvent>());
        }

        private void OnAgainBtnClick(PointerEventData arg)
        {
            Close();
            PVEGameBuilder.Instance.ReStart();
        }
    }
}
