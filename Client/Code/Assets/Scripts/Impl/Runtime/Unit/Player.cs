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
            
        }

        public override bool Hurt(Unit attacker, int damage, ACT.ECombatResult result)
        {
            return true;
        }

        public override int GetAttrib(EPA idx)
        {
            int ret = 0;

            return ret;
        }
    }
}
