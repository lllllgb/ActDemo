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

        public LocalPlayer(int unitID) : base(unitID)
        {
        }

        public override void Init()
        {
            base.Init();
            
            mController = new ACT.Controller();
            mController.Init(this, CameraHelper.CameraRootGo.transform);
        }

        public override void LateUpdate(float deltaTime)
        {
            base.LateUpdate(deltaTime);

            mController.LateUpdate();
        }
        //public void LinkSkill(SkillInput skillInput, int interruptIndex)
        //{
        //    ActionStatus.LinkAction(
        //        ActionStatus.ActiveAction.ActionInterrupts[interruptIndex],
        //        skillInput);
        //}

        //public void PlaySkill(SkillItem skillItem, string action)
        //{
        //    ActionStatus.SkillItem = skillItem;
        //    PlayAction(action);
        //}
    }
}

