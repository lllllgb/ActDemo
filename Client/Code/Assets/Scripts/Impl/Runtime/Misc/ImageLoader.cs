using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AosHotfixFramework;
using AosBaseFramework;

namespace AosHotfixRunTime
{
    public class ImageLoader : ReferenceDisposer
    {
        public enum EIconType
        {
            Unit,
            Skill,
            Item,
            Buff,
        }

        static Dictionary<EIconType, EABType> sIconType2AbType = new Dictionary<EIconType, EABType>
        {
            {EIconType.Unit, EABType.IconUnit},
            {EIconType.Skill, EABType.IconSkill},
            {EIconType.Item, EABType.IconItem},
            {EIconType.Buff, EABType.IconBuff},
        };

        bool mIsReleased = false;
        EABType mResType;
        string mResName;
        string mAssetName;
        Image mImage;
        Action mLoadedHandle;

        public ImageLoader()
        {
        }

        public void Load(EIconType type, string name, Image image, Action loadedHandle = null, bool async = true, string assetName = null)
        {
            EABType tmpAbType;

            if (!sIconType2AbType.TryGetValue(type, out tmpAbType))
            {
                Logger.LogError($"ImageLoader.Load 类型不正确  type = {type}");
                return;
            }

            LoadByAbType(tmpAbType, name, image, loadedHandle, async, assetName);
        }

        public void LoadByAbType(EABType type, string name, Image image, Action loadedHandle = null, bool async = true, string assetName = null)
        {
            mIsReleased = false;

            mResType = type;
            mResName = name;
            mImage = image;
            mAssetName = assetName;
            mLoadedHandle = loadedHandle;
            Utility.GameObj.SetActive(image, false);

            if (async)
            {
                LoadResAsync(type, name);
            }
            else
            {
                LoadResSync(type, name);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            mIsReleased = true;
            ReleaseRes();
        }

        //异步加载
        async void LoadResAsync(EABType type, string name)
        {
            await Game.ResourcesMgr.LoadBundleByTypeAsync(type, name);

            if (type != mResType || !name.Equals(mResName))
            {
                Game.ResourcesMgr.UnLoadBundleByType(type, name);
                return;
            }

            if (mIsReleased)
            {
                ReleaseRes();
                return;
            }

            OnResLoaded();
        }

        //同步加载
        void LoadResSync(EABType type, string name)
        {
            Game.ResourcesMgr.LoadBundleByType(type, name);
            OnResLoaded();
        }

        void OnResLoaded()
        {
            if (mAssetName!=null)
            {
                var tmpAssets = Game.ResourcesMgr.GetAssetByType<Sprite>(mResType, mResName, mAssetName);
                if (null == tmpAssets || null == mImage)
                {
                    return;
                }
                mImage.sprite = tmpAssets;
            }
            else
            {
                var tmpAsset = Game.ResourcesMgr.GetAssetByType<Sprite>(mResType, mResName);
                if (null == tmpAsset || null == mImage)
                {
                    return;
                }
                mImage.sprite = tmpAsset;
            }                                
            Utility.GameObj.SetActive(mImage, true);
            mLoadedHandle?.Invoke();
        }

        void ReleaseRes()
        {
            if (EABType.Invalid != mResType && !string.IsNullOrEmpty(mResName))
            {
                Game.ResourcesMgr.UnLoadBundleByType(mResType, mResName);
                mResType = EABType.Invalid;
                mResName = string.Empty;
            }

            if (null != mImage)
            {
                mImage.sprite = null;
                Utility.GameObj.SetActive(mImage,false);
            }
        }
    }
}
