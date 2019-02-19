using UnityEngine;
using System.Collections;

namespace ACT
{
    public enum EKeyList
    {
        KL_Attack = 0,
        KL_SubAttack,
        KL_SkillAttack,
        KL_AuxKey,

        KL_Grab,
        KL_Jump,
        KL_Move,
        KL_CameraUp,
        KL_CameraDown,
        KL_LastKey,
        KL_Max,
    };

    /// EOperation
    public enum EOperation
    {
        EO_None = 0,
        EO_Attack,
        EO_SpAttack,
        EO_Skill,
        EO_Move,
        EO_Jump,
        EO_Grab,
        EO_Front,
        EO_Back,
        EO_Last,
        EO_Auxiliary,
    };

    /// EInputType
    public enum EInputType
    {
        EIT_Click = 0,
        EIT_DoubleClick,
        EIT_Press,
        EIT_Release,
        EIT_Pressing,
        EIT_Releasing,
    };

    public enum EUnitCamp
    {
        EUC_NONE = 0,       // ÖÐÁ¢
        EUC_FRIEND = 1,     // ÅóÓÑ
        EUC_ENEMY = 2,      // µÐÈË
    };

    public enum EUnitState
    {
        Normal = 0,
        Die = 1,
    };

    public enum ECompareType
    {
        ECT_EQUAL = 0, // µÈÓÚ£š==£©
        ECT_NOT_EUQAL = 1, // ²»µÈÓÚ£š£¡=£©
        ECT_GREATER = 2, // ŽóÓÚ£š>£©
        ECT_LESS = 3, // Ð¡ÓÚ£š<£©
        ECT_GREATER_EQUAL = 4, // ŽóÓÚ»òµÈÓÚ£š>=£©
        ECT_LESS_EQUAL = 5, // Ð¡ÓÚ»òµÈÓÚ£š<=£©
    };

    public enum EVariableIdx
    {
        EVI_HP = 0,
        EVI_HPPercent = 1,
        EVI_Level = 2,
        EVI_Custom = 3,
        EVI_CustomMax = 20,
    };


    public enum ECombatResult
    {
        ECR_Normal,
        ECR_Block,
        ECR_Critical,
    };

    public struct HitData
    {
        public long TargetUUID;
        public short HitX;
        public short HitY;
        public short HitZ;
        public short HitDir;
        public short StraightTime;
        public short LashX;
        public short LashY;
        public short LashZ;
        public short LashTime;
        public byte HitAction;
        public byte AttackLevel;

        public void Serialize()
        {
        }
    }
}