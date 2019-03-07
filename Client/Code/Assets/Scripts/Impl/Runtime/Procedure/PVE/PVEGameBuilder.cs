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
            SceneLoader.Instance.LoadScene("Instance0", OnSceneLoaded);
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
            mLocalPlayer = new LocalPlayer(1003);
            mLocalPlayer.Init();
            ACT.ActionSystem.Instance.ActUnitMgr.Add(mLocalPlayer);
            ACT.ActionSystem.Instance.ActUnitMgr.LocalPlayer = mLocalPlayer;

            Game.WindowsMgr.ShowWindow<FightMainWnd>();
        }
    }
}
