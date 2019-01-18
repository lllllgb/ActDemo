using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AosHotfixFramework
{
    public interface ITimerManager
    {
        void AddTimer(float delay, Action<object> handle, object obj);

        void RemoveTimer(float delay, Action<object> handle);

        T CreateTimer<T>() where T : TimerBase;

        T CreateTimer<T, A>(A a) where T : TimerBase;

        T CreateTimer<T, A, B>(A a, B b) where T : TimerBase;

        T FindTimer<T>() where T : TimerBase;

        void StopTimer<T>() where T : TimerBase;

        void StartTimer<T>() where T : TimerBase;
    }
}

