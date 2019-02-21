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

            RegisterEventClick(Find("Button_LoadLocal"), OnLoadLocalPlayerBtnClick);
            RegisterEventClick(Find("Button_LoadOther"), OnLoadOtherRoleBtnClick);
            RegisterEventClick(Find("Button_DeleteAll"), OnClearAllBtnClick);
        }

        protected override void AfterShow()
        {
            base.AfterShow();
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
                TestAction.Instance.AddOtherUnit(tmpID);
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
