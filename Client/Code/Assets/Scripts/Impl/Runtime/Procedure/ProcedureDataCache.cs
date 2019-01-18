using System;
using System.Collections.Generic;

namespace AosHotfixRunTime
{
    public static class ProcedureDataCache
    {
        //切换的场景ID
        public static int ChangeSceneID { get; set; }
        public static string ChangeSceneName { get; set; }
        //下一流程
        public static EProcedureType NxProcedure { get; set; }
    }
}
