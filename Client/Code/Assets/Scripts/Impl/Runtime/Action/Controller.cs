using System;
using System.Collections;
using UnityEngine;

namespace ACT
{
    public class Controller
    {
        IActUnit mOwner;
        ActionStatus mActionStatus;
        float mCameraModify = 0.0f;
        bool mInputLocked = false;
        Vector3 mCameraPosCache = Vector3.zero;
        Vector3 mCameraOffset = Vector3.zero;
        InputBox mInputBox = null;

        public bool InputLocked { get { return mInputLocked; } }

        Transform mCameraTag;
        Transform mCameraTarget;
        public Vector3 CameraPos = new Vector3(0, 1, -5);
        public Vector3 CameraLookAtOffset = new Vector3(0, 1, 0);

        public float CameraModify { get { return mCameraModify; } }
        public Vector3 CameraOffset { set { mCameraOffset = value; } }


        // Use this for initialization
        public void Init(IActUnit actUnit, Transform cameraTag)
        {
            mOwner = actUnit;
            mCameraTag = cameraTag;
            mCameraTarget = mOwner.UGameObject.transform;
            mActionStatus = mOwner.ActStatus;

            OnEnable();
        }

        void OnEnable()
        {
            if (mActionStatus != null)
            {
                mInputBox = new InputBox(mOwner, this);
                ActionHelper.GInputBox = mInputBox;
            }
        }

        void OnDestroy()
        {
            if (mInputBox != null)
                mInputBox.ResetInput();
        }

        // LateUpdate is called after all Update functions have been called
        public void LateUpdate()
        {
            if (!mInputLocked)
            {
                CheckActionInput(Time.deltaTime);

                mInputBox.Update(Time.deltaTime);
            }

            UpdateCamera(Time.deltaTime);
        }

        void CheckActionInput(float deltaTime)
        {
            if (mInputBox == null || mActionStatus == null || mActionStatus.ActiveAction == null || mActionStatus.HasQueuedAction)
                return;

            int interruptIdx = 0;
            foreach (Data1.ActionInterrupt interrupt in mActionStatus.ActiveAction.ActionInterrupts)
            {
                if (!mActionStatus.GetInterruptEnabled(interruptIdx++))
                    continue;

                bool checker = false;
                if (interrupt.NoInput)
                    checker = true;
                else
                {
                    if (interrupt.CheckInput1 == false)
                        continue;

                    if (interrupt.InputKey1 == 0)
                    {
                        if (interrupt.SkillID == 0)
                            continue;
                    }
                    else if (interrupt.CheckInput2)
                    {
                        // or
                        checker =
                            mInputBox.HasInput(interrupt.InputKey1, interrupt.InputType1, deltaTime) ||
                            mInputBox.HasInput(interrupt.InputKey2, interrupt.InputType2, deltaTime) ||
                            mInputBox.HasInput(interrupt.InputKey3, interrupt.InputType3, deltaTime) ||
                            mInputBox.HasInput(interrupt.InputKey4, interrupt.InputType4, deltaTime);
                    }
                    else
                    {
                        // and
                        checker =
                            (interrupt.InputKey1 == 0 || mInputBox.HasInput(interrupt.InputKey1, interrupt.InputType1, deltaTime)) &&
                            (interrupt.InputKey2 == 0 || mInputBox.HasInput(interrupt.InputKey2, interrupt.InputType2, deltaTime)) &&
                            (interrupt.InputKey3 == 0 || mInputBox.HasInput(interrupt.InputKey3, interrupt.InputType3, deltaTime)) &&
                            (interrupt.InputKey4 == 0 || mInputBox.HasInput(interrupt.InputKey4, interrupt.InputType4, deltaTime));
                    }
                }

                // pass the key input, then check the other conditions.
                if (checker && (!interrupt.CheckAllCondition || mActionStatus.CheckActionInterrupt(interrupt)))
                {
                    bool success = mActionStatus.LinkAction(interrupt, null);
                    if (success)
                        break;
                }
            }
        }

        Vector3 mBeginOffset = Vector3.zero;
        Vector3 mAddtiveOffset = Vector3.zero;
        float mAdjustTime = 3.0f;
        float mAdjustLeftTime = 0.0f;

        void UpdateCamera(float deltaTime)
        {
            if (mCameraTag == null)
                return;

            if (mCameraPosCache != CameraPos)
            {
                mCameraModify = Mathf.Atan2(CameraPos.x, CameraPos.z);
                mCameraPosCache = CameraPos;
            }

            Vector3 targetPos = mOwner.Position;
            mCameraTag.position = targetPos + CameraPos;
            //mCameraTag.LookAt(targetPos + CameraLookAtOffset);
            mCameraTag.Translate(mCameraOffset);
        }

        void LockInput(bool flag)
        {
            mInputLocked = flag;

            mInputBox.ResetInput();

            CheckActionInput(Time.deltaTime);
        }
    }
}
