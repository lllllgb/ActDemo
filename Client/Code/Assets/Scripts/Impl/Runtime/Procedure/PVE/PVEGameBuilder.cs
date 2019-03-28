using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using Utility = AosBaseFramework.Utility;

namespace AosHotfixRunTime
{
    public class PVEGameBuilder : Singleton<PVEGameBuilder>
    {
        public int DifficultyLevel { get; set; }
        LocalPlayer mLocalPlayer;
        int mInstanceID;
        GameObject mInstanceRoot;

        List<Unit> mMonsterList = new List<Unit>();
        bool mIsTriggerMonster = true;

        List<SpriteRenderer> mSceneMaskSprs = new List<SpriteRenderer>();

        public void Init()
        {
            mInstanceID = 1;
            var tmpUnitCtrl = Game.ControllerMgr.Get<UnitController>();

            mLocalPlayer = new LocalPlayer();
            mLocalPlayer.Init(1003, 1);
            GameObject.DontDestroyOnLoad(mLocalPlayer.UGameObject);
            ACT.ActionSystem.Instance.ActUnitMgr.Add(mLocalPlayer);
            ACT.ActionSystem.Instance.ActUnitMgr.LocalPlayer = mLocalPlayer;
            tmpUnitCtrl.SetLocalPlayer(mLocalPlayer);

            Game.WindowsMgr.ShowWindow<FightMainWnd>();

            Game.WindowsMgr.ShowWindow<FadeWnd, bool, bool>(true, false);
            SceneLoader.Instance.LoadSceneAsync("Instance1", OnSceneLoaded);

            Game.EventMgr.Subscribe(CameraActionEvent.ModifySceneMaskColor.EventID, OnEventModifySceneMask);
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
            Game.EventMgr.Unsubscribe(CameraActionEvent.ModifySceneMaskColor.EventID, OnEventModifySceneMask);
        }

        private void TransferNextScene()
        {
            for (int i = 0, max = mMonsterList.Count; i < max; ++i)
            {
                if (!mMonsterList[i].Dead)
                {
                    return;
                }
            }

            for (int i = 0, max = mMonsterList.Count; i < max; ++i)
            {
                mMonsterList[i].Dispose();
            }

            mMonsterList.Clear();
            mInstanceID++;
            Game.WindowsMgr.ShowWindow<FadeWnd, bool, bool>(true, false);
            mSceneMaskSprs.Clear();
            SceneLoader.Instance.LoadSceneAsync($"Instance{mInstanceID}", OnSceneLoaded);
        }

        void OnSceneLoaded()
        {
            Game.WindowsMgr.ShowWindow<FadeWnd, bool, bool>(false, true);

            GameObject tmpSceneMaskGo = GameObject.Find("SceneMask");

            if (null != tmpSceneMaskGo)
            {
                for (int i = 0, max = tmpSceneMaskGo.transform.childCount; i < max; ++i)
                {
                    SpriteRenderer tmpSprRender = tmpSceneMaskGo.transform.GetChild(i).GetComponent<SpriteRenderer>();

                    if (null != tmpSprRender)
                    {
                        mSceneMaskSprs.Add(tmpSprRender);
                    }
                }
            }

            Game.ResourcesMgr.LoadBundleByType(EABType.Misc, $"Instacen{mInstanceID}");
            mInstanceRoot = Hotfix.Instantiate(Game.ResourcesMgr.GetAssetByType<GameObject>(EABType.Misc, $"Instacen{mInstanceID}"));

            GameObject tmpTransferGo = Utility.GameObj.Find(mInstanceRoot, "Transfer");

            if (tmpTransferGo)
            {
                TriggerListener.Get(tmpTransferGo).OnEnter = OnTriggerEnterTransfer;
            }

            GameObject tmpTriggerGo = Utility.GameObj.Find(mInstanceRoot, "Trigger");

            if (tmpTriggerGo)
            {
                TriggerListener.Get(tmpTriggerGo).OnEnter = OnTriggerEnterTrigger;
            }

            GameObject tmpBornPosGo = Utility.GameObj.Find(mInstanceRoot, "BornPos");
            mLocalPlayer.SetPosition(tmpBornPosGo.transform.position);
            mIsTriggerMonster = false;
        }

        void OnTriggerEnterTransfer(Collider other)
        {
            if (other.gameObject == mLocalPlayer.UGameObject)
            {
                TransferNextScene();
            }
        }

        void OnTriggerEnterTrigger(Collider other)
        {
            if (other.gameObject == mLocalPlayer.UGameObject && !mIsTriggerMonster)
            {
                mIsTriggerMonster = true;
                TriggerMonster();
            }
        }

        private void TriggerMonster()
        {
            if (mInstanceID == 1)
            {
                SpawnMonster(1004, 1, 0);
                SpawnMonster(1004, 1, 0);
                SpawnMonster(1004, 1, 0);
            }
            else if (mInstanceID == 2)
            {
                SpawnMonster(1004, 1, 0);
                SpawnMonster(1004, 1, 0);
                SpawnMonster(1005, 1, 0);
                SpawnMonster(1005, 1, 0);
            }
            else if (mInstanceID == 3)
            {
                SpawnMonster(1004, 1, 0);
                SpawnMonster(1004, 1, 0);
                SpawnMonster(1005, 1, 0);
                SpawnMonster(1005, 1, 0);
                SpawnMonster(1006, 1, 0);
            }
        }

        private void SpawnMonster(int unitID, int level, int aiDiff)
        {
            var tmpMonster = new Monster();
            tmpMonster.Init(unitID, level, EUnitType.EUT_Monster, ACT.EUnitCamp.EUC_ENEMY, true, aiDiff);
            tmpMonster.SetPosition(new Vector3(Random.Range(0, 5), 0f, Random.Range(0, 5)));
            float x = mLocalPlayer.Position.x - tmpMonster.Position.x;
            float dir = Mathf.Atan2(x, 0);
            tmpMonster.SetOrientation(dir);

            ACT.ActionSystem.Instance.ActUnitMgr.Add(tmpMonster);
            mMonsterList.Add(tmpMonster);
        }

        private void OnEventModifySceneMask(object sender, GameEventArgs arg)
        {
            var tmpEventArg = arg as CameraActionEvent.ModifySceneMaskColor;

            if (null == tmpEventArg)
            {
                return;
            }

            for (int i = 0, max = mSceneMaskSprs.Count; i < max; ++i)
            {
                mSceneMaskSprs[i].color = tmpEventArg.Data;
            }
        }
    }
}
