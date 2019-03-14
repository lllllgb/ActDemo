using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class PlayerController : ControllerBase
    {

        public List<SkillItem> SkillItems { get; set; } = new List<SkillItem>();

        public override void Reset()
        {
            base.Reset();
        }

        public void Init(int unitID, int level)
        {
            for (int i = 0, max = SkillBaseManager.instance.Size; i < max; ++i)
            {
                var tmpSkillItem = new SkillItem();
                tmpSkillItem.Init(SkillBaseManager.instance.Get(i).ID, 1);
                SkillItems.Add(tmpSkillItem);
            }
        }
    }
}
