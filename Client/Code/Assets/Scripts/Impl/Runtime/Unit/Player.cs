using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class Player : Unit
    {

        public Player() : base()
        {
        }

        public new void Init(int unitID, int level, EUnitType unitType = EUnitType.EUT_OtherPlayer, ACT.EUnitCamp unitCamp = ACT.EUnitCamp.EUC_FRIEND)
        {
            base.Init(unitID, level, unitType, unitCamp);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            
        }

        public override void UpdateAttributes()
        {
            PlayerAttrBase tmpPlayerAttrBase = PlayerAttrBaseManager.instance.Find(UnitID, Level);

            if (null != tmpPlayerAttrBase)
            {
                mUnitAttr.Init(tmpPlayerAttrBase);
            }

        }

        public override void Hurt(Unit attacker, int damage,int dpDamage, ACT.ECombatResult result)
        {
            base.Hurt(attacker, damage, dpDamage, result);

            GetComponent<HudPopupComponent>().Popup(EHudPopupType.Damage, damage);
        }
    }
}
