using System.Collections;
using System.Collections.Generic;
using System;
using AosBaseFramework;
using Object = UnityEngine.Object;

namespace AosHotfixFramework
{
    public abstract class TimerBase
    {
        public interface IInitData<A>
        {
            void InitData(A a);
        }
        public interface IInitData<A,B>
        {
            void InitData(A a,B b);
        }
        //是否开始计时
        protected bool mIsStart;
        public bool IsStart
        {
            get { return mIsStart; }
            set { mIsStart = value; }
        }

        protected virtual void Update(float deltaTime) { }

        internal void InternalUpdate(float deltaTime)
        {
            Update(deltaTime);
        }


    }
}

