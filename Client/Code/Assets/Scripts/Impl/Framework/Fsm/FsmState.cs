using System;
using System.Collections.Generic;

namespace AosHotfixFramework
{
    /// <summary>
    /// 有限状态机状态基类。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    public abstract class FsmState<T> where T : class
    {
        private readonly Dictionary<int, FsmEventHandler<T>> mEventHandlers;

        /// <summary>
        /// 初始化有限状态机状态基类的新实例。
        /// </summary>
        public FsmState()
        {
            mEventHandlers = new Dictionary<int, FsmEventHandler<T>>();
        }

        /// <summary>
        /// 有限状态机状态初始化时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnInit(IFsm<T> fsm)
        {

        }

        /// <summary>
        /// 有限状态机状态进入时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnEnter(IFsm<T> fsm)
        {

        }

        /// <summary>
        /// 有限状态机状态轮询时调用。
        /// </summary>
        protected internal virtual void OnUpdate(IFsm<T> fsm, float deltaTime)
        {

        }

        protected internal virtual void OnLateUpdate(IFsm<T> fsm, float deltaTime)
        {

        }

        /// <summary>
        /// 有限状态机状态离开时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="isShutdown">是否是关闭有限状态机时触发。</param>
        protected internal virtual void OnLeave(IFsm<T> fsm, bool isShutdown)
        {

        }

        /// <summary>
        /// 有限状态机状态销毁时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        protected internal virtual void OnDestroy(IFsm<T> fsm)
        {
            mEventHandlers.Clear();
        }

        /// <summary>
        /// 订阅有限状态机事件。
        /// </summary>
        /// <param name="eventId">事件编号。</param>
        /// <param name="eventHandler">有限状态机事件响应函数。</param>
        protected void SubscribeEvent(int eventId, FsmEventHandler<T> eventHandler)
        {
            if (eventHandler == null)
            {
                Logger.LogError("Event handler is invalid.");
            }

            if (!mEventHandlers.ContainsKey(eventId))
            {
                mEventHandlers[eventId] = eventHandler;
            }
            else
            {
                mEventHandlers[eventId] += eventHandler;
            }
        }

        /// <summary>
        /// 取消订阅有限状态机事件。
        /// </summary>
        /// <param name="eventId">事件编号。</param>
        /// <param name="eventHandler">有限状态机事件响应函数。</param>
        protected void UnsubscribeEvent(int eventId, FsmEventHandler<T> eventHandler)
        {
            if (eventHandler == null)
            {
                Logger.LogError("Event handler is invalid.");
            }

            if (mEventHandlers.ContainsKey(eventId))
            {
                mEventHandlers[eventId] -= eventHandler;
            }
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        /// <param name="fsm">有限状态机引用。</param>
        protected void ChangeState<TState>(IFsm<T> fsm) where TState : FsmState<T>
        {
            Fsm<T> fsmImplement = (Fsm<T>)fsm;
            if (fsmImplement == null)
            {
                Logger.LogError("FSM is invalid.");
                return;
            }

            fsmImplement.ChangeState<TState>();
        }

        protected void ChangeState(IFsm<T> fsm, Type stateType)
        {
            Fsm<T> fsmImplement = (Fsm<T>)fsm;
            if (fsmImplement == null)
            {
                Logger.LogError("FSM is invalid.");
                return;
            }

            if (stateType == null)
            {
                Logger.LogError("State type is invalid.");
                return;
            }

            if (!typeof(FsmState<T>).IsAssignableFrom(stateType))
            {
                Logger.LogError($"State type '{ stateType.FullName}' is invalid.");
                return;
            }

            fsmImplement.ChangeState(stateType);
        }

        /// <summary>
        /// 响应有限状态机事件时调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        internal void OnEvent(IFsm<T> fsm, object sender, int eventId, object userData)
        {
            FsmEventHandler<T> eventHandlers = null;
            if (mEventHandlers.TryGetValue(eventId, out eventHandlers))
            {
                if (eventHandlers != null)
                {
                    eventHandlers(fsm, sender, userData);
                }
            }
        }
    }
}
