using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class PlayerController : ControllerBase
    {

        public int UnitID { get; private set; }
        public int Level { get; private set; }

        List<List<SkillItemLink>> mSkillCollect;
        public int CurrSkillPage { get; private set; }
        SkillItemLink mExSkillLink;


        class SkillCDData
        {
            public int SkillID;
            public float CD;
        }

        List<SkillCDData> mSkillCDs = new List<SkillCDData>();

        public override void Reset()
        {
            base.Reset();
        }

        public void Init(int unitID, int level)
        {
            this.UnitID = unitID;
            this.Level = level;

            InitSkill();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            for (int i = 0, max = mSkillCDs.Count; i < max; ++i)
            {
                mSkillCDs[i].CD -= deltaTime;
            }
        }

        void InitSkill()
        {
            CurrSkillPage = 0;
            mSkillCollect = new List<List<SkillItemLink>>();

            for (int i = 0, max = 2; i < max; ++i)
            {
                List<SkillItemLink> tmpSkillPage = new List<SkillItemLink>();
                mSkillCollect.Add(tmpSkillPage);

                for (int j = 0, jmax = 6; j < jmax; ++j)
                {
                    SkillItemLink tmpSkillLink = new SkillItemLink();
                    tmpSkillPage.Add(tmpSkillLink);

                    string tmpStr = PlayerPrefs.GetString($"Skill{i}_{j}");
                    if (!string.IsNullOrEmpty(tmpStr))
                    {
                        string[] tmpStrs = tmpStr.Split('-');

                        for (int k = 0, kmax = tmpStrs.Length; k < kmax; ++k)
                        {
                            int tmpSkillID = 0;

                            if (int.TryParse(tmpStrs[k], out tmpSkillID))
                            {
                                SkillItem tmpSkillItem = new SkillItem();
                                tmpSkillItem.Init(tmpSkillID, 1);
                                tmpSkillLink.SkillItems.Add(tmpSkillItem);
                            }
                        }
                    }
                }
            }

            mExSkillLink = new SkillItemLink();
            UnitExtraBase tmpUnitExtraBase = UnitExtraBaseManager.instance.Find(UnitID);

            if(null != tmpUnitExtraBase)
            {
                SkillItem tmpSkillItem = new SkillItem();
                tmpSkillItem.Init(tmpUnitExtraBase.SkillEx, 1);
                mExSkillLink.SkillItems.Add(tmpSkillItem);
            }
        }

        public void SetCurrSkillPage(int pageIndex)
        {
            CurrSkillPage = pageIndex;
        }

        public List<SkillItemLink> GetSkillLinkList(int index)
        {
            return index >= mSkillCollect.Count ? null : mSkillCollect[index];
        }

        public SkillItemLink GetSkillLink(int index)
        {
            return mSkillCollect[CurrSkillPage][index];
        }

        public SkillItemLink GetExSkillLink()
        {
            return mExSkillLink;
        }

        public void SaveSkill()
        {
            for (int i = 0, max = mSkillCollect.Count; i < max; ++i)
            {
                for (int j = 0, jmax = mSkillCollect[i].Count; j < jmax; ++j)
                {
                    string tmpSkillItemLinkStr = string.Empty;

                    for (int k = 0, kmax = mSkillCollect[i][j].SkillItems.Count; k < kmax; ++k)
                    {
                        if (tmpSkillItemLinkStr == string.Empty)
                        {
                            tmpSkillItemLinkStr = mSkillCollect[i][j].SkillItems[k].ID.ToString();
                        }
                        else
                        {
                            tmpSkillItemLinkStr += $"-{mSkillCollect[i][j].SkillItems[k].ID}";
                        }
                    }

                    PlayerPrefs.SetString($"Skill{i}_{j}", tmpSkillItemLinkStr);
                }

            }
        }

        public void SetSkillCD(int skillID, float cd)
        {
            SkillCDData tmpData = null;

            for (int i = 0, max = mSkillCDs.Count; i < max; ++i)
            {
                if (mSkillCDs[i].SkillID == skillID)
                {
                    tmpData = mSkillCDs[i];
                    break;
                }
            }

            if (null == tmpData)
            {
                tmpData = new SkillCDData();
                tmpData.SkillID = skillID;
                mSkillCDs.Add(tmpData);
            }

            tmpData.CD = cd;

            var tmpEvent = ReferencePool.Fetch<PlayerCtrlEvent.UpdateSkillCD>();
            tmpEvent.SkillID = skillID;
            tmpEvent.CD = cd;
            Game.EventMgr.FireNow(this, tmpEvent);
        }

        public float GetSkillCD(int skillID)
        {
            float tmpCD = 0;

            for (int i = 0, max = mSkillCDs.Count; i < max; ++i)
            {
                if (mSkillCDs[i].SkillID == skillID)
                {
                    tmpCD = mSkillCDs[i].CD;
                    break;
                }
            }

            return tmpCD;
        }

    }
}
