using System;

namespace AosHotfixFramework
{
    /// <summary>
    /// 事件管理器。
    /// </summary>
    internal sealed class EventManager : GameModuleBase, IEventManager
    {
        private readonly EventPool<GameEventArgs> mEventPool;

        /// <summary>
        /// 初始化事件管理器的新实例。
        /// </summary>
        public EventManager()
        {
            mEventPool = new EventPool<GameEventArgs>();
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal override int Priority
        {
            get
            {
                return 100;
            }
        }

        /// <summary>
        /// 事件管理器轮询。
        /// </summary>
        internal override void Update(float deltaTime)
        {
            mEventPool.Update(deltaTime);
        }

        internal override void LateUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 关闭并清理事件管理器。
        /// </summary>
        internal override void Shutdown()
        {
            mEventPool.Shutdown();
        }

        /// <summary>
        /// 检查订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要检查的事件处理函数。</param>
        /// <returns>是否存在事件处理函数。</returns>
        public bool Check(int id, EventHandler<GameEventArgs> handler)
        {
            return mEventPool.Check(id, handler);
        }

        /// <summary>
        /// 订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要订阅的事件处理函数。</param>
        public void Subscribe(int id, EventHandler<GameEventArgs> handler)
        {
            mEventPool.Subscribe(id, handler);
        }

        /// <summary>
        /// 取消订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要取消订阅的事件处理函数。</param>
        public void Unsubscribe(int id, EventHandler<GameEventArgs> handler)
        {
            mEventPool.Unsubscribe(id, handler);
        }

        /// <summary>
        /// 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public void Fire(object sender, GameEventArgs e)
        {
            mEventPool.Fire(sender, e);
        }

        /// <summary>
        /// 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public void FireNow(object sender, GameEventArgs e)
        {
            mEventPool.FireNow(sender, e);
        }
    }
}
