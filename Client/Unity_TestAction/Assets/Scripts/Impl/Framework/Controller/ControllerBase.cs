using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixFramework
{
    public abstract class ControllerBase
    {
        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Reset()
        {
        }
    }
}
