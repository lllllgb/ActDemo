using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public enum EProcedureType
    {
        Invalid,
        ChangeScene,
        CheckVersion,
        Login,
        Main,
    }

    public enum EUnitType
    {
        EUT_NONE = 0,
        EUT_LocalPlayer = 1,
        EUT_OtherPlayer = 2,
        EUT_PVPPlayer = 3,
        EUT_Npc = 4,
        EUT_Monster = 5,
        EUT_MonsterItem = 6,
        EUT_Pet = 7,
        EUT_MAX,
    };

    //属性
    public enum EPA // player attribute
    {
        CurHP = 0, //当前血量
        MaxHP, //最大血量
        HPRestore, //血量恢复
        //------------------------------------------------
        CurMP, 
        MaxMP,
        MPRestore,
        //------------------------------------------------
        CurDP,
        MaxDP,
        DPRestore,
        //------------------------------------------------
        CurExp,
        EXPMax,
        //------------------------------------------------
        Level,
        //------------------------------------------------
        Attack,
        DpAttack,
        Defense,
        //------------------------------------------------
        Critical,
        Block,
        Tough,
        Hit,
        //------------------------------------------------
        MoveSpeed,
        //------------------------------------------------
        MAX,
    };

    //
    public enum EHudPopupType
    {
        Damage = 0, //伤害

        Max,
    }

    //BUFF特殊效果
    public enum EBuffSpecialEffect
    {
        None = 0,
        CanNotHurt = 1,
        CanMove = 2,
        CanRotate = 3,
        Max,
    }

    public static class ExtraParams
    {

        public static void Init()
        {
        }
    }
}
