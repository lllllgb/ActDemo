using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public interface ICameraActionManager
    {
        void Init(Camera camera, GameObject closeupGo, GameObject shakeGo);

        void StartAction(int actionID);

        void Reset();

        void Release();
    }

    internal class CameraActionManager : GameModuleBase, ICameraActionManager
    {

        internal override int Priority => base.Priority;


        CameraActioner mCameraAction = new CameraActioner();

        internal override void Update(float deltaTime)
        {
            mCameraAction.Update(deltaTime);
        }

        internal override void LateUpdate(float deltaTime)
        {
        }

        internal override void Shutdown()
        {
        }

        public void Init(Camera camera, GameObject closeupGo, GameObject shakeGo)
        {
            mCameraAction.Init(camera, closeupGo, shakeGo);
        }

        public void StartAction(int actionID)
        {
            mCameraAction.StartAction(actionID);
        }

        public void Reset()
        {
            mCameraAction.Reset();
        }

        public void Release()
        {
            mCameraAction.Release();
        }
    }
}
