using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixFramework
{
    public interface IControllerManager
    {

        T Get<T>() where T : ControllerBase;
    }
}
