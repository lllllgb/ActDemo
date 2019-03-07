using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UGUIJoystick
{
    static Vector2 sJoystickDelta = Vector2.zero;
    static bool sJoystickPressed = false;

    GameObject mRoot;
    Image mBackgroundImg;
    Image mCenterImg;
    Vector2 mBgInitPos;
    float mWorld2ScreenModify = 1f;
    float mRadius;
    bool mIsTouching;
    int mTouchedID;

    Color mInactiveColor = new Color(1, 1, 1, 0.3f);

    public static Vector2 JoystickDelta()
    {
        return sJoystickDelta;
    }

    public static bool JoystickPressed()
    {
        return sJoystickPressed;
    }
    
    // Use this for initialization
    public void Init(GameObject root, Image background, Image center)
    {
        mRoot = root;
        mBackgroundImg = background;
        mCenterImg = center;
        mBgInitPos = mBackgroundImg.rectTransform.anchoredPosition;

        ChangeAlpha(false);
        mWorld2ScreenModify = 1280f / Screen.width;
        mRadius = mBackgroundImg.rectTransform.sizeDelta.x * 0.5f - mCenterImg.rectTransform.sizeDelta.x * 0.5f;

        UGUIEventListener.Get(root).onDown = OnPointerDown;
        UGUIEventListener.Get(root).onUp = OnPointerUp;
    }

    // Update is called once per frame
    public void Update(float deltaTime)
    {
        JoystickController();
    }

    void JoystickController()
    {
        if (mIsTouching)
        {
            Vector2 tmpTouchPos = GetClickPosition() * mWorld2ScreenModify;
            float distance = Vector2.Distance(tmpTouchPos, mBackgroundImg.rectTransform.anchoredPosition);

            if (distance < mRadius)
            {
                //当距离小于半径就开始移动 摇杆重心
                mCenterImg.rectTransform.anchoredPosition = tmpTouchPos - mBackgroundImg.rectTransform.anchoredPosition;
            }
            else
            {
                //求圆上的一点：(目标点-原点) * 半径/原点到目标点的距离
                Vector2 endPosition = (tmpTouchPos - mBackgroundImg.rectTransform.anchoredPosition) * mRadius / distance;
                mCenterImg.rectTransform.anchoredPosition = endPosition;
            }

            sJoystickDelta = mCenterImg.rectTransform.anchoredPosition;
        }
    }

    void OnPointerDown(PointerEventData eventData)
    {
        if (mIsTouching)
        {
            return;
        }

        sJoystickPressed = true;
        mTouchedID = eventData.pointerId;
        Vector2 tmpTouchPos = GetClickPosition();
        mBackgroundImg.rectTransform.anchoredPosition = tmpTouchPos * mWorld2ScreenModify;
        mIsTouching = true;
        ChangeAlpha(true);
    }

    void OnPointerUp(PointerEventData eventData)
    {
        if (!mIsTouching || mTouchedID != eventData.pointerId)
        {
            return;
        }

        sJoystickPressed = false;
        sJoystickDelta = Vector2.zero;
        ChangeAlpha(false);
        mBackgroundImg.rectTransform.anchoredPosition = mBgInitPos;
        mCenterImg.rectTransform.anchoredPosition = Vector2.zero;
        mIsTouching = false;
    }

    /// <summary>
    /// 根据平台不同获取点击位置坐标
    /// </summary>
    /// <returns></returns>
    Vector2 GetClickPosition()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return Input.GetTouch(mTouchedID).position;

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
    void ChangeAlpha(bool active)
    {
        mBackgroundImg.color = active ? Color.white : mInactiveColor;
        mCenterImg.color = active ? Color.white : mInactiveColor;
    }
}
