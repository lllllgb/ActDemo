using System.Collections;
using System.Collections.Generic;
using System;

namespace AosHotfixFramework
{
    internal sealed class TimerManager : GameModuleBase, ITimerManager
    {
        internal override int Priority
        {
            get
            {
                return 0;
            }
        }

        internal override void Update(float deltaTime)
        {
            UpdateTimer(deltaTime);
            
            for (int i = 0, max = mGameTimer.Count; i < max; ++i)
            {
                TimerBase tmpTimer = mGameTimer[i];
                if (tmpTimer.IsStart)
                {
                    tmpTimer.InternalUpdate(deltaTime);
                }                
            }
        }

        internal override void LateUpdate(float deltaTime)
        {
        }

        internal override void Shutdown()
        {
            smTimerItems.Clear();
            ClearTimer();
        }

        class TimerItem
        {
            public float delay;
            public float time;
            public Action<object> handle;
            public object handleArg;
        }

        static Queue<TimerItem> smTimerItems = new Queue<TimerItem>();
        static TimerItem SpawnTimerItem()
        {
            TimerItem tmpTimerItem;

            if (smTimerItems.Count > 0)
                tmpTimerItem = smTimerItems.Dequeue();
            else
                tmpTimerItem = Activator.CreateInstance<TimerItem>();

            return tmpTimerItem;
        }

        static void DespawnTimerItem(TimerItem elem)
        {
            if (null == elem)
                return;

            elem.handle = null;
            elem.handleArg = null;
            smTimerItems.Enqueue(elem);
        }

        List<TimerItem> mTimerList = new List<TimerItem>();

        public void AddTimer(float delay, Action<object> handle, object obj)
        {
            if (null == mTimerList)
                mTimerList = new List<TimerItem>();

            TimerItem tmpTimerItem = SpawnTimerItem();
            tmpTimerItem.delay = delay;
            tmpTimerItem.time = 0;
            tmpTimerItem.handle = handle;
            tmpTimerItem.handleArg = obj;
            mTimerList.Add(tmpTimerItem);
        }

        void UpdateTimer(float deltaTime)
        {
            for (int i = mTimerList.Count - 1; i >= 0; --i)
            {
                if (i >= mTimerList.Count)
                    continue;

                TimerItem tmpTimerItem = mTimerList[i];
                tmpTimerItem.time += deltaTime;

                if (tmpTimerItem.time >= tmpTimerItem.delay)
                {
                    Action<object> tmpAction = tmpTimerItem.handle;
                    object tmpObj = tmpTimerItem.handleArg;
                    
                    DespawnTimerItem(tmpTimerItem);
                    mTimerList.RemoveAt(i);
                    
                    tmpAction?.Invoke(tmpObj);
                }
            }
        }

        public void RemoveTimer(float delay, Action<object> handle)
        {
            if (null == mTimerList)
                return;

            for (int i = mTimerList.Count - 1; i >= 0; --i)
            {
                TimerItem tmpTimerItem = mTimerList[i];

                if (AosBaseFramework.Utility.CompareFloatValue(tmpTimerItem.delay, delay) && tmpTimerItem.handle == handle)
                {
                    mTimerList.RemoveAt(i);
                    DespawnTimerItem(tmpTimerItem);
                }
            }
        }

        void ClearTimer()
        {
            if (null == mTimerList)
                return;

            for (int i = mTimerList.Count - 1; i >= 0; --i)
            {
                TimerItem tmpTimerItem = mTimerList[i];
                tmpTimerItem.handle = null;
                DespawnTimerItem(tmpTimerItem);
            }

            mTimerList.Clear();
            if (mGameTimer!=null)
            {
                mGameTimer.Clear();
            }
        }
        List<TimerBase> mGameTimer = new List<TimerBase>();
        public T FindTimer<T>() where T : TimerBase
        {
            for (int i = 0, max = mGameTimer.Count; i < max; ++i)
            {
                TimerBase tmpTimer = mGameTimer[i];

                if (tmpTimer.GetType() == typeof(T))
                    return tmpTimer as T;
            }

            return default(T);
        }

        public T CreateTimer<T>() where T : TimerBase
        {
            T tmpTimer = FindTimer<T>();

            if (null== tmpTimer)
            {
                tmpTimer = Activator.CreateInstance<T>();
                mGameTimer.Add(tmpTimer);
            }

            return tmpTimer;
        }

        public T CreateTimer<T,A>(A a) where T : TimerBase
        {
            T tmpTimer = FindTimer<T>();

            if (null == tmpTimer)
            {
                tmpTimer = Activator.CreateInstance<T>();
                mGameTimer.Add(tmpTimer);
                tmpTimer.IsStart = true;
            }
            var tmpInitData = tmpTimer as TimerBase.IInitData<A>;
            if (null != tmpInitData)
            {
                tmpInitData.InitData(a);
            }

            return tmpTimer;
        }
        public T CreateTimer<T, A,B>(A a,B b) where T : TimerBase
        {
            T tmpTimer = FindTimer<T>();

            if (null == tmpTimer)
            {
                tmpTimer = Activator.CreateInstance<T>();
                mGameTimer.Add(tmpTimer);
                tmpTimer.IsStart = true;
            }
            var tmpInitData = tmpTimer as TimerBase.IInitData<A,B>;
            if (null != tmpInitData)
            {
                tmpInitData.InitData(a,b);
            }

            return tmpTimer;
        }

        public void StopTimer<T>() where T : TimerBase
        {
            T tmpTimer = FindTimer<T>();

            if (tmpTimer!=null)
            {
                tmpTimer.IsStart = false;
            }
        }

        public void StartTimer<T>() where T : TimerBase
        {
            T tmpTimer = FindTimer<T>();

            if (tmpTimer != null)
            {
                tmpTimer.IsStart = true;
            }
        }
    }
}
