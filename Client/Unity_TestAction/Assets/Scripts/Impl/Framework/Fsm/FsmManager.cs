using System;
using System.Collections.Generic;

namespace AosHotfixFramework
{
    /// <summary>
    /// 有限状态机管理器。
    /// </summary>
    internal sealed class FsmManager : GameModuleBase, IFsmManager
    {
        private readonly Dictionary<Type, FsmBase> mFsms;
        private readonly List<FsmBase> mTempFsms;
        
        public FsmManager()
        {
            mFsms = new Dictionary<Type, FsmBase>();
            mTempFsms = new List<FsmBase>();
        }
        
        internal override int Priority
        {
            get
            {
                return 60;
            }
        }
        
        public int Count
        {
            get
            {
                return mFsms.Count;
            }
        }
        
        internal override void Update(float deltaTime)
        {
            mTempFsms.Clear();
            if (mFsms.Count <= 0)
            {
                return;
            }

            Dictionary<Type, FsmBase>.Enumerator tmpItor = mFsms.GetEnumerator();
            while (tmpItor.MoveNext())
            {
                mTempFsms.Add(tmpItor.Current.Value);
            }

            foreach (FsmBase fsm in mTempFsms)
            {
                if (fsm.IsDestroyed)
                {
                    continue;
                }

                fsm.Update(deltaTime);
            }
        }

        internal override void LateUpdate(float deltaTime)
        {
            mTempFsms.Clear();
            if (mFsms.Count <= 0)
            {
                return;
            }

            Dictionary<Type, FsmBase>.Enumerator tmpItor = mFsms.GetEnumerator();
            while (tmpItor.MoveNext())
            {
                mTempFsms.Add(tmpItor.Current.Value);
            }

            foreach (FsmBase fsm in mTempFsms)
            {
                if (fsm.IsDestroyed)
                {
                    continue;
                }

                fsm.LateUpdate(deltaTime);
            }
        }

        internal override void Shutdown()
        {
            Dictionary<Type, FsmBase>.Enumerator tmpItor = mFsms.GetEnumerator();
            while (tmpItor.MoveNext())
            {
                tmpItor.Current.Value.Shutdown();
            }

            mFsms.Clear();
            mTempFsms.Clear();
        }
        
        public bool HasFsm<T>() where T : class
        {
            return InternalHasFsm(typeof(T));
        }
        
        public IFsm<T> GetFsm<T>() where T : class
        {
            return (IFsm<T>)InternalGetFsm(typeof(T));
        }
        
        public FsmBase[] GetAllFsms()
        {
            int tmpIndex = 0;
            FsmBase[] tmpFsms = new FsmBase[mFsms.Count];
            Dictionary<Type, FsmBase>.Enumerator tmpItor = mFsms.GetEnumerator();
            while (tmpItor.MoveNext())
            {
                tmpFsms[tmpIndex++] = tmpItor.Current.Value;
            }

            return tmpFsms;
        }
        
        public IFsm<T> CreateFsm<T>(T owner) where T : class
        {
            IFsm<T> tmpFsm = GetFsm<T>();

            if (null == tmpFsm)
            {
                tmpFsm = new Fsm<T>(owner);
                mFsms.Add(typeof(T), (FsmBase)tmpFsm);
            }

            return tmpFsm;
        }


        public bool DestroyFsm<T>() where T : class
        {
            return InternalDestroyFsm(typeof(T));
        }
        
        public bool DestroyFsm(FsmBase fsm)
        {
            if (fsm == null)
            {
                Logger.LogError("FSM is invalid.");
            }

            return InternalDestroyFsm(fsm.OwnerType);
        }

        private bool InternalHasFsm(Type type)
        {
            return mFsms.ContainsKey(type);
        }

        private FsmBase InternalGetFsm(Type type)
        {
            FsmBase tmpFsm = null;
            if (mFsms.TryGetValue(type, out tmpFsm))
            {
                return tmpFsm;
            }

            return null;
        }

        private bool InternalDestroyFsm(Type type)
        {
            FsmBase tmpFsm = null;
            if (mFsms.TryGetValue(type, out tmpFsm))
            {
                tmpFsm.Shutdown();
                return mFsms.Remove(type);
            }

            return false;
        }
    }
}
