using System;
using UnityEngine;
using UnityEngine.UI;

namespace AosHotfixRunTime
{
    public class UITypewriter
    {
        public const float CHARS_PER_SECOND = 0.1f;//打字时间间隔
        private string mWords = string.Empty;//保存需要显示的文字
        private bool mIsActive = false;
        private float mTime;//计时器
        private Text mDisplayText;
        private int mCurrentPos = 0;//当前打字位置
        private Action mFinishHandle;

        public void Init(Text text)
        {
            if (null == text)
            {
                return;
            }

            mDisplayText = text;
            mDisplayText.text = string.Empty;
        }

        public void StartEffect(string word, Action finishHandle)
        {
            mIsActive = true;
            mTime = 0;
            mWords = word;
            mFinishHandle = finishHandle;
        }

        public void Update(float deltaTime)
        {
            Writing(deltaTime);
        }

        public void End()
        {
            mIsActive = false;
            mTime = 0;
            mCurrentPos = 0;

            if (null != mDisplayText)
            {
                mDisplayText.text = mWords;
            }
        }
        
        void Writing(float deltaTime)
        {
            if (!mIsActive || null == mDisplayText)
            {
                return;
            }

            mTime += deltaTime;

            if (mTime >= CHARS_PER_SECOND)
            {
                mTime = 0;
                mCurrentPos++;
                mDisplayText.text = mWords.Substring(0, mCurrentPos);

                if (mCurrentPos >= mWords.Length)
                {
                    OnFinish();
                }
            }
        }

        void OnFinish()
        {
            End();
            mFinishHandle?.Invoke();
            mFinishHandle = null;
        }

    }
}
