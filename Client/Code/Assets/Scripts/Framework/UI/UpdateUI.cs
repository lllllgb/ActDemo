using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AosBaseFramework
{
    public class UpdateUI : UIBase
    {
        protected override string ResName
        {
            get
            {
                return "UpdateUI";
            }
        }

        private GameObject mCheckingGo;


        private GameObject mUpdateingGo;
        private Text mUpdateTipText;
        private Slider mDownloadProgressSlider;

        const float SPEED = 100;
        float mUpdateProgress;
        float mShowProgress;
        float mLastShowProgress;

        protected override void AfterInit()
        {
            mCheckingGo = Utility.GameObj.Find(RootGo, "CheckUpdate");

            mUpdateingGo = Utility.GameObj.Find(RootGo, "Updateing");
            mUpdateTipText = Utility.GameObj.Find<Text>(mUpdateingGo, "Label_Tip");
            mDownloadProgressSlider = Utility.GameObj.Find<Slider>(mUpdateingGo, "Slider_Progress");
        }

        protected override void AfterShow()
        {
        }

        protected override void BeforeClose()
        {
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (mShowProgress < mUpdateProgress)
            {
                mShowProgress = Mathf.Min(mShowProgress + SPEED * deltaTime, mUpdateProgress);

                RefreshProgress(mShowProgress);
            }
        }

        private void RefreshProgress(float progress)
        {
            if (progress - mLastShowProgress >= 1)
            {
                mLastShowProgress = (int)progress;
                mUpdateTipText.text = $"{(int)mLastShowProgress} %";
            }

            mDownloadProgressSlider.value = progress * 0.01f;
        }

        public void ShowChecking()
        {
            Utility.GameObj.SetActive(mCheckingGo, true);
            Utility.GameObj.SetActive(mUpdateingGo, false);
        }

        public void ShowUpdateing()
        {
            Utility.GameObj.SetActive(mCheckingGo, false);
            Utility.GameObj.SetActive(mUpdateingGo, true);
            mUpdateProgress = 0;
            mShowProgress = 0;
            mLastShowProgress = -1;
            RefreshProgress(0);
        }

        public void SetUpdateProgress(int pregress)
        {
            mUpdateProgress = pregress;
        }
    }
}
