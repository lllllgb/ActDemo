using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosBaseFramework;
using AosHotfixFramework;
using System;

namespace AosHotfixRunTime
{
    public class UICountdown : ReferenceDisposer
    {
        private GameObject mCountdownGo;
        private Image mCountdownImg;
        private Text mCountdownLab;

        private bool mIsCountdowning;
        private float mCountdownTime;
        private float mCountdownDuration;
        private float mDeltaTime;

        private Action mTimeOutHandle;
        
        public void Init(GameObject countdownGo, Image countdownImg, Text countdownLab)
        {
            mCountdownGo = countdownGo;
            mCountdownImg = countdownImg;
            mCountdownLab = countdownLab;
        }

        public override void Dispose()
        {
            base.Dispose();

            mCountdownGo = null;
            mCountdownImg = null;
            mCountdownLab = null;
        }

        public void Update(float deltaTime)
        {
            if (mIsCountdowning)
            {
                mCountdownDuration -= deltaTime;
                mCountdownImg.fillAmount = mCountdownDuration / mCountdownTime;
                mDeltaTime += deltaTime;

                if (mDeltaTime >= 1f)
                {
                    mDeltaTime = 0f;
                    mCountdownLab.text = ((int)mCountdownDuration).ToString();

                    if (mCountdownDuration <= 0)
                    {
                        Stop();
                        mTimeOutHandle?.Invoke();
                        mTimeOutHandle = null;
                    }
                }
            }
        }

        public void Start(float time, Action timeoutHandle = null)
        {
            if (time > 0f)
            {
                mIsCountdowning = true;
                mCountdownDuration = mCountdownTime = time;
                Utility.GameObj.SetActive(mCountdownGo, true);
                mCountdownImg.fillAmount = 1f;
                mCountdownLab.text = ((int)time).ToString();
                mTimeOutHandle = timeoutHandle;
            }
            else
            {
                Stop();
            }
        }

        public void Stop()
        {
            mIsCountdowning = false;
            Utility.GameObj.SetActive(mCountdownGo, false);
        }
    }
}

