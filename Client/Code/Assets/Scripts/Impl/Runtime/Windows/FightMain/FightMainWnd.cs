using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class FightMainWnd : WindowBase
    {
        public override string BundleName { get { return "FightMainWnd"; } }

        protected override bool IsLoadAsync => false;


        private UGUIJoystick mJoystick;

        protected override void AfterInit()
        {
            base.AfterInit();

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

            if (null != mJoystick)
            {
                mJoystick.Update(deltaTime);
            }
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();
        }
    }
}
