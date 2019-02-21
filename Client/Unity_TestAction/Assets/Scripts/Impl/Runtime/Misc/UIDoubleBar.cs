using System;
using UnityEngine;
using UnityEngine.UI;

namespace AosHotfixRunTime
{
    public class UIDoubleBar
    {
        const float HALF_VALUE = 0.5f;

        private Image mBar0;
        private Image mBar1;
        private float mValue;
        public float Value
        {
            set
            {
                mValue = Mathf.Clamp01(value);

                if (mValue <= HALF_VALUE)
                {
                    if (null != mBar0) mBar0.fillAmount = mValue / HALF_VALUE;
                    if (null != mBar1) mBar1.fillAmount = 0f;
                }
                else
                {
                    if (null != mBar0) mBar0.fillAmount = 1f;
                    if (null != mBar1) mBar1.fillAmount = (mValue - HALF_VALUE) / HALF_VALUE;
                }
            }
            get
            {
                return mValue;
            }
        }

        public void Init(Image bar0, Image bar1)
        {
            mBar0 = bar0;
            mBar1 = bar1;
        }
        
    }
}
