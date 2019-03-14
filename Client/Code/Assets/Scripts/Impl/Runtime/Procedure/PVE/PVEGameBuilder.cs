using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class PVEGameBuilder : Singleton<PVEGameBuilder>
    {
        LocalPlayer mLocalPlayer;

        public void Init()
        {
            SceneLoader.Instance.LoadScene("Instance1", OnSceneLoaded);
        }

        public void Update(float deltaTime)
        {
            ACT.ActionSystem.Instance.ActUnitMgr.Update(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            if (null != mLocalPlayer)
            {
                mLocalPlayer.LateUpdate(deltaTime);
            }
        }

        public void Release()
        {
        }

        void OnSceneLoaded()
        {
            Game.ControllerMgr.Get<PlayerController>().Init(1003, 1);
            var tmpUnitCtrl = Game.ControllerMgr.Get<UnitController>();

            mLocalPlayer = new LocalPlayer();
            mLocalPlayer.Init(1003, 1);
            ACT.ActionSystem.Instance.ActUnitMgr.Add(mLocalPlayer);
            ACT.ActionSystem.Instance.ActUnitMgr.LocalPlayer = mLocalPlayer;
            tmpUnitCtrl.SetLocalPlayer(mLocalPlayer);

            var tmpMonster = new Monster();
            tmpMonster.Init(1004, 1);
            ACT.ActionSystem.Instance.ActUnitMgr.Add(tmpMonster);

            Game.WindowsMgr.ShowWindow<FightMainWnd>();
        }
    }
}
