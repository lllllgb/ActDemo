using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using UnityEngine.SceneManagement;

namespace AosHotfixRunTime
{
    public class TestAction : Singleton<TestAction>
    {
        LocalPlayer mTestPlayer;

        public void Init()
        {
            Game.ResourcesMgr.LoadBundleByType(EABType.Scene, "TestAction");
            SceneManager.LoadScene("TestAction");

            Game.WindowsMgr.ShowWindow<TestActionWnd>();
        }

        public void Update(float deltaTime)
        {
            for (int i = 0, max = ACT.ActionSystem.Instance.ActUnitMgr.Units.Count; i < max; ++i)
            {
                ACT.ActionSystem.Instance.ActUnitMgr.Units[i].Update(deltaTime);
            }
        }

        public void LateUpdate(float deltaTime)
        {
            if (null != mTestPlayer)
            {
                mTestPlayer.LateUpdate(deltaTime);
            }
        }


        public void AddLocalPlayer(int unitID)
        {
            mTestPlayer = new LocalPlayer(unitID);
            mTestPlayer.Init();
            ACT.ActionSystem.Instance.ActUnitMgr.Add(mTestPlayer);
        }

        public void AddOtherUnit(int unitID)
        {
            Monster tmpMonster = new Monster(unitID);
            tmpMonster.Init();
            ACT.ActionSystem.Instance.ActUnitMgr.Add(tmpMonster);
        }

        public void DeleteAll()
        {
            for (int i = 0, max = ACT.ActionSystem.Instance.ActUnitMgr.Units.Count; i < max; ++i)
            {
                ACT.ActionSystem.Instance.ActUnitMgr.Units[i].Destory();
            }

            ACT.ActionSystem.Instance.ActUnitMgr.ClearAll();
            mTestPlayer = null;

            ACT.ActionManager.Instance.Clear();
        }

        public void ShowAtkFrame(bool flag)
        {
            ACT.HitDefinition.ShowAttackFrame = flag;
        }

        public void ShowDefFrame(bool flag)
        {
            ACT.ActionStatus.ShowBeatenFrame = flag;
        }

        public void ShowListTargeFrame(bool flag)
        {
            ACT.ActionStatus.ShowListTarFrame = flag;
        }
    }
}
