using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UGUIEventListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public delegate void VoidDelegate(PointerEventData eventData);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;

    static public UGUIEventListener Get(GameObject go)
    {
        UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
        if (listener == null) listener = go.AddComponent<UGUIEventListener>();
        return listener;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(eventData);
    }

    //public void OnSelect(BaseEventData eventData)
    //{
    //    if (onSelect != null) onSelect(eventData);
    //}

    //public void OnUpdateSelected(BaseEventData eventData)
    //{
    //    if (onUpdateSelect != null) onUpdateSelect(eventData);
    //}

    public void SetEventNil()
    {
        onClick = null;
    }

    public void Coroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }


}
