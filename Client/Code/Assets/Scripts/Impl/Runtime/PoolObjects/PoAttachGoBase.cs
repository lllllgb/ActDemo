using System;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine;

namespace AosHotfixRunTime
{
    public abstract class PoAttachGoBase : IPoolObject
    {
        public long Id { get; set; }
        public bool IsFromPool { get; set; }

        public virtual void OnInit()
        {
        }

        public virtual void OnSpawn()
        {
            mIsInvalid = false;
            LoadRes();
        }

        public virtual void OnUnspawn()
        {
            mIsInvalid = true;
            ReleaseRes();
        }

        public virtual void OnRelease()
        {
            mIsInvalid = true;
            ReleaseRes();
        }

        protected virtual void OnResLoaded()
        {

        }

        //资源类型
        protected virtual EABType ResType { get; }
        //资源名字
        protected virtual string ResName { get; }
        //是否异步加载
        protected virtual bool IsLoadResAsync { get { return true; } }
        //是否已无效
        private bool mIsInvalid = false;
        //资源体
        protected GameObject mGameObject;

        //加载资源
        protected void LoadRes()
        {
            if (string.IsNullOrEmpty(ResName))
            {
                return;
            }

            //先从资源池里取
            GameObject tmpGo = Game.ResourcesMgr.ResPool.Spawn(ResType, ResName);

            if (null != tmpGo)
            {
                InitGameObject(tmpGo);
                OnResLoaded();
            }
            else
            {
                if (IsLoadResAsync)
                {
                    ReloadResAsync();
                }
                else
                {
                    ReloadRes();
                }
            }
        }

        //加载后初始资源
        private void InitGameObject(GameObject go)
        {
            if (null == go)
            {
                return;
            }

            mGameObject = go;
            mGameObject.transform.localPosition = Vector3.zero;
            mGameObject.transform.localRotation = Quaternion.identity;
            mGameObject.transform.localScale = Vector3.one;
        }

        //重新异步加载资源
        private async void ReloadResAsync()
        {
            EABType tmpResType = ResType;
            string tmpResName = ResName;
            await Game.ResourcesMgr.LoadBundleByTypeAsync(tmpResType, tmpResName);
            GameObject tmpGo = Game.ResourcesMgr.GetAssetByType<GameObject>(tmpResType, tmpResName);
            tmpGo = Hotfix.Instantiate(tmpGo);
            InitGameObject(tmpGo);

            if (mIsInvalid)
            {
                ReleaseRes(tmpResType, tmpResName);
                return;
            }

            OnResLoaded();
        }

        //重新同步加载资源
        private void ReloadRes()
        {
            Game.ResourcesMgr.LoadBundleByType(ResType, ResName);
            GameObject tmpGo = Game.ResourcesMgr.GetAssetByType<GameObject>(ResType, ResName);
            tmpGo = Hotfix.Instantiate(tmpGo);
            InitGameObject(tmpGo);
            OnResLoaded();
        }

        //释放资源
        void ReleaseRes(EABType resType = EABType.Invalid, string resName = null)
        {
            if (null != mGameObject)
            {
                Game.ResourcesMgr.ResPool.Despawn(EABType.Invalid != resType ? resType : this.ResType,
                    null != resName ? resName : this.ResName, mGameObject);

                mGameObject = null;
            }
        }
    }
}
