using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace AosHotfixRunTime
{
    public class UICombo
    {
        int mComboCount = 0;
        GameObject mCombo;
        CanvasGroup mCanvasGroup;
        Text mCountLab;

        Tween mTweenAlpha;
        DOTweenAnimation mTweenScale;

        public UICombo(GameObject comboGo)
        {
            if (null == comboGo)
            {
                Logger.LogError("can not find combo obj!");
                return;
            }

            mCombo = comboGo;
            mCanvasGroup = comboGo.GetComponent<CanvasGroup>();
            mTweenAlpha = mCanvasGroup.DOFade(0, 1f);
            mTweenAlpha.SetDelay(0.5f);
            mTweenAlpha.SetAutoKill(false);
            mTweenAlpha.OnComplete(OnFinish);
            mTweenScale = mCombo.transform.Find("Scale").GetComponent<DOTweenAnimation>();
            mCountLab = mTweenScale.transform.Find("Text_Count").GetComponent<Text>();
            
            mCombo.SetActive(false);
        }

        public void ComboHit(int comboHit)
        {
            if (null == mCombo)
                return;

            if (!mCombo.activeSelf)
                mCombo.SetActive(true);

            mComboCount += comboHit;
            mCountLab.text = mComboCount.ToString();

            mTweenAlpha.Restart();
            mTweenScale.DORestart();
        }

        void OnFinish()
        {
            mComboCount = 0;
        }

        public void Reset()
        {
            mComboCount = 0;
            mCombo.SetActive(false);
        }
    }
}
