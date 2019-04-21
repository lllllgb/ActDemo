using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;
using UnityEngine.EventSystems;

namespace AosHotfixRunTime
{
    public class SkillDescWnd : WindowBase
    {
        public override string BundleName { get { return "SkillWnd"; } }

        private Text mSkillNameLab;
        private Text mSkillDescLab;

        private int mSkillID;

        protected override void AfterInit()
        {
            base.AfterInit();

            mSkillNameLab = Find<Text>("Text_SkillName");
            mSkillDescLab = Find<Text>("Text_SkillDesc");

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
            }
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
