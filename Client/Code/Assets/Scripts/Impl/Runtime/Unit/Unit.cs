using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public abstract class Unit : ACT.ActUnit
    {
        protected BuffManager mBuffManager;
        
        public BuffManager BuffManager { get { return mBuffManager; } }

        public override int CurHp { get { return GetAttrib(EPA.CurHP); } }
        public override int HpMax { get { return GetAttrib(EPA.HPMax); } }
        public override int Speed { get { return 300; } }
        public override bool CanHurt { get { return true; } }

        public Unit(int unitID)
        {
            UnitID = unitID;
        }

        public virtual void Init()
        {
            UnitBase tmpUnitBase = UnitBaseManager.instance.Find(UnitID);

            if (null == tmpUnitBase)
            {
                Logger.LogError($"找不到UnitBase ID -> {UnitID}");
                return;
            }

            Game.ResourcesMgr.LoadBundleByType(EABType.Unit, tmpUnitBase.Prefab);
            GameObject tmpGo = Game.ResourcesMgr.GetAssetByType<GameObject>(EABType.Unit, tmpUnitBase.Prefab);

            if (null == tmpGo)
            {
                Logger.LogError($"未能成功加载单位预制 prefab -> {tmpUnitBase.Prefab}");
                return;
            }

            tmpGo = Hotfix.Instantiate(tmpGo);
            InitActUnit(tmpGo, tmpGo.transform.Find("model"));
        }

        public virtual void LateUpdate(float deltaTime)
        {
        }

        public virtual void UpdateAttributes()
        {
        }

        public abstract bool Hurt(Unit attacker, int damage, ACT.ECombatResult result);
        public virtual void AddHp(int v) { }
        public virtual void AddSoul(int v) { }
        public virtual void AddAbility(int v) { }
        public virtual bool UpLevel() { return false; }
        public virtual int GetAttrib(EPA idx) { return 0; }
        public virtual void Equip() { }
        public virtual void Revive() { }

        public override ACT.ECombatResult Combat(ACT.IActUnit target, int damageCoff, int damageBase, bool skillAttack, int actionCoff)
        {
            int damage = 0;
            ACT.ECombatResult result = ACT.ECombatResult.ECR_Normal;

            if (result != ACT.ECombatResult.ECR_Block)
            {
                // 技能的影响。
                damage = damage * damageCoff / 100 + damageBase;

                // 动作的系数调整在最后面。
                damage = damage * actionCoff / 50;

                // damage should not be 0.
                damage = Mathf.Max(damage, 1);

                (target as Unit).Hurt(this, damage, result);
            }

            return result;
        }

        public virtual void AddBuff(int id)
        {
            mBuffManager.AddBuff(id);
        }
    }
}
