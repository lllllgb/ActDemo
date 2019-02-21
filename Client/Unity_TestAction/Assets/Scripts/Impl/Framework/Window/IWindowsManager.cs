using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AosHotfixFramework
{
    public interface IWindowsManager
    {
        void SetWindowsRoot(Transform rootTrans);

        T FindWindow<T>() where T : WindowBase;

        T CreateWindow<T>() where T : WindowBase;

        T PreLoadWindow<T>(Action loadedHandle = null) where T : WindowBase;
        
        T ShowWindow<T>(Action<WindowBase> showedHandle = null) where T : WindowBase;

        T ShowWindow<T, A>(A a, Action<WindowBase> showedHandle = null) where T : WindowBase;

        T ShowWindow<T, A, B>(A a, B b, Action<WindowBase> showedHandle = null) where T : WindowBase;

        void CloseWindow<T>() where T : WindowBase;

        void CloseWindow(WindowBase.EWindowType windowType);

        void DestroyWindow<T>() where T : WindowBase;
    }
}
