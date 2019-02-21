using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AosBaseFramework
{
    public class MessageBoxUI : UIBase
    {
        [Flags]
        public enum EStyle
        {
            Left = 1 << 0,
            Middle = 1 << 1,
            Right = 1 << 2,

            L_R = Left | Right,
        }

        public const string CANCEL_STR = "取消";
        public const string CONFIRM_STR = "确定";
        public const string RETRY_STR = "重试";

        protected override string ResName
        {
            get
            {
                return "MessageBoxUI";
            }
        }

        private Text mContentText;
        private GameObject mLeftBtnGo;
        private Text mLeftBtnDescText;
        private GameObject mMiddleBtnGo;
        private Text mMiddleBtnDescText;
        private GameObject mRightBtnGo;
        private Text mRightBtnDescText;

        private Action mLeftBtnHandle;
        private Action mMiddleBtnHandle;
        private Action mRightBtnHandle;

        protected override void AfterInit()
        {
            mContentText = Utility.GameObj.Find<Text>(RootGo, "Text_Tip");
            mLeftBtnGo = Utility.GameObj.Find(RootGo, "Button_Left");
            mLeftBtnDescText = Utility.GameObj.Find<Text>(mLeftBtnGo, "Text");
            mMiddleBtnGo = Utility.GameObj.Find(RootGo, "Button_Mid");
            mMiddleBtnDescText = Utility.GameObj.Find<Text>(mMiddleBtnGo, "Text");
            mRightBtnGo = Utility.GameObj.Find(RootGo, "Button_Right");
            mRightBtnDescText = Utility.GameObj.Find<Text>(mRightBtnGo, "Text");

            RegisterEventClick(mLeftBtnGo, OnLeftBtnClick);
            RegisterEventClick(mMiddleBtnGo, OnMiddleBtnClick);
            RegisterEventClick(mRightBtnGo, OnRightBtnClick);
        }

        protected override void AfterShow()
        {

        }

        protected override void BeforeClose()
        {
            mLeftBtnHandle = null;
            mMiddleBtnHandle = null;
            mRightBtnHandle = null;
        }

        private void OnLeftBtnClick(PointerEventData eventdata)
        {
            mLeftBtnHandle?.Invoke();
            Close();
        }

        private void OnMiddleBtnClick(PointerEventData eventdata)
        {
            mMiddleBtnHandle?.Invoke();
            Close();
        }

        private void OnRightBtnClick(PointerEventData eventdata)
        {
            mRightBtnHandle?.Invoke();
            Close();
        }

        public void SetShowData(EStyle style, string content, string lBtnStr = null, Action lBtnHandle = null, 
            string mBtnStr = null, Action mBtnHandle = null, string rBtnStr = null, Action rBtnHandle = null)
        {
            Utility.GameObj.SetActive(mLeftBtnGo, 0 != (style & EStyle.Left));
            Utility.GameObj.SetActive(mMiddleBtnGo, 0 != (style & EStyle.Middle));
            Utility.GameObj.SetActive(mRightBtnGo, 0 != (style & EStyle.Right));

            if (null != mContentText)
            {
                mContentText.text = content;
            }

            if (null != lBtnStr)
            {
                mLeftBtnDescText.text = lBtnStr;
            }

            if (null != mBtnStr)
            {
                mMiddleBtnDescText.text = mBtnStr;
            }

            if (null != rBtnStr)
            {
                mRightBtnDescText.text = rBtnStr;
            }

            mLeftBtnHandle = lBtnHandle;
            mMiddleBtnHandle = mBtnHandle;
            mRightBtnHandle = rBtnHandle;
        }
    }
}
