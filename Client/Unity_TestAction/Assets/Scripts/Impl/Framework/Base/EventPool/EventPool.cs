using System;
using System.Collections.Generic;

namespace AosHotfixFramework
{
    /// <summary>
    /// 事件池。
    /// </summary>
    /// <typeparam name="T">事件类型。</typeparam>
    internal sealed partial class EventPool<T> where T : BaseEventArgs
    {
        private readonly Dictionary<int, EventHandler<T>> mEventHandlers;
        private readonly Queue<Event> mEvents;

        /// <summary>
        /// 初始化事件池的新实例。
        /// </summary>
        /// <param name="mode">事件池模式。</param>
        public EventPool()
        {
            mEventHandlers = new Dictionary<int, EventHandler<T>>();
            mEvents = new Queue<Event>();
        }

        /// <summary>
        /// 事件池轮询。
        /// </summary>
        public void Update(float deltaTime)
        {
            while (mEvents.Count > 0)
            {
                Event e = null;
                lock (mEvents)
                {
                    e = mEvents.Dequeue();
                }

                HandleEvent(e.Sender, e.EventArgs);
            }
        }

        /// <summary>
        /// 关闭并清理事件池。
        /// </summary>
        public void Shutdown()
        {
            Clear();
            mEventHandlers.Clear();
        }

        /// <summary>
        /// 清理事件。
        /// </summary>
        public void Clear()
        {
            lock (mEvents)
            {
                mEvents.Clear();
            }
        }

        /// <summary>
        /// 检查订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要检查的事件处理函数。</param>
        /// <returns>是否存在事件处理函数。</returns>
        public bool Check(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Logger.LogError("Event handler is invalid.");
            }

            EventHandler<T> handlers = null;
            if (!mEventHandlers.TryGetValue(id, out handlers))
            {
                return false;
            }

            if (handlers == null)
            {
                return false;
            }

            foreach (EventHandler<T> i in handlers.GetInvocationList())
            {
                if (i == handler)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要订阅的事件处理函数。</param>
        public void Subscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Logger.LogError("Event handler is invalid.");
            }

            EventHandler<T> eventHandler = null;
            if (!mEventHandlers.TryGetValue(id, out eventHandler) || eventHandler == null)
            {
                mEventHandlers[id] = handler;
            }
            else
            {
                eventHandler += handler;
                mEventHandlers[id] = eventHandler;
            }
        }

        /// <summary>
        /// 取消订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要取消订阅的事件处理函数。</param>
        public void Unsubscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Logger.LogError("Event handler is invalid.");
            }

            if (mEventHandlers.ContainsKey(id))
            {
                mEventHandlers[id] -= handler;
            }
        }

        /// <summary>
        /// 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public void Fire(object sender, T e)
        {
            Event eventNode = new Event(sender, e);
            lock (mEvents)
            {
                mEvents.Enqueue(eventNode);
            }
        }

        /// <summary>
        /// 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public void FireNow(object sender, T e)
        {
            HandleEvent(sender, e);
        }

        /// <summary>
        /// 处理事件结点。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        private void HandleEvent(object sender, T e)
        {
            EventHandler<T> handlers = null;
            if (mEventHandlers.TryGetValue(e.Id, out handlers))
            {
                if (handlers != null)
                {
                    handlers(sender, e);
                    e.Dispose();
                    return;
                }
            }
        }
    }
}
