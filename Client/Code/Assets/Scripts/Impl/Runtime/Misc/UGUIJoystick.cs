using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void JoystickMoveDelegate(float deltaX, float deltaY);

public class UGUIJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject joystickUI;                   //摇杆整体UI,方便Active
    public RectTransform joystickCenter;            //摇杆重心
    public RectTransform joystickBackground;        //摇杆背景

    public RectTransform joystickRect;
    private float radius;
    bool isClick;
    Vector2 clickPosition;


    public static event JoystickMoveDelegate JoystickMoveEvent;
    
    // Use this for initialization
    void Start()
    {
        radius = 71;
    }

    // Update is called once per frame
    void Update()
    {
        JoystickController();
    }

    public void JoystickController()
    {
        if (isClick)
        {
            clickPosition = GetClickPosition();
            float distance = Vector2.Distance(new Vector2(clickPosition.x, clickPosition.y) / Screen.width * 960, joystickRect.anchoredPosition);

            if (distance < radius)
            {
                //当距离小于半径就开始移动 摇杆重心
                joystickCenter.anchoredPosition = new Vector2(clickPosition.x / Screen.width * 960 - joystickRect.anchoredPosition.x, clickPosition.y / Screen.width * 960 - joystickRect.anchoredPosition.y);
            }
            else
            {
                //求圆上的一点：(目标点-原点) * 半径/原点到目标点的距离
                Vector2 endPosition = (new Vector2(clickPosition.x, clickPosition.y) / Screen.width * 960 - joystickRect.anchoredPosition) * radius / distance;
                joystickCenter.anchoredPosition = endPosition;
            }

            JoystickMoveEvent?.Invoke(joystickCenter.anchoredPosition.x - joystickBackground.anchoredPosition.x, joystickCenter.anchoredPosition.y - joystickBackground.anchoredPosition.y);

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeAlpha(1);
        clickPosition = GetClickPosition();
        joystickRect.anchoredPosition = clickPosition / Screen.width * 960;
        isClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ChangeAlpha(0.3f);
        joystickCenter.anchoredPosition = Vector2.zero;
        isClick = false;
    }

    /// <summary>
    /// 根据平台不同获取点击位置坐标
    /// </summary>
    /// <returns></returns>
    Vector2 GetClickPosition()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return Input.GetTouch(0).position;

        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return Input.mousePosition;
        }
        return Vector2.zero;
    }

    /// <summary>
    /// 改变图片alpha值
    /// </summary>
    /// <param name="alphaValue"></param>
    void ChangeAlpha(float alphaValue)
    {
        joystickBackground.GetComponent<RawImage>().color = new Color(1, 1, 1, alphaValue);
        joystickCenter.GetComponent<Image>().color = new Color(1, 1, 1, alphaValue);
    }
}
