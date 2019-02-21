using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace AosHotfixRunTime
{
    public static class TouchHelper
    {
        public static bool IsTouchedUI(int fingerId = 0)
        {
            bool tmpFlag = false;

#if UNITY_ANDROID || UNITY_IOS
            if (EventSystem.current.IsPointerOverGameObject(fingerId))
            {
                tmpFlag = true;
            }
#else
            if (EventSystem.current.IsPointerOverGameObject())
            {
                tmpFlag = true;
            }
#endif

            return tmpFlag;
        }

        public static void CheckTouch(Action<Vector3> touchAction, Action touchEndAction)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
#if UNITY_ANDROID || UNITY_IOS
                if (IsTouchedUI(Input.GetTouch(0).fingerId))
#else
                if (IsTouchedUI())
#endif
                    return;

                touchAction?.Invoke(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                touchEndAction?.Invoke();
            }
        }
    }
}
