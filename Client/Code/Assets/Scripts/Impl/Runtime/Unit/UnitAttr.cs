using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class UnitAttr
    {
        private Dictionary<EPA, int> mType2ValueDcit = new Dictionary<EPA, int>();

        public int Get(EPA epa)
        {
            int tmpResult = 0;
            mType2ValueDcit.TryGetValue(epa, out tmpResult);

            return tmpResult;
        }

        public void Init(PlayerAttrBase playerAttr)
        {
            mType2ValueDcit[EPA.CurHP] = playerAttr.MaxHp;
            mType2ValueDcit[EPA.MaxHP] = playerAttr.MaxHp;
            mType2ValueDcit[EPA.HPRestore] = playerAttr.HpRestore;
            mType2ValueDcit[EPA.CurMP] = playerAttr.MaxMp;
            mType2ValueDcit[EPA.MaxMP] = playerAttr.MaxMp;
            mType2ValueDcit[EPA.MPRestore] = playerAttr.MpRestore;
            mType2ValueDcit[EPA.CurDP] = playerAttr.MaxDp;
            mType2ValueDcit[EPA.MaxDP] = playerAttr.MaxDp;
            mType2ValueDcit[EPA.DPRestore] = playerAttr.DpRestore;
            mType2ValueDcit[EPA.Attack] = playerAttr.Attack;
            mType2ValueDcit[EPA.Defense] = playerAttr.Defense;
            mType2ValueDcit[EPA.MoveSpeed] = playerAttr.Speed;
        }

        public void Init(MonsterAttrBase monsterAttr)
        {
            mType2ValueDcit[EPA.CurHP] = monsterAttr.MaxHp;
            mType2ValueDcit[EPA.MaxHP] = monsterAttr.MaxHp;
            mType2ValueDcit[EPA.HPRestore] = monsterAttr.HpRestore;
            mType2ValueDcit[EPA.CurDP] = monsterAttr.MaxDp;
            mType2ValueDcit[EPA.MaxDP] = monsterAttr.MaxDp;
            mType2ValueDcit[EPA.DPRestore] = monsterAttr.DpRestore;
            mType2ValueDcit[EPA.Attack] = monsterAttr.Attack;
            mType2ValueDcit[EPA.Defense] = monsterAttr.Defense;
            mType2ValueDcit[EPA.MoveSpeed] = monsterAttr.Speed;
        }
    }
}
