using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    internal class ActionManager : GameModuleBase
    {

        internal override int Priority => base.Priority;

        internal override void Update(float deltaTime)
        {
            ACT.ActionSystem.Instance.Update(deltaTime);
        }

        internal override void LateUpdate(float deltaTime)
        {
        }

        internal override void Shutdown()
        {
        }
    }
}
