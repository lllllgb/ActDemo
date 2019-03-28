using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class Monster : Unit
    {
        bool mAIEnable = false;

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
                ActStatus.Bind(new ACT.AIListener(this));
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
    }
}