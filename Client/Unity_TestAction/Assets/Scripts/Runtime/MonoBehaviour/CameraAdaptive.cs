using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdaptive : MonoBehaviour
{
    public enum EAdaptiveMode
    {
        Width,
        Height,
    }

    public EAdaptiveMode adaptiveMode = EAdaptiveMode.Height;

    public float designWidth = 9.6f;
    public float designHeight = 5.4f;

    // Use this for initialization
    void Start()
    {
        float orthographicSize = 0;

        switch (adaptiveMode)
        {
            case EAdaptiveMode.Width:
                float aspectRatio = Screen.width * 1.0f / Screen.height;
                orthographicSize = designWidth / (2 * aspectRatio);
                break;
            case EAdaptiveMode.Height:
                orthographicSize = designHeight / 2;
                break;
        }

        this.GetComponent<Camera>().orthographicSize = orthographicSize;
    }
}
