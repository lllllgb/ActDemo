using System;
using System.Collections.Generic;
using AosHotfixFramework;
using UnityEngine;

namespace AosHotfixRunTime
{
    public static class AtlasHelper
    {
        public const string CommonButton = "commonbutton";
        public const string CommonImage = "commonimage";
        public const string CreateRole = "createrole";
        public const string FightMain = "fightmain";
        public const string Loading = "loading";
        public const string PveInstanceFinish = "pveinstancefinish";
        public const string SceneUI = "sceneui";
        public const string WareHouse = "wareHouse";
        public const string City = "city";

        public static Sprite GetSprite(string atlasName, string spriteName)
        {
            Sprite tmpSpr = Game.ResourcesMgr.GetAssetByType<Sprite>(EABType.Atlas, atlasName, spriteName);

            if (null == tmpSpr)
            {
                Logger.LogError($"图集 {atlasName} 未加载 / 图集没有图片 {spriteName}");
            }

            return tmpSpr;
        }
    }
}
