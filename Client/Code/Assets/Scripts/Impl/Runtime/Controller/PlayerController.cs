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
        public List<SkillItem> SkillItems { get; set; } = new List<SkillItem>();

        List<List<SkillItemLink>> mSkillCollect;
        int mCurrSkillPage;

        public override void Reset()
        {
            base.Reset();
        }

        public void Init(int unitID, int level)
        {
            this.UnitID = unitID;
            this.Level = level;

            for (int i = 0, max = SkillBaseManager.instance.Size; i < max; ++i)
            {
                var tmpSkillItem = new SkillItem();
                tmpSkillItem.Init(SkillBaseManager.instance.Get(i).ID, 1);
                SkillItems.Add(tmpSkillItem);
            }

            InitSkill();
        }

        void InitSkill()
        {
            mCurrSkillPage = 0;
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
        }

        public void SetCurrSkillPage(int pageIndex)
        {
            mCurrSkillPage = pageIndex;
        }

        public List<SkillItemLink> GetSkillLinkList(int index)
        {
            return index >= mSkillCollect.Count ? null : mSkillCollect[index];
        }

        public SkillItemLink GetSkillLink(int index)
        {
            return mSkillCollect[mCurrSkillPage][index];
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
    }
}
