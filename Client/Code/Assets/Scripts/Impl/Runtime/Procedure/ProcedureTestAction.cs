using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using ProcedureOwner = AosHotfixFramework.IFsm<AosHotfixFramework.IProcedureManager>;
using UnityEngine.SceneManagement;

namespace AosHotfixRunTime
{
    public class ProcedureTestAction : ProcedureBase
    {
        ProcedureOwner mProcedureOwner;
        LocalPlayer mTestPlayer;
        Monster mTestMonster;

        protected internal override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);

            mProcedureOwner = procedureOwner;

            Game.ResourcesMgr.LoadBundleByType(EABType.Scene, "TestAction");
            SceneManager.LoadScene("TestAction");
        }

        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Game.TimerMgr.AddTimer(0.1f, arg =>
            {
                mTestPlayer = new LocalPlayer(1010);
                mTestPlayer.Init();
                ACT.ActionSystem.Instance.ActUnitMgr.Add(mTestPlayer);

                Monster tmpMonster = new Monster(3001);
                tmpMonster.Init();
                ACT.ActionSystem.Instance.ActUnitMgr.Add(tmpMonster);
            }, null);
        }

        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float deltaTime)
        {
            base.OnUpdate(procedureOwner, deltaTime);

            for (int i = 0, max = ACT.ActionSystem.Instance.ActUnitMgr.Units.Count; i < max; ++i)
            {
                ACT.ActionSystem.Instance.ActUnitMgr.Units[i].Update(deltaTime);
            }
        }

        protected internal override void OnLateUpdate(ProcedureOwner fsm, float deltaTime)
        {
            base.OnLateUpdate(fsm, deltaTime);

            if (null != mTestPlayer)
            {
                mTestPlayer.LateUpdate(deltaTime);
            }
        }

        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected internal override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
