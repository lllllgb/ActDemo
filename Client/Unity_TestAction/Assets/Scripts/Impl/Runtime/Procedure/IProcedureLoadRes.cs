using System;
using System.Collections.Generic;

namespace AosHotfixRunTime
{
    public interface IProcedureLoadRes
    {
        void PreLoadRes();

        int LoadProgress();
    }
}
