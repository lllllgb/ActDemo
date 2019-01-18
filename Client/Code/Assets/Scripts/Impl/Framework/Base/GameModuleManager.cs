using System;
using System.Collections.Generic;

namespace AosHotfixFramework
{
    public static class GameModuleManager
    {
        private static readonly LinkedList<GameModuleBase> s_GameFrameworkModules = new LinkedList<GameModuleBase>();

        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        public static void Update(float deltaTime)
        {
            foreach (GameModuleBase module in s_GameFrameworkModules)
            {
                module.Update(deltaTime);
            }
        }

        public static void LateUpdate(float deltaTime)
        {
            foreach (GameModuleBase module in s_GameFrameworkModules)
            {
                module.LateUpdate(deltaTime);
            }
        }

        /// <summary>
        /// 关闭并清理所有游戏框架模块。
        /// </summary>
        public static void Shutdown()
        {
            for (LinkedListNode<GameModuleBase> current = s_GameFrameworkModules.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            s_GameFrameworkModules.Clear();
            ReferencePool.ClearAll();
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架模块类型。</typeparam>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        public static T GetModule<T>() where T : class
        {
            foreach (GameModuleBase module in s_GameFrameworkModules)
            {
                if (module is T)
                {
                    return module as T;
                }
            }

            return null;
        }

        /// <summary>
        /// 创建游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要创建的游戏框架模块类型。</param>
        /// <returns>要创建的游戏框架模块。</returns>
        public static void CreateModule<T>() where T : class
        {
            GameModuleBase module = Activator.CreateInstance<T>() as GameModuleBase;
            if (module == null)
            {
                Logger.LogError(string.Format("Can not create module '{0}'.", module.GetType().FullName));
                return;
            }

            LinkedListNode<GameModuleBase> current = s_GameFrameworkModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                s_GameFrameworkModules.AddBefore(current, module);
            }
            else
            {
                s_GameFrameworkModules.AddLast(module);
            }
        }
    }
}
