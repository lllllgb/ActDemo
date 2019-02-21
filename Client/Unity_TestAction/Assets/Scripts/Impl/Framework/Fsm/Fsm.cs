using System;
using System.Collections.Generic;
using AosBaseFramework;

namespace AosHotfixFramework
{
    /// <summary>
    /// 有限状态机。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    internal sealed class Fsm<T> : FsmBase, IFsm<T> where T : class
    {
        private readonly T mOwner;
        private readonly Dictionary<Type, FsmState<T>> mStates;
        private FsmState<T> mCurrentState;
        private float mCurrentStateTime;
        private bool mIsDestroyed;

        public Fsm(T owner) : this(string.Empty, owner)
        {
        }
        /// <summary>
        /// 初始化有限状态机的新实例。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        public Fsm(string name, T owner, params FsmState<T>[] states)
            : base(name)
        {
            if (owner == null)
            {
                Logger.LogError("FSM owner is invalid.");
            }

            mOwner = owner;
            mStates = new Dictionary<Type, FsmState<T>>();
            mCurrentStateTime = 0f;
            mCurrentState = null;
            mIsDestroyed = false;
        }

        /// <summary>
        /// 获取有限状态机持有者。
        /// </summary>
        public T Owner
        {
            get
            {
                return mOwner;
            }
        }

        /// <summary>
        /// 获取有限状态机持有者类型。
        /// </summary>
        public override Type OwnerType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// 获取有限状态机中状态的数量。
        /// </summary>
        public override int FsmStateCount
        {
            get
            {
                return mStates.Count;
            }
        }

        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public override bool IsRunning
        {
            get
            {
                return mCurrentState != null;
            }
        }

        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public override bool IsDestroyed
        {
            get
            {
                return mIsDestroyed;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态。
        /// </summary>
        public FsmState<T> CurrentState
        {
            get
            {
                return mCurrentState;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        public override string CurrentStateName
        {
            get
            {
                return mCurrentState != null ? mCurrentState.GetType().FullName : null;
            }
        }

        /// <summary>
        /// 获取当前有限状态机状态持续时间。
        /// </summary>
        public override float CurrentStateTime
        {
            get
            {
                return mCurrentStateTime;
            }
        }

        public void AddState<TState>() where TState : FsmState<T>
        {
            Type tmpStateType = typeof(TState);

            if (!mStates.ContainsKey(tmpStateType))
            {
                TState tmpState = Activator.CreateInstance<TState>();
                mStates.Add(tmpStateType, tmpState);
                tmpState.OnInit(this);
            }
        }

        /// <summary>
        /// 开始有限状态机。
        /// </summary>
        /// <typeparam name="TState">要开始的有限状态机状态类型。</typeparam>
        public void Start<TState>() where TState : FsmState<T>
        {
            if (IsRunning)
            {
                Logger.LogError("FSM is running, can not start again.");
            }

            FsmState<T> tmpState = GetState<TState>();
            if (tmpState == null)
            {
                Logger.LogError(string.Format("FSM '{0}' can not start state '{1}' which is not exist.", typeof(T).FullName, typeof(TState).FullName));
            }

            mCurrentStateTime = 0f;
            mCurrentState = tmpState;
            mCurrentState.OnEnter(this);
        }

        /// <summary>
        /// 是否存在有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要检查的有限状态机状态类型。</typeparam>
        /// <returns>是否存在有限状态机状态。</returns>
        public bool HasState<TState>() where TState : FsmState<T>
        {
            return mStates.ContainsKey(typeof(TState));
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要获取的有限状态机状态类型。</typeparam>
        /// <returns>要获取的有限状态机状态。</returns>
        public TState GetState<TState>() where TState : FsmState<T>
        {
            FsmState<T> tmpState = null;
            if (mStates.TryGetValue(typeof(TState), out tmpState))
            {
                return (TState)tmpState;
            }

            return null;
        }

        public FsmState<T> GetStateByType(Type stateType)
        {
            FsmState<T> tmpState = null;

            if (mStates.TryGetValue(stateType, out tmpState))
            {
                return tmpState;
            }

            return null;
        }

        /// <summary>
        /// 获取有限状态机的所有状态。
        /// </summary>
        /// <returns>有限状态机的所有状态。</returns>
        public FsmState<T>[] GetAllStates()
        {
            int tmpIndex = 0;
            FsmState<T>[] tmpAllStates = new FsmState<T>[mStates.Count];
            foreach (var state in mStates)
            {
                tmpAllStates[tmpIndex++] = state.Value;
            }

            return tmpAllStates;
        }

        /// <summary>
        /// 抛出有限状态机事件。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        public void FireEvent(object sender, int eventId)
        {
            if (mCurrentState == null)
            {
                Logger.LogError("Current state is invalid.");
            }

            mCurrentState.OnEvent(this, sender, eventId, null);
        }

        /// <summary>
        /// 抛出有限状态机事件。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void FireEvent(object sender, int eventId, object userData)
        {
            if (mCurrentState == null)
            {
                Logger.LogError("Current state is invalid.");
            }

            mCurrentState.OnEvent(this, sender, eventId, userData);
        }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        internal override void Update(float deltaTime)
        {
            if (mCurrentState == null)
            {
                return;
            }

            mCurrentStateTime += deltaTime;
            mCurrentState.OnUpdate(this, deltaTime);
        }

        internal override void LateUpdate(float deltaTime)
        {
            if (mCurrentState == null)
            {
                return;
            }
            
            mCurrentState.OnLateUpdate(this, deltaTime);
        }

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        internal override void Shutdown()
        {
            if (mCurrentState != null)
            {
                mCurrentState.OnLeave(this, true);
                mCurrentState = null;
                mCurrentStateTime = 0f;
            }

            foreach (var state in mStates)
            {
                state.Value.OnDestroy(this);
            }

            mStates.Clear();

            mIsDestroyed = true;
        }

        /// <summary>
        /// 切换当前有限状态机状态。
        /// </summary>
        /// <typeparam name="TState">要切换到的有限状态机状态类型。</typeparam>
        internal void ChangeState<TState>() where TState : FsmState<T>
        {
            ChangeState(typeof(TState));
        }

        internal void ChangeState(Type stateType)
        {
            if (mCurrentState == null)
            {
                Logger.LogError("Current state is invalid.");
            }

            FsmState<T> state = GetStateByType(stateType);
            if (state == null)
            {
                Logger.LogError(string.Format("FSM '{0}' can not change state to '{1}' which is not exist.", Utility.Text.GetFullName<T>(Name), stateType.FullName));
            }

            mCurrentState.OnLeave(this, false);
            mCurrentStateTime = 0f;
            mCurrentState = state;
            mCurrentState.OnEnter(this);
        }
    }
}
