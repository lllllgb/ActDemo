using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosBaseFramework;

namespace AosHotfixRunTime
{
    public class HudPopupDamage : HudPopupBase, IPopupStart1<int>
    {
        public const string RES_NAME = "HUD_Popup_Damage";

        public override EHudPopupType PopupType => EHudPopupType.Damage;

        protected override string ResName => RES_NAME;

        Text mPopupValueLab;

        protected override void OnResLoaded()
        {
            base.OnResLoaded();

            if (null == mGameObject)
            {
                return;
            }

            mPopupValueLab = Utility.GameObj.Find<Text>(mGameObject, "Text_Value");
        }

        public override void OnUnspawn()
        {
            base.OnUnspawn();

            mPopupValueLab = null;
        }

        public void StartPopup(int value)
        {
            if (null != mPopupValueLab)
            {
                mPopupValueLab.text = value.ToString();
            }

            if (null != PopupAnimation)
            {
                Duration = PopupAnimation.clip.length;
                PopupAnimation.Play();
            }

            if (null != Owner.TopNode)
            {
                SetPopupPos(Owner.TopNode.position);
            }
        }
    }
}
