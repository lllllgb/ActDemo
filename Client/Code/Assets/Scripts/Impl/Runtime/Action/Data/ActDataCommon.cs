using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ActData
{
    [Flags]
    public enum HeightStatusFlag
    {
        None = 0,
        Stand = 1,
        Ground = 2,
        LowAir = 4,
        HighAir = 8,
        Hold = 16,
    }

    public enum EActionState
    {
        Idle = 0,
        Move = 1,
        Attack = 2,
        Hit = 3,
        Defense = 4,
    }

    public enum EAirStatus
    {
        Normal = 0,
        Diaup = 1,
    }

    public class CommonAction
    {
        public const string Show = "S1000";//Show in CreatRole
        public const string ShowEquip = "S2000";//Show in Store And Info
        public const string Idle = "idle"; // Idle in fight;
        public const string IdleInTown = "N1000";//Idle in town;
        public const string Run = "run"; //run in Fight;
        public const string RunInTown = "N1010";
        public const string Revive = "H0100";
        public const string Bounce = "hit_s";
        public const string DieStand = "H1040";//stand Dying;
        public const string DieDown = "H1041"; // Lay Dying;
        public const string Knockout = "H0010"; //
        public const string KnockBack = "H0020";//
    }
}
