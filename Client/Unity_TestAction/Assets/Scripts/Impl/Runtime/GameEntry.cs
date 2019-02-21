using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public static class GameEntry
    {

        public static void Initialize()
        {
            Game.Init();
        }

        public static void Update()
        {
            GameModuleManager.Update(Time.deltaTime);
        }

        public static void LateUpdate()
        {
            GameModuleManager.LateUpdate(Time.deltaTime);
        }

        public static void FixedUpdate()
        {
        }

        public static void Close()
        {
        }

        public static void ApplicationQuit()
        {
            GameModuleManager.Shutdown();
        }
    }
}
