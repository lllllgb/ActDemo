using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class SkillItem : ACT.ISkillItem
    {
        public SkillBase SkillBase { get; private set; }
        public SkillAttrBase SkillAttrBase { get; private set; }
        public ACT.ISkillInput SkillInput { get; set; }

        public int ID { get { return null == SkillBase ? 0 : SkillBase.ID; } }
        public int Level { get { return null == SkillAttrBase ? 0 : SkillAttrBase.Level; } }

        public SkillItem()
        {

        }

        public void Init(int skillID, int skillLv)
        {
            SkillBase = SkillBaseManager.instance.Find(skillID);

            if (null == SkillBase)
            {
                Logger.LogError($"找不到技能表 id -> {skillID}");
            }

            SkillAttrBase = SkillAttrBaseManager.instance.Find(skillID, skillLv);

            if (null == SkillAttrBase)
            {
                Logger.LogError($"找不到技能属性表 id -> {skillID} level -> {skillLv}");
            }
        }
    }

    public class SkillItemLink
    {
        public int SkillSlot { get; set; }
        public List<SkillItem> SkillItems { get; private set; } = new List<SkillItem>();
    }
}
