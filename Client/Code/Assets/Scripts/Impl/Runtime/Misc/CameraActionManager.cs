using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using System;

namespace AosHotfixRunTime
{
    public class CameraActionManager
    {
        [Flags]
        public enum EActionType
        {
            Invalid = 0,
            CloseUp = 1,
            Shake = 2,
            Lighteness = 4,
        }

        Camera mCamera;
        bool mIsRunning = false;
        EActionType mActionType = EActionType.Invalid;
        float mTotalTime = 0;
        float mRunningTime = 0;

        GameObject mCloseupGo;
        float mOrthographicSize;
        GameObject mShakeGo;
        Vector3 mShakeInitPos;

        #region closeup
        float mCloseUpPreTime;
        float mCloseUpKeepTime;
        float mCloseUpOutTime;
        float mCloseUpFactor;
        float mCloseOffset;
        #endregion

        #region shake
        float mShakeDelayTime;
        float mShakeKeepTime;
        float mShakeAmplitude;
        float mShakeFrequence;
        float mShakeLastTime;
        #endregion

        #region lighteness
        float mLightenessPreTime;
        float mLightenessKeepTime;
        float mLightenessOutTime;
        float mLightenessFactor;
        Color mLightenessOffset;
        #endregion


        public CameraActionManager()
        {
        }

        public void Init(Camera camera, GameObject closeupGo, GameObject shakeGo)
        {
            mCamera = camera;
            mCloseupGo = closeupGo;
            mShakeGo = shakeGo;

            mOrthographicSize = mCamera.orthographicSize;
            mShakeInitPos = mShakeGo.transform.localPosition;
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

            if ((mActionType & EActionType.Lighteness) == EActionType.Lighteness)
            {
                SetData_Lighteness(tmpCameraAction.Lighteness);
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

                if ((mActionType & EActionType.Lighteness) == EActionType.Lighteness)
                {
                    Update_Lighteness(deltaTime);
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
            mCamera.orthographicSize = mOrthographicSize;

            var tmpEventArg = ReferencePool.Fetch<CameraActionEvent.ModifySceneMaskColor>();
            tmpEventArg.Data = Color.clear;
            Game.EventMgr.FireNow(this, tmpEventArg);
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
            if (null == closeupData || closeupData.values.Count < 4)
            {
                Logger.LogError("特写参数错误");
                return;
            }

            mCloseUpFactor = closeupData.values[0] * 0.01f;
            mCloseUpPreTime = closeupData.values[1] * 0.001f;
            mCloseUpKeepTime = closeupData.values[2] * 0.001f;
            mCloseUpOutTime = closeupData.values[3] * 0.001f;

            mCloseOffset = mOrthographicSize * (1 - mCloseUpFactor);
            Update_Closeup(0f);
        }

        private void Update_Closeup(float deltaTime)
        {
            float tmpRatio = 0f;

            if (mRunningTime > mCloseUpPreTime + mCloseUpKeepTime + mCloseUpOutTime)
            {
                return;
            }
            else if (mRunningTime >= mCloseUpPreTime + mCloseUpKeepTime)
            {
                tmpRatio = mCloseUpOutTime > 0f ? (mRunningTime - mCloseUpPreTime - mCloseUpKeepTime) / mCloseUpOutTime : 1f;
                mCamera.orthographicSize = mOrthographicSize - mCloseOffset + mCloseOffset * tmpRatio;
            }
            else if (mRunningTime <= mCloseUpPreTime)
            {
                tmpRatio = mCloseUpPreTime > 0f ? mRunningTime / mCloseUpPreTime : 1f;
                mCamera.orthographicSize = mOrthographicSize - mCloseOffset * tmpRatio;
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

            mShakeAmplitude = shakeData.values[0] * 0.01f;
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

        #region mask

        private void SetData_Lighteness(CameraActionBase.Param lightenessData)
        {
            if (null == lightenessData || lightenessData.values.Count < 4)
            {
                Logger.LogError("亮度参数错误");
                return;
            }

            mLightenessFactor = lightenessData.values[0] * 0.01f;
            mLightenessPreTime = lightenessData.values[1] * 0.001f;
            mLightenessKeepTime = lightenessData.values[2] * 0.001f;
            mLightenessOutTime = lightenessData.values[3] * 0.001f;

            mLightenessOffset = Color.black * (1 - mLightenessFactor);
            Update_Lighteness(0f);
        }

        private void Update_Lighteness(float deltaTime)
        {
            float tmpRatio = 0f;

            if (mRunningTime > mLightenessPreTime + mLightenessKeepTime + mLightenessOutTime)
            {
                return;
            }
            else if (mRunningTime > mLightenessPreTime + mLightenessKeepTime)
            {
                tmpRatio = mLightenessOutTime > 0f ? (mRunningTime - mLightenessPreTime - mLightenessKeepTime) / mLightenessOutTime : 1f;

                var tmpEventArg = ReferencePool.Fetch<CameraActionEvent.ModifySceneMaskColor>();
                tmpEventArg.Data = mLightenessOffset * (1 - tmpRatio);
                Game.EventMgr.FireNow(this, tmpEventArg);
            }
            else if (mRunningTime <= mLightenessPreTime)
            {
                tmpRatio = mLightenessPreTime > 0f ? mRunningTime / mLightenessPreTime : 1f;
                var tmpEventArg = ReferencePool.Fetch<CameraActionEvent.ModifySceneMaskColor>();
                tmpEventArg.Data = mLightenessOffset * tmpRatio;
                Game.EventMgr.FireNow(this, tmpEventArg);
            }

        }

        #endregion
    }
}
