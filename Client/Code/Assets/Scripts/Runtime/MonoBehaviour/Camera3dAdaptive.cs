using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Camera3dAdaptive : MonoBehaviour
{
    public float DesignWidth = 1024f;
    public float DesignHeight = 768f;

    // Use this for initialization
    void Start ()
    {
        float tmpManualHeight;

        if ((float)Screen.height / Screen.width < DesignHeight / DesignWidth)
        {
            //如果屏幕的高宽比大于自定义的高宽比 。则通过公式  ManualWidth * manualHeight = Screen.width * Screen.height；
            //来求得适应的  manualHeight ，用它待求出 实际高度与理想高度的比率 scale
            tmpManualHeight = DesignWidth / Screen.width * Screen.height;
        }
        else
        {   //否则 直接给manualHeight 自定义的 ManualHeight的值，那么相机的fieldOfView就会原封不动
            tmpManualHeight = DesignHeight;
        }
        
        float tmpScale = tmpManualHeight / DesignHeight;
        Vector3 tmpPos = transform.localPosition;
        tmpPos.z *= tmpScale;
        transform.localPosition = tmpPos;
    }
}
