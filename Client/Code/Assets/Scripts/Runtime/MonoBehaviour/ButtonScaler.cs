using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonScaler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float ScaleValue = 0.9f;
    public float ScaleTime = 0.2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
        transform.DOScale(ScaleValue, ScaleTime);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1f, ScaleTime);
    }
}
