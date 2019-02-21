using System;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using UnityStandardAssets.ImageEffects;
using AosBaseFramework;

namespace AosHotfixRunTime
{
    public static class CameraHelper
    {
        //主相机
        public static GameObject CameraRootGo { get; private set; }
        public static GameObject MainCameraGo { get; private set; }
        public static Camera MainCamera { get; private set; }

        //UI相机
        public static GameObject UIRootGo { get; private set; }
        public static GameObject UICanvasRootGo { get; private set; }

        //HUD相机
        public static GameObject HudRootGo { get; private set; }
        public static GameObject HudInfoCanvasRootGo { get; private set; }
        public static GameObject HudPopupCanvasRootGo { get; private set; }
        public static Camera HudCamera { get; private set; }

        public static void Init(GameObject go)
        {
            CameraRootGo = go;

            GameObject tmpGo = Utility.GameObj.Find(go, "MainCamera");
            MainCameraGo = tmpGo;
            MainCamera = tmpGo.GetComponent<Camera>();
            GameDefine.CurrentCamera = MainCamera;
        }


        public static void InitUICamera(GameObject go)
        {
            UIRootGo = go;
            UICanvasRootGo = Utility.GameObj.Find(go, "CanvasRoot");
        }



        public static void InitHudCamera(GameObject go)
        {
            HudRootGo = go;
            HudInfoCanvasRootGo = Utility.GameObj.Find(go, "HudInfoCanvasRoot");
            HudPopupCanvasRootGo = Utility.GameObj.Find(go, "HudPopupCanvasRoot");
            HudCamera = Utility.GameObj.Find<Camera>(go, "HUDCamera");
            HudCamera.cullingMask = GameLayer.LayerMask_HUD | GameLayer.LayerMask_HudPopup;
        }

        public static void ResetHudCamera()
        {
            HudCamera.cullingMask = GameLayer.LayerMask_HUD | GameLayer.LayerMask_HudPopup;
        }
    }
}
