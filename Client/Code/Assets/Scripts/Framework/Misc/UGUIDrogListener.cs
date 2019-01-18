using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIDrogListener : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public delegate void VoidDelegate(PointerEventData eventData);

    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onEnter;
    public VoidDelegate onBeginDrag;
    public VoidDelegate onDrag;
    public VoidDelegate onEndDrag;

    static public UGUIDrogListener Get(GameObject go)
    {
        UGUIDrogListener listener = go.GetComponent<UGUIDrogListener>();
        if (listener == null) listener = go.AddComponent<UGUIDrogListener>();
        return listener;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(eventData);
    }
  


}
