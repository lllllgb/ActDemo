using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AosBaseFramework
{
    public static partial class Utility
    {
        public static class GameObj
        {
            public static GameObject Find(GameObject obj, string name)
            {
                if (obj == null || string.IsNullOrEmpty(name))
                    return null;
                Transform trans = FindByName(obj.transform, name);
                if (trans != null)
                    return trans.gameObject;
                else
                    return null;
            }

            public static T Find<T>(Transform transform, string name) where T : UnityEngine.Component
            {
                Transform trans = FindByName(transform, name);
                if (trans != null)
                    return trans.GetComponent<T>();

                return null;
            }

            public static T Find<T>(GameObject obj, string name) where T : UnityEngine.Component
            {
                if (null == obj || string.IsNullOrEmpty(name))
                    return null;

                return Find<T>(obj.transform, name);
            }

            public static Transform FindByName(Transform trans, string name)
            {
                if (trans == null)
                    return null;

                if (trans.name == name)
                    return trans;

                return FindTranformInChild(trans, name);
            }

            static Transform FindTranformInChild(Transform trans, string name)
            {
                if (trans == null)
                    return null;
                
                Transform tempTrans = trans.Find(name);
                if (tempTrans != null)
                    return tempTrans;

                for (int i = 0; i < trans.childCount; i++)
                {
                    Transform child = trans.GetChild(i);
                    Transform temp = FindTranformInChild(child, name);
                    if (temp != null)
                        return temp;
                }

                return null;
            }

            public static void SetActive(GameObject go, bool active)
            {
                if (go == null)
                    return;
                if (go.activeSelf != active)
                    go.SetActive(active);
            }

            public static void SetActive<T>(T instance, bool active) where T : UnityEngine.Component
            {
                if (instance == null)
                    return;
                SetActive(instance.gameObject, active);
            }

            public static void SetLayer(GameObject go, int layer, bool includeChild = true)
            {
                if (null == go)
                    return;

                go.layer = layer;

                if (includeChild)
                {
                    for (int i = 0, max = go.transform.childCount; i < max; ++i)
                    {
                        SetLayer(go.transform.GetChild(i).gameObject, layer, includeChild);
                    }
                }
            }

            public static void ResetTransform(Transform transform)
            {
                if (null == transform)
                    return;

                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
        }
    }
}
