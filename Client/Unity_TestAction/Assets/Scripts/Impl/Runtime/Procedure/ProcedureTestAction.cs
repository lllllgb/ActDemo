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

        protected internal override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);

            mProcedureOwner = procedureOwner;
        }

        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            TestAction.Instance.Init();

            Game.TimerMgr.AddTimer(0.1f, arg =>
            {
                

                
            }, null);
        }

        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float deltaTime)
        {
            base.OnUpdate(procedureOwner, deltaTime);

            TestAction.Instance.Update(deltaTime);
        }

        protected internal override void OnLateUpdate(ProcedureOwner fsm, float deltaTime)
        {
            base.OnLateUpdate(fsm, deltaTime);

            TestAction.Instance.LateUpdate(deltaTime);
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
