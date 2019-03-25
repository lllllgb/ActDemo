using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using System;

namespace AosHotfixRunTime
{
    public interface ICameraAction
    {
        void StartAction(int actionID);
    }

    public class CameraActionManager
    {
        [Flags]
        public enum EActionType
        {
            Invalid = 0,
            MotionBlur = 1,
            CloseUp = 2,
            Shake = 4,
            Animation = 8,
        }

        Camera mCamera;
        bool mIsRunning = false;
        EActionType mActionType = EActionType.Invalid;
        float mTotalTime = 0;
        float mRunningTime = 0;

        GameObject mCloseupGo;
        Vector3 mCloseupInitPos;
        GameObject mShakeGo;
        Vector3 mShakeInitPos;

        #region closeup

        float mCloseUpPreTime;
        float mCloseUpKeepTime;
        float mCloseUpRearTime;
        Vector3 mCloseUpDis;

        #endregion

        #region shake
        float mShakeDelayTime;
        float mShakeKeepTime;
        float mShakeAmplitude;
        float mShakeFrequence;
        float mShakeLastTime;
        #endregion


        public CameraActionManager()
        {
        }

        public void Init(Camera camera, GameObject closeupGo, GameObject shakeGo)
        {
            mCamera = camera;
            mCloseupGo = closeupGo;
            mShakeGo = shakeGo;
        }

        public void StartAction(int actionID)
        {
            CameraActionBase tmpCameraAction = CameraActionBaseManager.instance.Find(actionID);

            if (null == tmpCameraAction)
            {
                Logger.LogError($"找不到相机动作ID -> {actionID}");
                return;
            }

            mActionType = (EActionType)tmpCameraAction.type;
            mTotalTime = tmpCameraAction.time * 0.001f;
            mIsRunning = mTotalTime > 0f;
            mRunningTime = 0f;

            if ((mActionType & EActionType.CloseUp) == EActionType.CloseUp)
            {
                SetData_Closeup(tmpCameraAction.closeup);
            }

            if ((mActionType & EActionType.Shake) == EActionType.Shake)
            {
                SetData_Shake(tmpCameraAction.shake);
            }
        }

        public void Update(float deltaTime)
        {
            if (mIsRunning)
            {
                mRunningTime += deltaTime;

                if ((mActionType & EActionType.CloseUp) == EActionType.CloseUp)
                {
                    Update_Closeup(deltaTime);
                }

                if ((mActionType & EActionType.Shake) == EActionType.Shake)
                {
                    Update_Shake(deltaTime);
                }

                if (mRunningTime >= mTotalTime)
                {
                    Reset();
                    OnFinish();
                }
            }
        }

        public void Reset()
        {
            mIsRunning = false;

        }

        public void Release()
        {
        }

        private void OnFinish()
        {
        }

        #region closeup

        private void SetData_Closeup(CameraActionBase.Param closeupData)
        {
            if (null == closeupData || closeupData.values.Count < 6)
            {
                Logger.LogError("特写参数错误");
                return;
            }

            mCloseUpDis = new Vector3(closeupData.values[0], closeupData.values[1], closeupData.values[2]);
            mCloseUpPreTime = closeupData.values[3] * 0.001f;
            mCloseUpKeepTime = closeupData.values[4] * 0.001f;
            mCloseUpRearTime = closeupData.values[5] * 0.001f;
        }

        private void Update_Closeup(float deltaTime)
        {
            float tmpRatio = 0f;

            if (mRunningTime > mCloseUpPreTime + mCloseUpKeepTime + mCloseUpRearTime)
            {
                return;
            }
            else if (mRunningTime > mCloseUpPreTime + mCloseUpKeepTime)
            {
                tmpRatio = mCloseUpRearTime > 0f ? deltaTime / mCloseUpRearTime : 1f;
                tmpRatio *= -1;
            }
            else if (mRunningTime <= mCloseUpPreTime)
            {
                tmpRatio = mCloseUpPreTime > 0f ? deltaTime / mCloseUpPreTime : 1f;
            }

            if (0f != tmpRatio)
            {
                mCloseupGo.transform.localPosition = mCloseupInitPos + mCloseUpDis * tmpRatio;
            }
        }

        #endregion


        #region shake

        private void SetData_Shake(CameraActionBase.Param shakeData)
        {
            if (null == shakeData || shakeData.values.Count < 4)
            {
                Logger.LogError("震屏参数错误");
                return;
            }

            mShakeAmplitude = shakeData.values[0] * 0.1f;
            mShakeFrequence = shakeData.values[1] * 0.001f;
            mShakeDelayTime = shakeData.values[2] * 0.001f;
            mShakeKeepTime = shakeData.values[3] * 0.001f;
            mShakeLastTime = 0f;
        }

        private void Update_Shake(float deltaTime)
        {
            if (mRunningTime > mShakeDelayTime + mShakeKeepTime)
            {
                return;
            }
            else if (mRunningTime >= mShakeDelayTime)
            {
                if (mRunningTime - mShakeLastTime >= mShakeFrequence)
                {
                    mShakeLastTime = mRunningTime;
                    Vector3 tmpPos = mShakeInitPos;
                    tmpPos.x += UnityEngine.Random.Range(-1f, 1f) * mShakeAmplitude;
                    tmpPos.y += UnityEngine.Random.Range(-1f, 1f) * mShakeAmplitude;
                    mShakeGo.transform.localPosition = tmpPos;
                }
            }
        }

        #endregion
    }
}
