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
        bool mAIEnable = false;

        public Monster(int unitID, bool aiEnable, int aiDiff) : base(unitID)
        {
            mAIEnable = aiEnable;
            AIDiff = aiDiff;
        }

        public override void Init()
        {
            base.Init();

            Camp = ACT.EUnitCamp.EUC_ENEMY;

            if (mAIEnable)
            {
                ActStatus.Bind(new ACT.AIListener(this));
            }
        }

        public override bool Hurt(Unit attacker, int damage, ACT.ECombatResult result)
        {
            return true;
        }
    }
}