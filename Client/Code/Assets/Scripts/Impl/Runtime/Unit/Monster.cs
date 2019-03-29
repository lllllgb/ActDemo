using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class Monster : Unit, ACT.AIListener.IPlaySkillListener
    {
        bool mAIEnable = false;
        List<SkillItem> mSkillItems = new List<SkillItem>();

        public Monster() : base()
        {
        }

        public void Init(int unitID, int level,
            EUnitType unitType = EUnitType.EUT_Monster, ACT.EUnitCamp unitCamp = ACT.EUnitCamp.EUC_ENEMY,
            bool aiEnable = false, int aiDiff = 0)
        {
            base.Init(unitID, level, unitType, unitCamp);

            mAIEnable = aiEnable;
            AIDiff = aiDiff;

            if (mAIEnable)
            {
                ActStatus.Bind(new ACT.AIListener(this, this));
            }
        }

        public override void UpdateAttributes()
        {
            MonsterAttrBase tmpMonsterAttrBase = MonsterAttrBaseManager.instance.Find(UnitID, Level);

            if (null != tmpMonsterAttrBase)
            {
                mUnitAttr.Init(tmpMonsterAttrBase);
            }
        }

        public override void Hurt(Unit attacker, int damage, int dpDamage, ACT.ECombatResult result)
        {
            base.Hurt(attacker, damage, dpDamage, result);

            GetComponent<HudPopupComponent>().Popup(EHudPopupType.Damage, damage);
            Game.ControllerMgr.Get<UnitController>().SetHitedMonster(this);
        }

        public void PlayAISkill(int skillID)
        {
            SkillItem tmpSkillItem = null;

            for (int i = 0, max = mSkillItems.Count; i < max; ++i)
            {
                if (mSkillItems[i].ID == skillID)
                {
                    tmpSkillItem = mSkillItems[i];
                    break;
                }
            }

            if (null == tmpSkillItem && 0 != skillID)
            {
                if (SkillBaseManager.instance.Find(skillID) != null)
                {
                    tmpSkillItem = new SkillItem();
                    tmpSkillItem.Init(skillID, 1);
                    mSkillItems.Add(tmpSkillItem);
                }
            }

            ActStatus.SkillItem = tmpSkillItem;
        }
    }
}