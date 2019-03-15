using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using ProcedureOwner = AosHotfixFramework.IFsm<AosHotfixFramework.IProcedureManager>;

namespace AosHotfixRunTime
{
    public class ProcedureMain : ProcedureBase
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

            Game.EventMgr.Subscribe(InstanceWndEvent.StartInstanceEvent.EventID, OnEventInstanceStart);
            Game.WindowsMgr.ShowWindow<InstanceWnd>();
        }

        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float deltaTime)
        {
            base.OnUpdate(procedureOwner, deltaTime);
        }

        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            Game.WindowsMgr.CloseWindow<InstanceWnd>();
            Game.EventMgr.Unsubscribe(InstanceWndEvent.StartInstanceEvent.EventID, OnEventInstanceStart);
        }

        protected internal override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        private void OnEventInstanceStart(object sender, GameEventArgs arg)
        {
            ChangeState<ProcedurePVE>(mProcedureOwner);
        }
    }
}
