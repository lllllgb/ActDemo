using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using AosHotfixFramework;
using UnityEngine.EventSystems;
using DG.Tweening;
using RenderHeads.Media.AVProVideo;

namespace AosHotfixRunTime
{
    public class SkillDescWnd : WindowBase
    {
        public override string BundleName { get { return "SkillWnd"; } }

        private Text mSkillNameLab;
        private Text mSkillDescLab;
        private VideoPlayer mSkillVideo;
        private RawImage mSkillVideoRawImg;
        private MediaPlayer mSkillVideoPlayer;

        private int mSkillID;

        protected override void AfterInit()
        {
            base.AfterInit();

            mSkillNameLab = Find<Text>("Text_SkillName");
            mSkillDescLab = Find<Text>("Text_SkillDesc");
            mSkillVideoRawImg = Find<RawImage>("RawImage_Video");
            mSkillVideo = mSkillVideoRawImg.GetComponent<VideoPlayer>();
            mSkillVideoPlayer = Find<MediaPlayer>("SkillVideoPlayer");

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
            //Game.ResourcesMgr.LoadBundleByType(EABType.Audio, videoName);
            //var tmpVideoClip = Game.ResourcesMgr.GetAssetByType<VideoClip>(EABType.Audio, videoName);

            //byte[] tmpBytes = AosBaseFramework.FileHelper.LoadBytesFile($"testVideo.mp4", AosBaseFramework.PathHelper.EBytesFileType.Video);
            //mSkillVideoPlayer.OpenVideoFromBuffer(tmpBytes, true);

            mSkillVideoPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, $"video/{videoName}.mp4", true);

            //mSkillVideo.clip = tmpVideoClip;
            //mSkillVideo.Play();
            //mSkillVideoRawImg.color = Color.clear;
            //mSkillVideoRawImg.DOColor(Color.white, 1f);

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
