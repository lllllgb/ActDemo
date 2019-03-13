using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AosHotfixFramework;
using AosBaseFramework;

namespace AosHotfixRunTime
{

    public abstract partial class Unit : ACT.ActUnit
    {

        public override int CurHp { get { return GetAttr(EPA.CurHP); } }
        public override int HpMax { get { return GetAttr(EPA.MaxHP); } }
        public override int Speed { get { return 300; } }
        public override bool CanHurt { get { return true; } }
        
        public EUnitType UnitType { get; private set; }
        public string Name { get; private set; }
        public Transform TopNode { get; private set; }
        public Transform MidNode { get; private set; }
        public Transform BottomNode { get; private set; }
        //属性
        protected UnitAttr mUnitAttr = new UnitAttr();

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
            TopNode = Utility.GameObj.FindByName(tmpGo.transform, "topNode");
            MidNode = Utility.GameObj.FindByName(tmpGo.transform, "midNode");
            BottomNode = Utility.GameObj.FindByName(tmpGo.transform, "bottomNode");
            UpdateAttributes();
            AddComponents();
        }

        protected virtual void AddComponents()
        {
            AddComponent<HudPopupComponent>();
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

        public virtual int GetAttr(EPA idx)
        {
            return mUnitAttr.Get(idx);
        }

        public virtual void Equip() { }
        public virtual void Revive() { }

        public override ACT.ECombatResult Combat(ACT.IActUnit target, ACT.ISkillItem skillItem)
        {
            int tmpDamage = 0;
            Unit tmpTarget = target as Unit;
            ACT.ECombatResult tmpResult = MathUtility.Combat(this, tmpTarget, out tmpDamage);

            if (ACT.ECombatResult.ECR_Block != tmpResult)
            {
                int tmpDamageCoff = 100;
                int tmpDamageBase = 0;
                // 技能的影响。
                tmpDamage = tmpDamage * tmpDamageCoff / 100 + tmpDamageBase;

                // damage should not be 0.
                tmpDamage = Mathf.Max(tmpDamage, 1);

                tmpTarget.Hurt(this, tmpDamage, tmpResult);
            }

            return tmpResult;
        }
        
    }
}
