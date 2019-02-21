using System;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine.UI;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class LoadingWnd : WindowBase
    {
        public override string BundleName { get { return "LoadingWnd"; } }

        protected override bool IsLoadAsync { get { return false; } }

        Slider mProgressBar;

        const float mSlowSpeed = 0.3f;
        const float mMidSpeed = 0.6f;
        const float mFastSpeed = 4f;

        private bool mFinish;
        public bool Finish { get { return mFinish; } }

        float mWaifProgress = 80;
        private float mProgress;

        private bool mIsResLoaded;
        public bool IsResLoaded { set { mIsResLoaded = value; } }

        public LoadingWnd()
        {
            mWindowType = EWindowType.Loading;
        }

        protected override void AfterInit()
        {
            GameObject tmpCB = Find("CB");
            mProgressBar = Find<Slider>(tmpCB, "Slider_Progress");
        }

        protected override void AfterShow()
        {
            mFinish = false;
            mIsResLoaded = false;
            mProgress = 0;
            mWaifProgress = UnityEngine.Random.Range(70, 95);
            mProgressBar.value = 0f;
        }

        protected override void BeforeClose()
        {
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            UpdateProgress();
        }

        void UpdateProgress()
        {
            if (mProgress >= 110)
            {
                mFinish = true;
                return;
            }

            if (mIsResLoaded)
            {
                mProgress += mFastSpeed;
                UpdateProgress(mProgress);
            }
            else if (mProgress < mWaifProgress)
            {
                if (mProgress < mWaifProgress * 0.5f)
                {
                    mProgress += mMidSpeed;
                }
                else
                {
                    mProgress += mSlowSpeed;
                }

                mProgress = Mathf.Min(mProgress, mWaifProgress);
                UpdateProgress(mProgress);
            }

        }

        public void UpdateProgress(float progress)
        {
            mProgressBar.value = progress * 0.01f;
        }
    }
}
