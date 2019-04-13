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
        public override int Speed { get { return GetAttr(EPA.MoveSpeed); } }
        public override bool CanHurt { get { return true; } }
        public override bool IsPabodyState { get { return mIsPabodyState; } }

        public EUnitType UnitType { get; private set; }
        public string Name { get; private set; }
        public Transform TopNode { get; private set; }
        public Transform MidNode { get; private set; }
        public Transform BottomNode { get; private set; }
        //属性
        protected UnitAttr mUnitAttr = new UnitAttr();
        //阴影
        protected UnitShadow mUnitShadow = new UnitShadow();
        //恢复时间
        const float RestoreTime = 1f;
        float mRestoreLeft = 0.0f;
        //霸体状态
        bool mIsPabodyState;
        //
        UnitRenderers mUnitRenderer;

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
            mUnitRenderer = tmpGo.GetComponent<UnitRenderers>();
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
            UpdateRestore(deltaTime);
            UpdatePabodyState(deltaTime);
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

        //更新恢复
        private void UpdateRestore(float deltaTime)
        {
            if (!Dead && 0 != mUnitAttr.Get(EPA.HPRestore))
            {
                
            }

            if (Dead)
            {
                return;
            }

            mRestoreLeft += deltaTime;

            if (mRestoreLeft < RestoreTime)
            {
                return;
            }

            mRestoreLeft = 0;

            if (0 != mUnitAttr.Get(EPA.HPRestore) && mUnitAttr.Get(EPA.CurHP) < mUnitAttr.Get(EPA.MaxHP))
            {
                AddHp(mUnitAttr.Get(EPA.HPRestore));
            }

            if (0 != mUnitAttr.Get(EPA.DPRestore) && mUnitAttr.Get(EPA.CurDP) < mUnitAttr.Get(EPA.MaxDP))
            {
                AddDp(mUnitAttr.Get(EPA.DPRestore));
            }
        }

        //更新霸体状态
        private void UpdatePabodyState(float deltaTime)
        {
            if (ActStatus.ActionState != ActData.EActionState.Hit)
            {
                if (mUnitAttr.Get(EPA.CurDP) > 0 || ActStatus.ActiveAction.SuperArmor)
                {
                    SetIsPabodyState(true);
                }
                else
                {
                    SetIsPabodyState(false);
                }
            }
        }

        //是否霸体
        protected virtual void SetIsPabodyState(bool flag)
        {
            if (mIsPabodyState != flag)
            {
                //if (UnitType == EUnitType.EUT_Monster)
                //{
                //    Logger.Log($" name-> {Name} Pabody->{flag}");
                //}
                mIsPabodyState = flag;

                if (mUnitRenderer)
                {
                    mUnitRenderer.EnableRim(mIsPabodyState);
                }

                if (!flag)
                {
                    Game.EffectMgr.PlayEffect("E_pojia_01", 1, MidNode, Vector3.zero, Quaternion.identity);
                }
            }
        }

        

        public virtual void Hurt(Unit attacker, int damage, int dpDamage, ACT.ECombatResult result)
        {
            if (Dead)
            {
                return;
            }
            
            AddHp(-damage);

            if (0 != dpDamage)
            {
                AddDp(-dpDamage);
            }
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

        protected override void OnDead()
        {
            base.OnDead();

            var tmpEvent = ReferencePool.Fetch<UnitEvent.Dead>();
            tmpEvent.Data = this;
            Game.EventMgr.FireNow(this, tmpEvent);
        }

        public virtual void AddDp(int v)
        {
            if (Dead)
            {
                return;
            }

            int tmpMaxDp = mUnitAttr.Get(EPA.MaxDP);
            int tmpCurrDp = Mathf.Clamp(mUnitAttr.Get(EPA.CurDP) + v, 0, tmpMaxDp);
            mUnitAttr.Set(EPA.CurDP, tmpCurrDp);

            if (tmpCurrDp <= 0)
            {
                SetIsPabodyState(false);
            }
            else if (tmpCurrDp >= tmpMaxDp)
            {
                SetIsPabodyState(true);
            }

            var tmpEvent = ReferencePool.Fetch<UnitEvent.DpModify>();
            tmpEvent.Data = this;
            Game.EventMgr.FireNow(this, tmpEvent);
        }

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
            int tmpDpDamage = 0;
            Unit tmpTarget = target as Unit;
            ACT.ECombatResult tmpResult = ACT.ECombatResult.ECR_Normal;

            if (tmpTarget.ActStatus.ActionState == ActData.EActionState.Defense)
            {
                tmpResult = ACT.ECombatResult.ECR_Block;
            }
            else
            {
                tmpResult = MathUtility.Combat(this, tmpTarget, out tmpDamage);
            }

            if (ACT.ECombatResult.ECR_Block != tmpResult)
            {
                if (null != skillItem)
                {
                    int tmpDamageCoff = (skillItem as SkillItem).SkillAttrBase.DamageCoff;
                    int tmpDamageBase = (skillItem as SkillItem).SkillAttrBase.DamageBase;
                    // 技能的影响。
                    tmpDamage = (int)(tmpDamage * tmpDamageCoff * 0.01f + tmpDamageBase);

                    int tmpDpDamageCoff = (skillItem as SkillItem).SkillAttrBase.DpDamageCoff;
                    tmpDpDamage = (int)(tmpTarget.GetAttr(EPA.DpAttack) * tmpDpDamageCoff * 0.01f);
                }

                // damage should not be 0.
                tmpDamage = Mathf.Max(tmpDamage, 1);

                tmpTarget.Hurt(this, tmpDamage, tmpDpDamage, tmpResult);
            }
            else
            {
                target.Combat(this, target.ActStatus.SkillItem);
            }

            return tmpResult;
        }
        
    }
}
