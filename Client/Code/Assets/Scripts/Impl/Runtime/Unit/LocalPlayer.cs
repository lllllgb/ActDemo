using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ACT;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class LocalPlayer : Player
    {
        ACT.Controller mController;
        public ACT.Controller Controller { get { return mController; } }

        CameraActionManager mCameraActionMgr = new CameraActionManager();

        public LocalPlayer() : base()
        {
            CameraMgr tmpCameraMgr = CameraMgr.Instance;
            mCameraActionMgr.Init(tmpCameraMgr.MainCamera, tmpCameraMgr.CloseupGo, tmpCameraMgr.ShakeGo);
        }

        public void Init(int unitID, int level)
        {
            base.Init(unitID, level, EUnitType.EUT_LocalPlayer, ACT.EUnitCamp.EUC_FRIEND);
            
            mController = new ACT.Controller();
            mController.Init(this, CameraMgr.Instance.CameraRootGo.transform);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            mCameraActionMgr.Update(deltaTime);
        }

        public override void LateUpdate(float deltaTime)
        {
            base.LateUpdate(deltaTime);

            mController.LateUpdate();
        }

        public void LinkSkill(ACT.ISkillInput skillInput, int interruptIndex)
        {
            ActStatus.SkillItem = null;
            ActStatus.LinkAction(ActStatus.ActiveAction.ActionInterrupts[interruptIndex],
                skillInput);
        }

        public void PlaySkill(ACT.ISkillItem skillItem, string action)
        {
            ActStatus.SkillItem = skillItem;
            PlayAction(action);
        }

        public override void PlayCameraAction(int cameraActionID)
        {
            base.PlayCameraAction(cameraActionID);

            mCameraActionMgr.StartAction(cameraActionID);
        }

        public override ECombatResult Combat(IActUnit target, ISkillItem skillItem)
        {
            ECombatResult tmpResult = base.Combat(target, skillItem);

            if (ECombatResult.ECR_Block != tmpResult)
            {
                Game.EventMgr.FireNow(this, ReferencePool.Fetch<UnitEvent.Combo>());
            }

            return tmpResult;
        }
    }
}

