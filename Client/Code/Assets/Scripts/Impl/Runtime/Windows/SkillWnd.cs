using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;
using UnityEngine.EventSystems;

namespace AosHotfixRunTime
{
    public class SkillWnd : WindowBase
    {
        class SkillElem
        {
            public GameObject RootGo;
            public Toggle SkillTge;
            public Image IconImg;
            public ImageLoader IconLoader;
            public Image Checkmark;
            public SkillBase SkillBase { get; private set; }

            public void Init(GameObject go)
            {
                RootGo = go;
                SkillTge = go.GetComponent<Toggle>();
                IconImg = Find<Image>(go, "Image_Icon");
                Checkmark = Find<Image>(go, "Checkmark");
            }

            public void Refresh(SkillBase data)
            {
                SkillBase = data;

                if (null == data)
                {
                    return;
                }

                if (null == IconLoader)
                {
                    IconLoader = ReferencePool.Fetch<ImageLoader>();
                }

                IconLoader.Load(ImageLoader.EIconType.Skill, data.Icon, IconImg);
            }

            public void Release()
            {
                SkillBase = null;

                if (null != IconLoader)
                {
                    ReferencePool.Recycle(IconLoader);
                    IconLoader = null;
                }

                IconImg.sprite = null;
                SetActive(IconImg, false);
            }

            public void SetSelected(bool flag)
            {
                //SetActive(Checkmark, flag);
                SkillTge.isOn = flag;
            }
        }

        public override string BundleName { get { return "SkillWnd"; } }

        protected override bool IsLoadAsync => false;

        Toggle mSkillPage1Toggle;
        List<SkillElem> mEquipedSkills = new List<SkillElem>();
        GameObject mEquipingRootGo;
        List<SkillElem> mEquipingSkills = new List<SkillElem>();
        List<UIGridTool> mSkillTrees = new List<UIGridTool>();
        Dictionary<int, List<SkillElem>> mSkillTreeItems = new Dictionary<int, List<SkillElem>>();

        int mSkillPage;
        int mSelectedEuqipedSoltIdx;
        int mSelectedEquipingSoltIdx;
        List<SkillItemLink> mSkillLinkList;

        protected override void AfterInit()
        {
            base.AfterInit();

            mSkillPage1Toggle = Find<Toggle>("Toggle_Page1");
            mSkillPage1Toggle.onValueChanged.AddListener(OnValueChangePage1);
            var tmpPage2 = Find<Toggle>("Toggle_Page2");
            tmpPage2.onValueChanged.AddListener(OnValueChangePage2);

            GameObject tmpSkillTemplate = Find("SkillItemTemplate");
            for (int i = 0, max = 3; i < max; ++i)
            {
                GameObject tmpGo = Find($"ScrollView_SkillTree{i}");

                if (null != tmpGo)
                {
                    UIGridTool tmpGT = new UIGridTool(Find(tmpGo, "Content"), tmpSkillTemplate);
                    mSkillTrees.Add(tmpGT);
                }
            }

            GameObject tmpEquipedSkillGo = Find("EquipedSkill");
            for (int i = 0, max = 6; i < max; ++i)
            {
                GameObject tmpGo = Find(tmpEquipedSkillGo, $"Toggle_Skill{i}");

                if (null != tmpGo)
                {
                    SkillElem tmpSkillElem = new SkillElem();
                    tmpSkillElem.Init(tmpGo);
                    mEquipedSkills.Add(tmpSkillElem);

                    RegistEquipedSlotClick(tmpSkillElem, i);
                }
            }

            mEquipingRootGo = Find("EquipingSkill");
            for (int i = 0, max = 5; i < max; ++i)
            {
                GameObject tmpGo = Find(mEquipingRootGo, $"Toggle_Skill{i}");

                if (null != tmpGo)
                {
                    SkillElem tmpSkillElem = new SkillElem();
                    tmpSkillElem.Init(tmpGo);
                    mEquipingSkills.Add(tmpSkillElem);

                    RegistEquingSlotClick(tmpSkillElem, i);
                }
            }

            RegisterEventClick(Find("Button_Back"), OnBackBtnClick);
        }

        protected override void AfterShow()
        {
            base.AfterShow();

            InitSkillTree();
            mSkillPage = Game.ControllerMgr.Get<PlayerController>().CurrSkillPage;
            RefreshByPage();
        }

        protected override void BeforeClose()
        {
            base.BeforeClose();
        }

        protected override void BeforeDestory()
        {
            base.BeforeDestory();
        }

        private void InitSkillTree()
        {
            var tmpPlayerCtrl = Game.ControllerMgr.Get<PlayerController>();
            UnitExtraBase tmpUnitExtraBase = UnitExtraBaseManager.instance.Find(tmpPlayerCtrl.UnitID);
            Dictionary<int, List<SkillBase>> tmpTreeType2Skills = new Dictionary<int, List<SkillBase>>();

            for (int i = 0, max = tmpUnitExtraBase.Skills.data.Count; i < max; ++i)
            {
                SkillBase tmpSkillBase = SkillBaseManager.instance.Find(tmpUnitExtraBase.Skills.data[i]);

                if (null != tmpSkillBase)
                {
                    List<SkillBase> tmpSkills = null;

                    if (!tmpTreeType2Skills.TryGetValue(tmpSkillBase.TreeType, out tmpSkills))
                    {
                        tmpSkills = new List<SkillBase>();
                        tmpTreeType2Skills.Add(tmpSkillBase.TreeType, tmpSkills);
                    }

                    tmpSkills.Add(tmpSkillBase);
                }
            }

            for (int i = 0, max = 3; i < max; ++i)
            {
                List<SkillBase> tmpSkills = null;

                if (!tmpTreeType2Skills.TryGetValue(i, out tmpSkills))
                {
                    continue;
                }

                UIGridTool tmpGT = mSkillTrees[i];
                tmpGT.GenerateElem(tmpSkills.Count);
                List<SkillElem> tmpSkillElems = null;

                if (!mSkillTreeItems.TryGetValue(i, out tmpSkillElems))
                {
                    tmpSkillElems = new List<SkillElem>();
                    mSkillTreeItems.Add(i, tmpSkillElems);
                }

                for (int j = 0, jmax = tmpSkills.Count; j < jmax; ++j)
                {
                    SkillElem tmpSkillElem = null;

                    if (j < tmpSkillElems.Count)
                    {
                        tmpSkillElem = tmpSkillElems[j];
                    }
                    else
                    {
                        tmpSkillElem = new SkillElem();
                        tmpSkillElems.Add(tmpSkillElem);
                        tmpSkillElem.Init(tmpGT.Get(j));
                    }

                    tmpSkillElem.Refresh(tmpSkills[j]);
                    RegistSkillTreeClick(tmpSkillElem);
                }
            }
        }

        private void RefreshByPage()
        {
            mSkillLinkList = Game.ControllerMgr.Get<PlayerController>().GetSkillLinkList(mSkillPage);
            RefreshEquipedSkill();
            SetActive(mEquipingRootGo, false);
        }

        private void RefreshEquipedSkill()
        {
            for (int i = 0, max = mSkillLinkList.Count; i < max; ++i)
            {
                List<SkillItem> tmpSkillItemList = mSkillLinkList[i].SkillItems;

                if (tmpSkillItemList.Count > 0)
                {
                    mEquipedSkills[i].Refresh(tmpSkillItemList[0].SkillBase);
                }
                else
                {
                    mEquipedSkills[i].Release();
                }
            }
        }

        private void RefreshEquipingSkill()
        {
            if (mSelectedEuqipedSoltIdx < 0 || mSelectedEuqipedSoltIdx >= mSkillLinkList.Count)
                return;

            List<SkillItem> tmpSkillItemList = mSkillLinkList[mSelectedEuqipedSoltIdx].SkillItems;

            for (int i = 0, max = mEquipingSkills.Count; i < max; ++i)
            {
                if (i >= tmpSkillItemList.Count)
                {
                    mEquipingSkills[i].Release();
                }
                else
                {
                    mEquipingSkills[i].Refresh(tmpSkillItemList[i].SkillBase);
                }
            }
        }

        private void OnValueChangePage1(bool value)
        {
            if (value)
            {
                mSkillPage = 0;
                RefreshByPage();

                mEquipedSkills[0].SetSelected(true);
                mSelectedEuqipedSoltIdx = 0;
                mEquipingSkills[0].SetSelected(true);
                mSelectedEquipingSoltIdx = 0;
            }
        }

        private void OnValueChangePage2(bool value)
        {
            if (value)
            {
                mSkillPage = 1;
                RefreshByPage();

                mEquipedSkills[0].SetSelected(true);
                mSelectedEuqipedSoltIdx = 0;
                mEquipingSkills[0].SetSelected(true);
                mSelectedEquipingSoltIdx = 0;
            }
        }

        private void RegistEquipedSlotClick(SkillElem skillElem, int slotIdx)
        {
            UGUIEventListener.Get(skillElem.RootGo).onClick = arg => {

                if (mSelectedEuqipedSoltIdx == slotIdx)
                {
                    SetActive(mEquipingRootGo, false);
                    mSelectedEuqipedSoltIdx = -1;
                }
                else
                {
                    SetActive(mEquipingRootGo, true);
                    mSelectedEuqipedSoltIdx = slotIdx;
                    RefreshEquipingSkill();

                    mEquipingSkills[0].SetSelected(true);
                    mSelectedEquipingSoltIdx = 0;
                }
            };
        }

        private void RegistEquingSlotClick(SkillElem skillElem, int slotIdx)
        {
            UGUIEventListener.Get(skillElem.RootGo).onClick = arg =>
            {
                if (mSelectedEquipingSoltIdx == slotIdx)
                {
                    mSelectedEquipingSoltIdx = -1;
                }
                else
                {
                    mSelectedEquipingSoltIdx = slotIdx;
                }
            };
        }

        private void RegistSkillTreeClick(SkillElem skillElem)
        {
            UGUIEventListener.Get(skillElem.RootGo).onClick = arg =>
            {
                if (mSelectedEquipingSoltIdx != -1)
                {
                    List<SkillItem> tmpSkillItemList = mSkillLinkList[mSelectedEuqipedSoltIdx].SkillItems;
                    
                    if (mSelectedEquipingSoltIdx >= tmpSkillItemList.Count)
                    {
                        SkillItem tmpSkillItem = new SkillItem();
                        tmpSkillItem.Init(skillElem.SkillBase.ID, 1);
                        tmpSkillItemList.Add(tmpSkillItem);
                    }
                    else
                    {
                        tmpSkillItemList[mSelectedEquipingSoltIdx].Init(skillElem.SkillBase.ID, 1);
                    }

                    RefreshEquipingSkill();

                    if (tmpSkillItemList.Count == 1 || mSelectedEquipingSoltIdx == 0)
                    {
                        RefreshEquipedSkill();
                    }
                }
            };
        }

        private void OnBackBtnClick(PointerEventData arg)
        {
            var tmpCtrl = Game.ControllerMgr.Get<PlayerController>();
            tmpCtrl.SaveSkill();

            if (tmpCtrl.CurrSkillPage != mSkillPage)
            {
                tmpCtrl.SetCurrSkillPage(mSkillPage);

                Game.EventMgr.FireNow(this, ReferencePool.Fetch<SkillWndEvent.SkillSetChange>());
            }

            Close();
        }
      
    }
}
