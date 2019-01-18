using System;
using System.Collections.Generic;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public static class ProcedureHelper
    {

        public static Type GetProcedureByType(EProcedureType type)
        {
            Type tmpProcedureType = null;

            switch (type)
            {
                case EProcedureType.CheckVersion:
                    tmpProcedureType = typeof(ProcedureCheckVersion);
                    break;
                case EProcedureType.Login:
                    tmpProcedureType = typeof(ProcedureLogin);
                    break;
                case EProcedureType.Main:
                    tmpProcedureType = typeof(ProcedureMain);
                    break;
                default:
                    Logger.LogError($"无效的流程! type = {type}");
                    break;
            }

            return tmpProcedureType;
        }
    }
}
