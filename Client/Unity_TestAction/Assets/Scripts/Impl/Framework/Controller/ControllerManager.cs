using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixFramework
{
    internal sealed class ControllerManager : GameModuleBase, IControllerManager
    {
        List<ControllerBase> mControllers = new List<ControllerBase>();
       
        internal override int Priority
        {
            get
            {
                return 0;
            }
        }
        
        internal override void Update(float deltaTime)
        {
            for (int i = 0, max = mControllers.Count; i < max; ++i)
            {
                mControllers[i].Update(deltaTime);
            }
        }

        internal override void LateUpdate(float deltaTime)
        {
        }

        internal override void Shutdown()
        {
            for (int i = 0, max = mControllers.Count; i < max; ++i)
            {
                mControllers[i].Reset();
            }

            mControllers.Clear();
        }

        public T Get<T>() where T : ControllerBase
        {
            T tmpController = default(T);

            for (int i = 0, max = mControllers.Count; i < max; ++i)
            {
                if (mControllers[i] is T)
                {
                    tmpController = mControllers[i] as T;
                    break;
                }
            }

            if (null == tmpController)
            {
                tmpController = System.Activator.CreateInstance<T>();
                mControllers.Add(tmpController);
            }

            return tmpController;
        }
    }
}
