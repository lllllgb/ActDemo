using System;

namespace AosHotfixFramework
{
    /// <summary>
    /// 游戏框架模块抽象类。
    /// </summary>
    internal abstract class GameModuleBase
    {
        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal virtual int Priority
        {
            get
            {
                return 0;
            }
        }
        
        internal abstract void Update(float deltaTime);

        internal abstract void LateUpdate(float deltaTime);

        /// <summary>
        /// 关闭并清理游戏框架模块。
        /// </summary>
        internal abstract void Shutdown();
    }
}
