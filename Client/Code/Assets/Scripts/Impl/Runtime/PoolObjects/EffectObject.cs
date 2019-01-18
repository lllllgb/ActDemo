using System.Collections;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class EffectObject : PoAttachGoBase
    {
        protected override EABType ResType => EABType.Effect;
        //
        private string mResName;
        protected override string ResName => mResName;
        //特效使用同步加载
        protected override bool IsLoadResAsync => false;

        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        public override void OnUnspawn()
        {
            base.OnUnspawn();

            mResName = string.Empty;

            if (mEffectObj)
            {
                GameObject.Destroy(mEffectObj);
            }

            mEffectRecord = null;
        }

        public override void OnRelease()
        {
            base.OnRelease();

            mResName = string.Empty;

            if (mEffectObj)
            {
                GameObject.Destroy(mEffectObj);
            }

            mEffectRecord = null;
        }

        protected override void OnResLoaded()
        {
            base.OnResLoaded();

            if (null == mGameObject)
            {
                return;
            }

            mGameObject.transform.SetParent(EffectTrans, false);
            mEffectRecord = mGameObject.GetComponent<EffectRecord>();

            if (null == mEffectRecord)
            {
                mEffectRecord = mGameObject.AddComponent<EffectRecord>();
            }
        }



        private GameObject mEffectObj;
        public Transform EffectTrans { get { return mEffectObj ? mEffectObj.transform : null; } }
        //回收时间
        public float Duration { get; set; }
        //
        private EffectRecord mEffectRecord;

        public void Initialize(string effectName)
        {
            mResName = effectName;

            if (null == mEffectObj)
            {
                mEffectObj = new GameObject("effectObj");
            }

            LoadRes();
        }

        public void Play()
        {
            if (null != mEffectRecord)
            {
                mEffectRecord.Play();
            }
        }

        public void Stop()
        {
            if (null != mEffectRecord)
            {
                mEffectRecord.Stop();
            }
        }
    }
}
