using System;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AosHotfixRunTime
{
    public class MessageBoxWnd : WindowBase
    {
        [Flags]
        public enum EStyle
        {
            Confirm = 1 << 0,
            Cancel = 1 << 1,
            All = Confirm | Cancel,
        }

        public override string BundleName { get { return "MessageBoxWnd"; } }
        protected override bool IsLoadAsync { get { return false; } }

        Text mMsgContentLabel;
        GameObject mCanceBtn;
        GameObject mMidBtn;
        Action mConfirmHandle;
        Action mCancelHandle;

        protected override void AfterInit()
        {
            mMsgContentLabel = Find<Text>("Text_Content");
            mCanceBtn = Find("Button_Cancel");

            RegisterEventClick(Find("Button_Mid"), OnMidBtnClick);
            RegisterEventClick(mCanceBtn, OnCancel);
            RegisterEventClick(Find("Button_Cloese"),OnClose);
        }

        private void OnClose(PointerEventData eventdata)
        {
            Close();
        }

        private void OnCancel(PointerEventData eventdata)
        {
            Close();

            mCancelHandle?.Invoke();
        }

        protected override void AfterShow()
        {
        }

        protected override void BeforeClose()
        {
        }


        private void OnMidBtnClick(PointerEventData arg)
        {
            Close();

            mConfirmHandle?.Invoke();
        }

        void SetShowData(EStyle style, string content,
            Action confirmHandle = null, Action cancelHandle = null)
        {
            mMsgContentLabel.text = content;
            mConfirmHandle = confirmHandle;

            mCancelHandle = cancelHandle;

            SetActive(mCanceBtn, cancelHandle != null);
        }

        public static void ShowMsgBox(EStyle style, string content, Action confirmHandle = null, Action cancelHandle = null)
        {
            Game.WindowsMgr.ShowWindow<MessageBoxWnd>(delegate(WindowBase wnd)
            {
                MessageBoxWnd tmpWnd = wnd as MessageBoxWnd;
                tmpWnd.SetShowData(style, content, confirmHandle, cancelHandle);
            });
        }
    }
}
