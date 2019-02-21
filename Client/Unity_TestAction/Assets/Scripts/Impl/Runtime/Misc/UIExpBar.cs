using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AosHotfixRunTime
{
    public class UIExpBar
    {
        const int TWEEN_SPEED = 2;

        private Slider mSlider;
        private Text mRatioTxt;
        private int mValue;
        private int mCurrValue;
        private int mFullFillCount = 0;
        private bool mIsTweening = false;

        public void Init(Slider slider, Text ratioTxt)
        {
            mSlider = slider;
            mRatioTxt = ratioTxt;
        }

        public void SetValue(int value)
        {
            mCurrValue = mValue = value;

            if (null != mSlider)
            {
                mSlider.value = mValue * 0.01f;
            }

            if (null != mRatioTxt)
            {
                mRatioTxt.text = $"{mCurrValue}%";
            }
        }

        public void Start(int value)
        {
            if (null == mSlider || mValue == value)
            {
                return;
            }

            mIsTweening = true;
            mValue = value;
            mFullFillCount = mValue < mSlider.value * 100 ? 1 : 0;
        }

        public void Update(float deltaTime)
        {
            if (!mIsTweening || null == mSlider)
            {
                return;
            }

            int tmpValue = (int)(mSlider.value * 100) + TWEEN_SPEED;

            if (mFullFillCount > 0)
            {
                if (tmpValue > 100)
                {
                    --mFullFillCount;
                    tmpValue = 0;
                }
            }
            else
            {
                if (tmpValue >= mValue)
                {
                    tmpValue = mValue;
                    mIsTweening = false;
                    mRatioTxt.text = $"{tmpValue}%";
                }
            }

            mCurrValue = tmpValue;
            mSlider.value = tmpValue * 0.01f;
        }

        public void Update10Frame(float deltaTime)
        {
            if (!mIsTweening || null == mRatioTxt)
            {
                return;
            }

            mRatioTxt.text = $"{mCurrValue}%";
        }

        public void Stop()
        {
            mIsTweening = false;

            if (null != mSlider)
            {
                mSlider.value = mValue * 0.01f;
            }
        }
    }
}
