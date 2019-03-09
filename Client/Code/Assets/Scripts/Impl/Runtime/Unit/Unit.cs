using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{

    public abstract partial class Unit : ACT.ActUnit
    {

        public override int CurHp { get { return GetAttrib(EPA.CurHP); } }
        public override int HpMax { get { return GetAttrib(EPA.MaxHP); } }
        public override int Speed { get { return 300; } }
        public override bool CanHurt { get { return true; } }
        
        public EUnitType UnitType { get; private set; }
        public string Name { get; private set; }
        public Transform TopNode { get; private set; }
        public Transform MidNode { get; private set; }
        public Transform BottomNode { get; private set; }

        public Unit()
        {
        }

        protected void Init(int unitID, int level, EUnitType unitType, ACT.EUnitCamp unitCamp)
        {
            UnitID = unitID;
            Level = level;
            UnitType = unitType;
            Camp = unitCamp;

            UnitBase tmpUnitBase = UnitBaseManager.instance.Find(UnitID);

            if (null == tmpUnitBase)
            {
                Logger.LogError($"找不到UnitBase ID -> {UnitID}");
                return;
            }

            ActionID = tmpUnitBase.ActionID;
            Game.ResourcesMgr.LoadBundleByType(EABType.Unit, tmpUnitBase.Prefab);
            GameObject tmpGo = Game.ResourcesMgr.GetAssetByType<GameObject>(EABType.Unit, tmpUnitBase.Prefab);

            if (null == tmpGo)
            {
                Logger.LogError($"未能成功加载单位预制 prefab -> {tmpUnitBase.Prefab}");
                return;
            }

            tmpGo = Hotfix.Instantiate(tmpGo);
            tmpGo.transform.localEulerAngles = new Vector3(0, 90, 0);
            InitActUnit(tmpGo, tmpGo.transform.Find("model"));
            UpdateAttributes();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            UpdateComponents(deltaTime);
        }

        public virtual void LateUpdate(float deltaTime)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            
            GameObject.Destroy(UGameObject);
        }

        //更新属性
        public abstract void UpdateAttributes();

        //名字
        public void SetName(string name)
        {
            Name = name;
        }

        public abstract bool Hurt(Unit attacker, int damage, ACT.ECombatResult result);
        public virtual void AddHp(int v) { }
        public virtual void AddSoul(int v) { }
        public virtual void AddAbility(int v) { }
        public virtual bool UpLevel() { return false; }
        public virtual int GetAttrib(EPA idx) { return 0; }
        public virtual void Equip() { }
        public virtual void Revive() { }

        public override ACT.ECombatResult Combat(ACT.IActUnit target, ACT.ISkillItem skillItem)
        {
            int damage = 0;
            ACT.ECombatResult result = ACT.ECombatResult.ECR_Normal;

            return result;
        }
        
    }
}
