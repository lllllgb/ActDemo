using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerListener : MonoBehaviour
{
    public delegate void VoidDelegate(Collider other);

    public VoidDelegate OnEnter { get; set; }
    public VoidDelegate OnStay { get; set; }
    public VoidDelegate OnExit { get; set; }

    public static TriggerListener Get(GameObject go)
    {
        TriggerListener tmpListener = go.GetComponent<TriggerListener>();

        if (null == tmpListener)
            tmpListener = go.AddComponent<TriggerListener>();

        return tmpListener;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnExit?.Invoke(other);
    }
}
