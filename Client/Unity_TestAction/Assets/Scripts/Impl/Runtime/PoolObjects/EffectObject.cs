using System.Collections;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class EffectObject : PoAttachGoBase, ACT.IActEffect
    {
        //资源类型
        protected override EABType ResType => EABType.Effect;
        //资源名
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

        //特效根节点
        private GameObject mEffectObj;
        public Transform EffectTrans { get { return mEffectObj ? mEffectObj.transform : null; } }
        //回收时间
        private float Duration { get; set; }
        //粒子系统记录器
        private EffectRecord mEffectRecord;
        //是否过期
        public bool IsInvalid { get { return Duration <= 0f; } }


        public void Init(string name, float duration, Transform parent, Vector3 pos, Quaternion rotation)
        {
            mResName = name;

            if (null == mEffectObj)
            {
                mEffectObj = new GameObject("effectObj");
            }

            LoadRes();
            Duration = duration;
            mEffectObj.transform.SetParent(parent, false);
            mEffectObj.transform.localPosition = pos;
            mEffectObj.transform.localRotation = rotation;
        }
        
        public void Play()
        {
            if (null != mEffectRecord)
            {
                mEffectRecord.Play();
            }
        }

        public void Update(float deltaTime)
        {
            Duration -= deltaTime;
        }

        public void Stop()
        {
            if (null != mEffectRecord)
            {
                mEffectRecord.Stop();
            }
        }

        public void Dispose()
        {
            var tmpPool = Game.PoolMgr.GetObjectPool<EffectObject>() as ObjectPoolBase;
            tmpPool.Unspawn(this);
        }
    }
}
