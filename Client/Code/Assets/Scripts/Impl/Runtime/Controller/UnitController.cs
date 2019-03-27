using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class UnitController : ControllerBase
    {
        public LocalPlayer LocalPlayer { get; private set; }

        public Monster CurrHitedMonster { get; private set; }

        public override void Reset()
        {
            base.Reset();
        }


        public void SetLocalPlayer(LocalPlayer localPlayer)
        {
            this.LocalPlayer = localPlayer;
        }

        public void SetHitedMonster(Monster monster)
        {
            if (CurrHitedMonster != monster || monster.Dead)
            {
                this.CurrHitedMonster = monster.Dead ? null : monster;

                var tmpEvent = ReferencePool.Fetch<UnitCtrlEvent.CurrHitedMonsterChange>();
                tmpEvent.Data = CurrHitedMonster;

                Game.EventMgr.FireNow(this, tmpEvent);
            }
        }
    }
}
