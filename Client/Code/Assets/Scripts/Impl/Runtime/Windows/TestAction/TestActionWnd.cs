using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AosHotfixRunTime
{
    public class TestActionWnd : WindowBase
    {
        public override string BundleName { get { return "TestActionWnd"; } }

        protected override bool IsLoadAsync => false;

        private InputField mOtherIDInput;
        private InputField mOtherAIInput;

        private InputField mLocalIDInput;

        private InputField mCameraYInput;
        private InputField mCameraZInput;

        private InputField mSpeedZInput;

        private UGUIJoystick mJoystick;

        protected override void AfterInit()
        {
            base.AfterInit();

            mOtherIDInput = Find<InputField>("InputField_OtherID");
            mOtherAIInput = Find<InputField>("InputField_OtherAI");

            mLocalIDInput = Find<InputField>("InputField_LocalID");

            var tmpAtkTge = Find<Toggle>("Toggle_ShowAtk");
            var tmpDefTge = Find<Toggle>("Toggle_ShowDef");
            var tmpListTargetTge = Find<Toggle>("Toggle_ListTarget");
            tmpAtkTge.onValueChanged.AddListener(OnAtkToogleValueChange);
            tmpDefTge.onValueChanged.AddListener(OnDefToogleValueChange);
            tmpListTargetTge.onValueChanged.AddListener(OnListTargetValueChange);

            mCameraYInput = Find<InputField>("InputField_CameraY");
            mCameraZInput = Find<InputField>("InputField_CameraZ");

            mSpeedZInput = Find<InputField>("InputField_Speed");

            RegisterEventClick(Find("Button_LoadLocal"), OnLoadLocalPlayerBtnClick);
            RegisterEventClick(Find("Button_LoadOther"), OnLoadOtherRoleBtnClick);
            RegisterEventClick(Find("Button_DeleteAll"), OnClearAllBtnClick);
            RegisterEventClick(Find("Button_Camera"), OnCameraBtnClick);
            RegisterEventClick(Find("Button_Speed"), OnSpeedBtnClick);

            var tmpJoystickGo = Find("UGUIJoystick");
            mJoystick = new UGUIJoystick();
            mJoystick.Init(tmpJoystickGo, Find<Image>(tmpJoystickGo, "Background"), Find<Image>(tmpJoystickGo, "Center"));
        }

        protected override void AfterShow()
        {
            base.AfterShow();
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            mJoystick.Update(deltaTime);
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();

        }

        private void OnLoadLocalPlayerBtnClick(PointerEventData arg)
        {
            int tmpID = 0;

            if (int.TryParse(mLocalIDInput.text, out tmpID))
            {
                TestAction.Instance.AddLocalPlayer(tmpID);
            }
            else
            {
                Logger.LogError("ID Input Error!");
            }
        }

        private void OnLoadOtherRoleBtnClick(PointerEventData arg)
        {
            int tmpID = 0;

            if (int.TryParse(mOtherIDInput.text, out tmpID))
            {
                int tmpAIDiff = 0;
                bool tmpAIEnable = int.TryParse(mOtherAIInput.text, out tmpAIDiff);
                TestAction.Instance.AddOtherUnit(tmpID, tmpAIEnable, tmpAIDiff);
            }
            else
            {
                Logger.LogError("ID Input Error!");
            }
        }

        private void OnClearAllBtnClick(PointerEventData arg)
        {
            TestAction.Instance.DeleteAll();
        }

        private void OnCameraBtnClick(PointerEventData arg)
        {
            float tmpY, tmpZ;

            if (float.TryParse(mCameraYInput.text, out tmpY) && float.TryParse(mCameraZInput.text, out tmpZ))
            {
                TestAction.Instance.ModifyCamera(tmpY, tmpZ);
            }
            else
            {
                Logger.LogError("参数填写错误");
            }
        }

        private void OnSpeedBtnClick(PointerEventData arg)
        {
            float tmpSpeed;

            if (float.TryParse(mSpeedZInput.text, out tmpSpeed))
            {
                TestAction.Instance.ModifySpeed(tmpSpeed);
            }
            else
            {
                Logger.LogError("参数填写错误");
            }
        }

        private void OnAtkToogleValueChange(bool value)
        {
            TestAction.Instance.ShowAtkFrame(value);
        }

        private void OnDefToogleValueChange(bool value)
        {
            TestAction.Instance.ShowDefFrame(value);
        }

        private void OnListTargetValueChange(bool value)
        {
            TestAction.Instance.ShowListTargeFrame(value);
        }
    }
}
