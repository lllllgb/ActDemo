using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Utility = AosBaseFramework.Utility;

namespace AosHotfixRunTime
{
    public class CameraMgr : AosHotfixFramework.Singleton<CameraMgr>
    {
        //主相机
        public GameObject CameraRootGo { get; private set; }
        public GameObject MainCameraGo { get; private set; }
        public Camera MainCamera { get; private set; }

        //UI相机
        public GameObject UIRootGo { get; private set; }
        public GameObject UICanvasRootGo { get; private set; }

        //HUD相机
        public GameObject HudRootGo { get; private set; }
        public GameObject HudInfoCanvasRootGo { get; private set; }
        public GameObject HudPopupCanvasRootGo { get; private set; }
        public Camera HudCamera { get; private set; }

        public void Init(GameObject go)
        {
            CameraRootGo = go;

            GameObject tmpGo = Utility.GameObj.Find(go, "MainCamera");
            MainCameraGo = tmpGo;
            MainCamera = tmpGo.GetComponent<Camera>();
        }


        public void InitUICamera(GameObject go)
        {
            UIRootGo = go;
            UICanvasRootGo = Utility.GameObj.Find(go, "CanvasRoot");
        }



        public void InitHudCamera(GameObject go)
        {
            HudRootGo = go;
            HudInfoCanvasRootGo = Utility.GameObj.Find(go, "HudInfoCanvasRoot");
            HudPopupCanvasRootGo = Utility.GameObj.Find(go, "HudPopupCanvasRoot");
            HudCamera = Utility.GameObj.Find<Camera>(go, "HUDCamera");
            HudCamera.cullingMask = GameLayer.LayerMask_HudInfo | GameLayer.LayerMask_HudPopup;
        }

        public void ResetHudCamera()
        {
            HudCamera.cullingMask = GameLayer.LayerMask_HudInfo | GameLayer.LayerMask_HudPopup;
        }
    }
}
