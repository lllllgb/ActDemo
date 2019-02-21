using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class Monster : Unit
    {
        int mHP = 0;
        int mHPMax = 0;
        MainAttrib mMonsterAttrib;
        bool mIsBoss = false;
        string mName;

        public Monster(int unitID) : base(unitID)
        {
        }

        public override void Init()
        {
            base.Init();

            Camp = ACT.EUnitCamp.EUC_ENEMY;
        }

        public override bool Hurt(Unit attacker, int damage, ACT.ECombatResult result)
        {
            return true;
        }
    }
}