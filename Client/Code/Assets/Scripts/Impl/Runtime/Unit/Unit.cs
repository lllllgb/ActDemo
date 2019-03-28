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
        //阴影
        protected UnitShadow mUnitShadow = new UnitShadow();


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

            Name = tmpUnitBase.Name;
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
            mUnitShadow.Init(this, Utility.GameObj.Find(tmpGo, "shadow"));
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
            mUnitShadow.Update(deltaTime);
        }

        public virtual void LateUpdate(float deltaTime)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            ACT.ActionSystem.Instance.ActUnitMgr.Remove(this);
            GameObject.Destroy(UGameObject);
        }

        //更新属性
        public abstract void UpdateAttributes();

        //名字
        public void SetName(string name)
        {
            Name = name;
        }

        public virtual void Hurt(Unit attacker, int damage, ACT.ECombatResult result)
        {
            if (Dead)
            {
                return;
            }

            AddHp(-damage);
        }

        public virtual void AddHp(int v)
        {
            if (Dead)
            {
                return;
            }

            int tmpCurrHp = Mathf.Clamp(mUnitAttr.Get(EPA.CurHP) + v, 0, mUnitAttr.Get(EPA.MaxHP));
            mUnitAttr.Set(EPA.CurHP, tmpCurrHp);

            if (tmpCurrHp <= 0)
            {
                SetIsDead(true);
            }

            var tmpEvent = ReferencePool.Fetch<UnitEvent.HpModify>();
            tmpEvent.Data = this;
            Game.EventMgr.FireNow(this, tmpEvent);
        }

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
                if (null != skillItem)
                {
                    int tmpDamageCoff = (skillItem as SkillItem).SkillAttrBase.DamageCoff;
                    int tmpDamageBase = (skillItem as SkillItem).SkillAttrBase.DamageBase;
                    // 技能的影响。
                    tmpDamage = tmpDamage * tmpDamageCoff / 100 + tmpDamageBase;
                }

                // damage should not be 0.
                tmpDamage = Mathf.Max(tmpDamage, 1);

                tmpTarget.Hurt(this, tmpDamage, tmpResult);
            }

            return tmpResult;
        }
        
    }
}
