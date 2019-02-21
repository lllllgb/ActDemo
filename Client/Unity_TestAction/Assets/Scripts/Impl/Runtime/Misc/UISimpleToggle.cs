using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AosHotfixFramework;
using AosBaseFramework;

namespace AosHotfixRunTime
{
    public class UISimpleToggle
    {
        const string BACKGROUND_NAME = "background";
        const string MASK_NAME = "mask";

        GameObject mBackgroundGo;
        GameObject mMaskGo;

        private bool mValue = false;
        public bool Value
        {
            set
            {
                mValue = value;
                Utility.GameObj.SetActive(mMaskGo, mValue);
            }
            get { return mValue; }
        }

        public UISimpleToggle()
        {
        }

        public void Init(GameObject toggleGo)
        {
            mBackgroundGo = Utility.GameObj.Find(toggleGo, BACKGROUND_NAME);
            mMaskGo = Utility.GameObj.Find(toggleGo, MASK_NAME);
            Utility.GameObj.SetActive(mMaskGo, mValue);

            UGUIEventListener.Get(toggleGo).onClick += OnClick;
        }

        private void OnClick(PointerEventData arg)
        {
            Value = !mValue;
        }
    }
}
