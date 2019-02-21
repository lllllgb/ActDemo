using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace AosBaseFramework
{
    public class AppFacade
    {

        private static AppFacade msInstance;
        public static AppFacade Instance
        {
            get
            {
                if (null == msInstance)
                {
                    msInstance = new AppFacade();
                }

                return msInstance;
            }
        }
        
        private IStaticMethod mUpdateMethod;
        public IStaticMethod UpdateMethod { set { mUpdateMethod = value; } }

        private IStaticMethod mLateUpdateMethod;
        public IStaticMethod LateUpdateMethod { set { mLateUpdateMethod = value; } }

        private IStaticMethod mFixedUpdateMethod;
        public IStaticMethod FixedUpdateMethod { set { mFixedUpdateMethod = value; } }

        private IStaticMethod mCloseMethod;
        public IStaticMethod CloseMethod { set { mCloseMethod = value; } }

        private IStaticMethod mApplicationQuitMethod;
        public IStaticMethod ApplicationQuitMethod { set { mApplicationQuitMethod = value; } }

        public void StartUp()
        {
            Game.Initialize();
        }

        public void OnUpdate()
        {
            Game.Update();

            if (null != mUpdateMethod)
            {
                mUpdateMethod.Run();
            }
        }

        public void OnLateUpdate()
        {
            Game.LateUpdate();

            if (null != mLateUpdateMethod)
            {
                mLateUpdateMethod.Run();
            }
        }

        public void OnFixedUpdate()
        {
            Game.FixedUpdate();

            if (null != mFixedUpdateMethod)
            {
                mFixedUpdateMethod.Run();
            }
        }

        public void Close()
        {
            Game.Close();

            if (null != mCloseMethod)
            {
                mCloseMethod.Run();
            }
        }

        public void OnApplicationQuit()
        {
            Game.ApplicationQuit();

            if (null != mApplicationQuitMethod)
            {
                mApplicationQuitMethod.Run();
            }
        }

    }
}
