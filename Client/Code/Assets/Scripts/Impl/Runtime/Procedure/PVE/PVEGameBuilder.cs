using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using Utility = AosBaseFramework.Utility;

namespace AosHotfixRunTime
{
    public class PVEGameBuilder : Singleton<PVEGameBuilder>
    {
        public int InstanceID { get; set; }

        InstanceBase mInstanceBase;
        LocalPlayer mLocalPlayer;
        List<Unit> mMonsterList = new List<Unit>();
        int mTriggerCount = 0;

        List<SpriteRenderer> mSceneMaskSprs = new List<SpriteRenderer>();

        public void Init()
        {
            Game.EventMgr.Subscribe(CameraActionEvent.ModifySceneMaskColor.EventID, OnEventModifySceneMask);
            Game.EventMgr.Subscribe(UnitEvent.Dead.EventID, OnEventUnitDead);
        }

        public void Start()
        {
            mInstanceBase = InstanceBaseManager.instance.Find(InstanceID);

            if (null == mInstanceBase)
            {
                Logger.LogError($"找不到副本配置 id -> {InstanceID}");
                return;
            }

            var tmpUnitCtrl = Game.ControllerMgr.Get<UnitController>();

            mLocalPlayer = new LocalPlayer();
            mLocalPlayer.Init(1003, 1);
            mLocalPlayer.SetPosition(new Vector3(1000, 0, 0));
            GameObject.DontDestroyOnLoad(mLocalPlayer.UGameObject);
            ACT.ActionSystem.Instance.ActUnitMgr.Add(mLocalPlayer);
            ACT.ActionSystem.Instance.ActUnitMgr.LocalPlayer = mLocalPlayer;
            tmpUnitCtrl.SetLocalPlayer(mLocalPlayer);

            Game.WindowsMgr.ShowWindow<FadeWnd, bool, bool>(true, false);
            Game.WindowsMgr.ShowWindow<FightMainWnd>();
            SceneLoader.Instance.LoadSceneAsync(mInstanceBase.SceneName, OnSceneLoaded);
        }

        public void Update(float deltaTime)
        {
            ACT.ActionSystem.Instance.ActUnitMgr.Update(deltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            if (null != mLocalPlayer && !mLocalPlayer.Dead)
            {
                mLocalPlayer.LateUpdate(deltaTime);
            }
        }

        private void Reset()
        {
            if (null != mLocalPlayer)
            {
                mLocalPlayer.Dispose();
                mLocalPlayer = null;
            }
            
            for (int i = 0, max = mMonsterList.Count; i < max; ++i)
            {
                mMonsterList[i].Dispose();
            }

            mMonsterList.Clear();
        }

        public void Release()
        {
            Game.EventMgr.Unsubscribe(CameraActionEvent.ModifySceneMaskColor.EventID, OnEventModifySceneMask);
            Game.EventMgr.Unsubscribe(UnitEvent.Dead.EventID, OnEventUnitDead);

            Reset();
        }

        public void ReStart()
        {
            Reset();
            Start();
        }

        private void TransferNextScene(string sceneName)
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
            Game.WindowsMgr.ShowWindow<FadeWnd, bool, bool>(true, false);
            SceneLoader.Instance.LoadSceneAsync(sceneName, OnSceneLoaded);
        }

        void OnSceneLoaded()
        {
            Game.WindowsMgr.ShowWindow<FadeWnd, bool, bool>(false, true);

            GameObject tmpSceneMaskGo = GameObject.Find("SceneMask");

            mSceneMaskSprs.Clear();
            if (null != tmpSceneMaskGo)
            {
                for (int i = 0, max = tmpSceneMaskGo.transform.childCount; i < max; ++i)
                {
                    SpriteRenderer tmpSprRender = tmpSceneMaskGo.transform.GetChild(i).GetComponent<SpriteRenderer>();

                    if (null != tmpSprRender)
                    {
                        mSceneMaskSprs.Add(tmpSprRender);
                        tmpSprRender.color = Color.clear;
                    }
                }
            }

            GameObject tmpInstanceRoot = GameObject.Find("InstanceRoot");
            mTriggerCount = 0;
            GameObject tmpTransferGo = Utility.GameObj.Find(tmpInstanceRoot, "Transfer");

            if (tmpTransferGo)
            {
                mTriggerCount += tmpTransferGo.transform.childCount;
                for (int i = 0, max = tmpTransferGo.transform.childCount; i < max; ++i)
                {
                    TriggerListener.Get(tmpTransferGo.transform.GetChild(i).gameObject).OnEnter = OnTriggerEnterTransfer;
                }
            }

            GameObject tmpTriggerGo = Utility.GameObj.Find(tmpInstanceRoot, "Trigger");

            if (tmpTriggerGo)
            {
                mTriggerCount += tmpTriggerGo.transform.childCount;
                for (int i = 0, max = tmpTriggerGo.transform.childCount; i < max; ++i)
                {
                    TriggerListener.Get(tmpTriggerGo.transform.GetChild(i).gameObject).OnEnter = OnTriggerEnterTrigger;
                }
            }

            GameObject tmpBornPosGo = Utility.GameObj.Find(tmpInstanceRoot, "BornPos");

            if (tmpBornPosGo)
            {
                mLocalPlayer.SetPosition(tmpBornPosGo.transform.position);
                mLocalPlayer.Controller.RepositionCamera();
            }

        }

        void OnTriggerEnterTransfer(GameObject self, Collider other)
        {
            if (other.gameObject == mLocalPlayer.UGameObject)
            {
                --mTriggerCount;
                for (int i = 0, max = mInstanceBase.TransferTriggerInfo.data.Count; i < max; ++i)
                {
                    var tmpTransferTrigger = mInstanceBase.TransferTriggerInfo.data[i];

                    if (self.name.Equals(tmpTransferTrigger.TriggerName))
                    {
                        TransferNextScene(tmpTransferTrigger.InstanceName);
                        break;
                    }
                }
            }
        }

        void OnTriggerEnterTrigger(GameObject self, Collider other)
        {
            if (other.gameObject == mLocalPlayer.UGameObject)
            {
                --mTriggerCount;
                self.SetActive(false);

                for (int i = 0, max = mInstanceBase.MonsterTriggerInfo.data.Count; i < max; ++i)
                {
                    var tmpMonsterTrigger = mInstanceBase.MonsterTriggerInfo.data[i];

                    if (self.name.Equals(tmpMonsterTrigger.TriggerName))
                    {
                        var tmpTriggerBase = MonsterTriggerBaseManager.instance.Find(tmpMonsterTrigger.TriggerID);

                        if (null != tmpTriggerBase)
                        {
                            TriggerMonster(tmpTriggerBase);
                        }
                        break;
                    }
                }

            }
        }

        private void TriggerMonster(MonsterTriggerBase triggerBase)
        {
            if (null == triggerBase)
            {
                return;
            }

            for (int i = 0, max = triggerBase.MonsterInfo.data.Count; i < max; ++i)
            {
                var tmpMonsterInfo = triggerBase.MonsterInfo.data[i];

                for (int j = 0, jmax = tmpMonsterInfo.Count; j < jmax; ++j)
                {
                    SpawnMonster(tmpMonsterInfo.MonsterID, tmpMonsterInfo.Level, tmpMonsterInfo.AIDiff, 1 == tmpMonsterInfo.IsBoss);
                }
            }
        }

        private void SpawnMonster(int unitID, int level, int aiDiff, bool isBoss)
        {
            var tmpMonster = new Monster();
            tmpMonster.Init(unitID, level, EUnitType.EUT_Monster, ACT.EUnitCamp.EUC_ENEMY, true, aiDiff);
            tmpMonster.SetIsBoss(isBoss);
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

        private void OnEventUnitDead(object sender, GameEventArgs arg)
        {
            var tmpEventArg = arg as UnitEvent.Dead;

            if (null == tmpEventArg)
            {
                return;
            }

            if (tmpEventArg.Data.UnitType == EUnitType.EUT_LocalPlayer)
            {
                ACT.ActionSystem.Instance.ActUnitMgr.LocalPlayer = null;
                mLocalPlayer = null;
                DelaySettle(false);
            }
            else
            {
                if ((tmpEventArg.Data as Monster).IsBoss)
                {
                    for (int i = 0, max = mMonsterList.Count; i < max; ++i)
                    {
                        mMonsterList[i].Dispose();
                    }

                    mMonsterList.Clear();
                }

                if (mTriggerCount <= 0)
                {
                    bool tmpFlag = true;

                    for (int i = 0, max = mMonsterList.Count; i < max; ++i)
                    {
                        if (!mMonsterList[i].Dead)
                        {
                            tmpFlag = false;
                            break;
                        }
                    }

                    if (tmpFlag)
                    {
                        DelaySettle(true);
                    }
                }
            }
        }

        private void DelaySettle(bool flag)
        {
            Game.TimerMgr.AddTimer(2f, arg => {
                Game.WindowsMgr.ShowWindow<PVESettleWnd, bool>(flag);
            }, null);
        }
    }
}
