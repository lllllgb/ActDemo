﻿using System.Collections;
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
        CurHP = 0,
        HPMax,
        HPRestore,
        //------------------------------------------------
        CurSoul,
        SoulMax,
        SoulRestore,
        //------------------------------------------------
        CurAbility,
        AbilityMax,
        AbRestore,
        AbHitAdd,
        //------------------------------------------------
        CurExp,
        EXPMax,
        //------------------------------------------------
        Level,
        //------------------------------------------------
        Damage,
        Defense,
        //------------------------------------------------
        SpecialDamage,
        SpecialDefense,
        //------------------------------------------------
        Critical,
        Block,
        Hit,
        Tough,
        //------------------------------------------------
        MoveSpeed,
        //------------------------------------------------
        FastRate,
        StiffAdd,
        StiffSub,
        //------------------------------------------------
        MAX,
    };

    //BUFF特殊效果
    public enum EBuffSpecialEffect
    {
        None = 0,
        CanNotHurt = 1,
        CanMove = 2,
        CanRotate = 3,
        Max,
    }


    public static class GameDefine
    {
        public static Camera CurrentCamera { get; set; }
    }

    public static class ExtraParams
    {

        public static void Init()
        {
        }
    }
}