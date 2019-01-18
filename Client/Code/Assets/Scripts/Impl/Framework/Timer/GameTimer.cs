using AosHotfixFramework;
using System;
using System.Collections;
using UnityEngine;
using static AosHotfixFramework.TimerBase;

namespace AosBaseFramework
{
    public class GameTimer : TimerBase, IInitData<long,bool>
    {
        const int mAddTimer=1;
        private long mRefreshTime;
        DateTime tmpDateTime;
        float Timer = 0;
        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Timer += deltaTime;
            if (Timer >= mAddTimer && tmpDateTime != null)
            {
                Timer = 0;
                tmpDateTime= tmpDateTime.AddSeconds(mAddTimer);
            }
        }
        public void InitData(long a,bool un)
        {
            IsStart = true;
            SetNewTimer(a,un);
        }

        public void SetNewTimer(long refreshTime, bool Un)
        {
            mRefreshTime = refreshTime;
            tmpDateTime = TimeHelper.GreenwishTime(mRefreshTime, Un);
            tmpDateTime = tmpDateTime.AddSeconds(-1);
        }

        public DateTime Timers()
        {
           return tmpDateTime;
        }
    }
}