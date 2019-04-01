using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using ProcedureOwner = AosHotfixFramework.IFsm<AosHotfixFramework.IProcedureManager>;
using UnityEngine.SceneManagement;

namespace AosHotfixRunTime
{
    public class ProcedurePVE : ProcedureBase
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

            Game.EventMgr.Subscribe(PVESettleWndEvent.BackMainEvent.EventID, OnEventBackMain);

            PVEGameBuilder.Instance.Init();
            PVEGameBuilder.Instance.Start();
        }

        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float deltaTime)
        {
            base.OnUpdate(procedureOwner, deltaTime);

            PVEGameBuilder.Instance.Update(deltaTime);
        }

        protected internal override void OnLateUpdate(ProcedureOwner fsm, float deltaTime)
        {
            base.OnLateUpdate(fsm, deltaTime);

            PVEGameBuilder.Instance.LateUpdate(deltaTime);
        }

        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            PVEGameBuilder.Instance.Release();
            Game.WindowsMgr.CloseWindow(WindowBase.EWindowType.ALL);
            Game.EventMgr.Unsubscribe(PVESettleWndEvent.BackMainEvent.EventID, OnEventBackMain);
        }

        protected internal override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        private void OnEventBackMain(object sender, GameEventArgs arg)
        {
            ChangeState<ProcedureMain>(mProcedureOwner);
        }
    }
}
