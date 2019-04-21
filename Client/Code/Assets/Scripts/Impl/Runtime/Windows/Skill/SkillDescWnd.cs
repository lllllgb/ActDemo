using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using AosHotfixFramework;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace AosHotfixRunTime
{
    public class SkillDescWnd : WindowBase
    {
        public override string BundleName { get { return "SkillWnd"; } }

        private Text mSkillNameLab;
        private Text mSkillDescLab;
        private VideoPlayer mSkillVideo;
        private RawImage mSkillVideoRawImg;

        private int mSkillID;

        protected override void AfterInit()
        {
            base.AfterInit();

            mSkillNameLab = Find<Text>("Text_SkillName");
            mSkillDescLab = Find<Text>("Text_SkillDesc");
            mSkillVideoRawImg = Find<RawImage>("RawImage_Video");
            mSkillVideo = mSkillVideoRawImg.GetComponent<VideoPlayer>();

            RegisterEventClick(Find("Image_Mask"), OnClickMask);
        }

        public void SetSkillData(int skillID)
        {
            mSkillID = skillID;
        }

        protected override void AfterShow()
        {
            base.AfterShow();

            SkillBase tmpSkillBase = SkillBaseManager.instance.Find(mSkillID);

            if (null != tmpSkillBase)
            {
                mSkillNameLab.text = tmpSkillBase.Name;
                mSkillDescLab.text = tmpSkillBase.Desc;
                PlayVideo(tmpSkillBase.VideoName);
            }
        }

        void PlayVideo(string videoName)
        {
            Game.ResourcesMgr.LoadBundleByType(EABType.Audio, videoName);
            var tmpVideoClip = Game.ResourcesMgr.GetAssetByType<VideoClip>(EABType.Audio, videoName);
            mSkillVideo.clip = tmpVideoClip;
            mSkillVideo.Play();
            mSkillVideoRawImg.color = Color.clear;
            //mSkillVideoRawImg.DOFade(1, 0.5f);
            mSkillVideoRawImg.DOColor(Color.white, 1f);
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();
        }

        private void OnClickMask(PointerEventData pointer)
        {
            Close();
        }
    }
}
