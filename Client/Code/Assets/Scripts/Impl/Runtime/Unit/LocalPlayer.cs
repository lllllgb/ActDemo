using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class LocalPlayer : Player
    {
        ACT.Controller mController;
        public ACT.Controller Controller { get { return mController; } }

        public LocalPlayer() : base()
        {
        }

        public void Init(int unitID, int level)
        {
            base.Init(unitID, level, EUnitType.EUT_LocalPlayer, ACT.EUnitCamp.EUC_FRIEND);
            
            mController = new ACT.Controller();
            mController.Init(this, CameraMgr.Instance.CameraRootGo.transform);
        }

        public override void LateUpdate(float deltaTime)
        {
            base.LateUpdate(deltaTime);

            mController.LateUpdate();
        }
    }
}

