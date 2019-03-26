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
            ACT.ActionSystem.Instance.ActUnitMgr.Update(deltaTime);
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
            mTestPlayer = new LocalPlayer();
            mTestPlayer.Init(unitID, 1);
            ACT.ActionSystem.Instance.ActUnitMgr.Add(mTestPlayer);
            ACT.ActionSystem.Instance.ActUnitMgr.LocalPlayer = mTestPlayer;
        }

        public void AddOtherUnit(int unitID, bool aiEnable, int aiDiff)
        {
            Monster tmpMonster = new Monster();
            tmpMonster.Init(unitID, 1, EUnitType.EUT_Monster, ACT.EUnitCamp.EUC_ENEMY, aiEnable, aiDiff);
            ACT.ActionSystem.Instance.ActUnitMgr.Add(tmpMonster);
        }

        public void DeleteAll()
        {
            for (int i = 0, max = ACT.ActionSystem.Instance.ActUnitMgr.Units.Count; i < max; ++i)
            {
                ACT.ActionSystem.Instance.ActUnitMgr.Units[i].Dispose();
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

        public void ModifyCamera(float y, float z)
        {
            if (null != mTestPlayer && null != mTestPlayer.Controller)
            {
                mTestPlayer.Controller.CameraPos = new Vector3(0f, y, z);
            }
        }

        public void ModifySpeed(float speed)
        {
            if (null != mTestPlayer)
            {
                mTestPlayer.MoveZMultiple = speed;
            }
        }
    }
}
