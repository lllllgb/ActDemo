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
            public Text SkillName;
            public Image Checkmark;
            public GameObject LinkLineGo;
            public SkillBase SkillBase { get; private set; }


            public void Init(GameObject go)
            {
                RootGo = go;
                SkillTge = go.GetComponent<Toggle>();
                IconImg = Find<Image>(go, "Image_Icon");
                SkillName = Find<Text>(go, "Text_SkillName");
                Checkmark = Find<Image>(go, "Checkmark");
                LinkLineGo = Find(go, "Image_LinkLine");
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

                if (null != SkillName)
                {
                    SkillName.text = data.Name;
                }
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
                if (SkillTge)
                {
                    SkillTge.isOn = flag;
                }
            }
        }

        public override string BundleName { get { return "SkillWnd"; } }

        protected override bool IsLoadAsync => false;

        Toggle mSkillPage1Toggle;
        List<SkillElem> mEquipedSkills = new List<SkillElem>();
        GameObject mEquipingRootGo;
        List<SkillElem> mEquipingSkills = new List<SkillElem>();
        UIGridTool mSkillTreeGT;
        List<SkillElem[]> mSkillTreeItems = new List<SkillElem[]>();

        int mSkillPage;
        int mSelectedEuqipedSoltIdx;
        List<SkillItemLink> mSkillLinkList;

        SkillDescWnd mSkillDescWnd;

        protected override void AfterInit()
        {
            base.AfterInit();

            mSkillPage1Toggle = Find<Toggle>("Toggle_Page1");
            mSkillPage1Toggle.onValueChanged.AddListener(OnValueChangePage1);
            var tmpPage2 = Find<Toggle>("Toggle_Page2");
            tmpPage2.onValueChanged.AddListener(OnValueChangePage2);

            GameObject tmpSkillTemplate = Find("SkillItemTemplate");
            GameObject tmpSkillTreeGo = Find($"ScrollView_SkillTree");

            if (null != tmpSkillTreeGo)
            {
                mSkillTreeGT = new UIGridTool(Find(tmpSkillTreeGo, "Content"), tmpSkillTemplate);
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

                    RegistEquipingSkillDragBegin(tmpSkillElem, i);
                    RegistEquipingSkillDrag(tmpSkillElem);
                    RegistEquipingSkillDragEnd(tmpSkillElem, i);
                    RegistEquipingSkillDrop(tmpSkillElem, i);
                }
            }

            RegisterEventClick(Find("Button_Back"), OnBackBtnClick);
        }

        protected override void InitSubWindow()
        {
            base.InitSubWindow();

            mSkillDescWnd = CreateChildWindow<SkillDescWnd>(mGameObejct.transform, Find("SkillDescPanel"));
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

            int tmpMaxRow = 0;
            foreach (var elem in tmpTreeType2Skills)
            {
                if (elem.Value.Count > tmpMaxRow)
                {
                    tmpMaxRow = elem.Value.Count;
                }
            }

            mSkillTreeGT.GenerateElem(tmpMaxRow);

            for (int i = 0, max = 3; i < max; ++i)
            {
                List<SkillBase> tmpSkills = null;

                if (!tmpTreeType2Skills.TryGetValue(i, out tmpSkills))
                {
                    continue;
                }

                for (int j = 0, jmax = tmpMaxRow; j < jmax; ++j)
                {
                    GameObject tmpRowGo = mSkillTreeGT.Get(j);
                    SkillElem[] tmpRowElems = null;

                    if (j < mSkillTreeItems.Count)
                    {
                        tmpRowElems = mSkillTreeItems[j];
                    }
                    else
                    {
                        tmpRowElems = new SkillElem[3];
                        mSkillTreeItems.Add(tmpRowElems);
                    }

                    SkillElem tmpSkillElem = tmpRowElems[i];

                    if (null == tmpSkillElem)
                    {
                        tmpSkillElem = new SkillElem();
                        tmpSkillElem.Init(Find(tmpRowGo, $"SkillItem{i}"));
                        tmpRowElems[i] = tmpSkillElem;
                    }

                    if (j < tmpSkills.Count)
                    {
                        SetActive(tmpSkillElem.RootGo, true);
                        tmpSkillElem.Refresh(tmpSkills[j]);
                        SetActive(tmpSkillElem.LinkLineGo, j != tmpSkills.Count - 1);

                        RegistSkillTreeDragBegin(tmpSkillElem);
                        RegistSkillTreeDrag(tmpSkillElem);
                        RegistSkillTreeDragEnd(tmpSkillElem);
                        RegistSkillTreeClick(tmpSkillElem);
                    }
                    else
                    {
                        SetActive(tmpSkillElem.RootGo, false);
                    }
                }
            }
        }

        private void RefreshByPage()
        {
            mSkillLinkList = Game.ControllerMgr.Get<PlayerController>().GetSkillLinkList(mSkillPage);
            RefreshEquipedSkill();
            mSelectedEuqipedSoltIdx = -1;
            SetActive(mEquipingRootGo, false);

            for (int i = 0, max = mEquipedSkills.Count; i < max; ++i)
            {
                mEquipedSkills[i].SetSelected(false);
            }
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
            }
        }

        private void OnValueChangePage2(bool value)
        {
            if (value)
            {
                mSkillPage = 1;
                RefreshByPage();
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
                }
            };
        }

        SkillElem mDragingSkill;
        private void RegistSkillTreeDragBegin(SkillElem skillElem)
        {
            UGUIDrogListener.Get(skillElem.RootGo).onBeginDrag = arg => {

                if (UIDragHelper.Instance.BeginDrag(arg, skillElem.IconImg.gameObject, mGameObejct.transform))
                {
                    mDragingSkill = skillElem;
                }
            };
        }

        private void RegistSkillTreeDrag(SkillElem skillElem)
        {
            UGUIDrogListener.Get(skillElem.RootGo).onDrag =  arg => {
                UIDragHelper.Instance.Draging(arg);
            };
        }

        private void RegistSkillTreeDragEnd(SkillElem skillElem)
        {
            UGUIDrogListener.Get(skillElem.RootGo).onEndDrag = arg => {

                if (UIDragHelper.Instance.EndDrag(arg))
                {
                    mDragingSkill = null;
                }
            };
        }

        private void RegistSkillTreeClick(SkillElem skillElem)
        {
            UGUIEventListener.Get(skillElem.RootGo).onClick = arg => {
                mSkillDescWnd.SetSkillData(skillElem.SkillBase.ID);
                mSkillDescWnd.Show();
            };
        }


        private void RegistEquipingSkillDragBegin(SkillElem skillElem, int slotIdx)
        {
            UGUIDrogListener.Get(skillElem.RootGo).onBeginDrag = arg => {

                if (null == skillElem.SkillBase)
                {
                    return;
                }

                if (UIDragHelper.Instance.BeginDrag(arg, skillElem.IconImg.gameObject, mGameObejct.transform))
                {
                }
            };
        }

        private void RegistEquipingSkillDrag(SkillElem skillElem)
        {
            UGUIDrogListener.Get(skillElem.RootGo).onDrag = arg => {
                UIDragHelper.Instance.Draging(arg);
            };
        }

        private void RegistEquipingSkillDragEnd(SkillElem skillElem, int slotIdx)
        {
            UGUIDrogListener.Get(skillElem.RootGo).onEndDrag = arg => {

                if (UIDragHelper.Instance.EndDrag(arg))
                {
                    if (arg.pointerCurrentRaycast.gameObject != skillElem.RootGo)
                    {
                        List<SkillItem> tmpSkillItemList = mSkillLinkList[mSelectedEuqipedSoltIdx].SkillItems;

                        if (slotIdx < tmpSkillItemList.Count)
                        {
                            tmpSkillItemList.RemoveAt(slotIdx);
                        }

                        RefreshEquipingSkill();

                        if (tmpSkillItemList.Count == 0 || slotIdx == 0)
                        {
                            RefreshEquipedSkill();
                        }
                    }
                }
            };
        }

        private void RegistEquipingSkillDrop(SkillElem skillElem, int slotIdx)
        {
            UGUIDrogListener.Get(skillElem.RootGo).onDrop = arg => {

                if (UIDragHelper.Instance.OnDrop(arg))
                {
                    if (null == mDragingSkill)
                    {
                        return;
                    }

                    List<SkillItem> tmpSkillItemList = mSkillLinkList[mSelectedEuqipedSoltIdx].SkillItems;
                    SkillItem tmpSkillItem = null;

                    if (slotIdx >= tmpSkillItemList.Count)
                    {
                        tmpSkillItem = new SkillItem();
                        tmpSkillItemList.Add(tmpSkillItem);
                    }
                    else
                    {
                        tmpSkillItem = tmpSkillItemList[slotIdx];
                    }

                    tmpSkillItem.Init(mDragingSkill.SkillBase.ID, 1);
                    RefreshEquipingSkill();

                    if (tmpSkillItemList.Count == 1 || slotIdx == 0)
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
            }

            Game.EventMgr.FireNow(this, ReferencePool.Fetch<SkillWndEvent.SkillSetChange>());
            Close();
        }
      
    }
}
