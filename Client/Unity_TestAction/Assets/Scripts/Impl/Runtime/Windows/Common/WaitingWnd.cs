using System;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine.UI;
using UnityEngine;


namespace AosHotfixRunTime
{
    public class WaitingWnd : WindowBase
    {
        public override string BundleName { get { return "WaitingWnd"; } }
        protected override bool IsLoadAsync { get { return false; } }

        public WaitingWnd()
        {
            mWindowType = EWindowType.Loading;
        }

        protected override void AfterInit()
        {
            base.AfterInit();
        }

        protected override void AfterShow()
        {
            base.AfterShow();
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();
        }

        protected override void BeforeDestory()
        {
            base.BeforeDestory();
        }
    }
}
