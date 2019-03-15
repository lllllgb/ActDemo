using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using UnityEngine.UI;

namespace AosHotfixRunTime
{
    public class FadeWnd : WindowBase, WindowBase.IInitData2<bool, bool>
    {
        public override string BundleName { get { return "FadeWnd"; } }
        protected override bool IsLoadAsync { get { return false; } }

        const float FADE_TIME = 0.5f;

        RawImage mFadeImg;

        bool mIsFade2Black;
        bool mIsCloseOnFinish;
        float mFadeTime;

        public FadeWnd()
        {
            WindowType = EWindowType.None;
        }

        protected override void AfterInit()
        {
            mFadeImg = Find<RawImage>("RawImage_Fade");
        }

        public void InitData(bool isFade2Black, bool isCloseOnFinish)
        {
            mIsFade2Black = isFade2Black;
            mIsCloseOnFinish = isCloseOnFinish;
        }

        protected override void AfterShow()
        {
            mFadeTime = FADE_TIME;
            mFadeImg.color = mIsFade2Black ? Color.clear : Color.black;
        }

        protected override void BeforeClose()
        {

        }

        protected override void Update(float deltaTime)
        {
            if (mFadeTime > 0f)
            {
                float tmpLerp = deltaTime / mFadeTime;

                if (mIsFade2Black)
                {
                    Fade2Black(tmpLerp);
                }
                else
                {
                    Fade2Clear(tmpLerp);
                }

                mFadeTime -= deltaTime;

                if (mFadeTime <= 0f && mIsCloseOnFinish)
                {
                    Close();
                }
            }
        }

        private void Fade2Clear(float t)
        {
            mFadeImg.color = Color.Lerp(mFadeImg.color, Color.clear, t);
        }

        private void Fade2Black(float t)
        {
            mFadeImg.color = Color.Lerp(mFadeImg.color, Color.black, t);
        }
    }
}
