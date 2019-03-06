using System;
using System.Collections;
using UnityEngine;

namespace ACT
{
    public class InputBoxExtend
    {
        static bool mJoystickPressed = false;
        static public bool JoystickPressed { get { return mJoystickPressed; } }

        static Vector2 mJoystickDelta = Vector2.zero;
        static public Vector2 JoystickDelta { get { return (Time.timeScale != 0) ? mJoystickDelta : Vector2.zero; } }
    }

    public class InputBox
    {
        [Serializable]
        public class KeyState
        {
            public string AxisName;
            public float PressedTime = 10.0f;
            public float ReleasedTime = 10.0f;
            public int Pressed = 0;         // 0=release 1=click 2=double click
            public int Operation = 0;       // for action input binding...
        };

        public KeyState[] KeyStates = new KeyState[(int)EKeyList.KL_Max];
        const float DoubleClickTime = 0.4f;
        const float LongPressedTime = 0.3f;

        IActUnit mOwner;
        ActionStatus mActionStatus;
        Controller mController;
        Vector2 mInputVector = Vector2.zero;
        Vector2 mLastInputVector = Vector2.one;
        bool mMoveKeyDown = false;

        // Use this for initialization
        public InputBox(IActUnit owner, Controller controller)
        {
            mOwner = owner;
            mController = controller;
            mActionStatus = mOwner.ActStatus;

            for (int i = 0; i < KeyStates.Length; i++)
                KeyStates[i] = new KeyState();
            KeyStates[(int)EKeyList.KL_Jump].AxisName = "Jump";
            KeyStates[(int)EKeyList.KL_Attack].AxisName = "Attack";
            KeyStates[(int)EKeyList.KL_SubAttack].AxisName = "SubAttack";
            KeyStates[(int)EKeyList.KL_SkillAttack].AxisName = "SkillAttack";
            KeyStates[(int)EKeyList.KL_Skill0].AxisName = "skill0";
            KeyStates[(int)EKeyList.KL_Skill1].AxisName = "skill1";
            KeyStates[(int)EKeyList.KL_Skill2].AxisName = "skill2";
            KeyStates[(int)EKeyList.KL_Skill3].AxisName = "skill3";
            KeyStates[(int)EKeyList.KL_Skill4].AxisName = "skill4";
            KeyStates[(int)EKeyList.KL_Skill5].AxisName = "skill5";
            KeyStates[(int)EKeyList.KL_Skill6].AxisName = "skill6";
            KeyStates[(int)EKeyList.KL_Skill7].AxisName = "skill7";
            KeyStates[(int)EKeyList.KL_Skill8].AxisName = "skill8";
            KeyStates[(int)EKeyList.KL_Skill9].AxisName = "skill9";
        }

        // Update is called once per frame
        public void Update(float deltaTime)
        {
            // update the key status first.
            UpdateKeyStatus(deltaTime);
            UpdateMoveInput(deltaTime);

            mInputVector = new Vector2(InputBoxExtend.JoystickDelta.x, -InputBoxExtend.JoystickDelta.y);

            if (mInputVector == Vector2.zero)
            {
                if (InputBoxExtend.JoystickPressed)
                    mInputVector = mLastInputVector;
                else
                    mInputVector = new Vector2(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"));
            }

            bool moveKeyDown = (mInputVector != Vector2.zero);
            if (moveKeyDown)
                mLastInputVector = mInputVector;

            if (moveKeyDown != mMoveKeyDown)
            {
                if (moveKeyDown)
                    OnKeyDown(KeyStates[(int)EKeyList.KL_Move]);
                else
                    OnKeyUp(KeyStates[(int)EKeyList.KL_Move]);
                mMoveKeyDown = moveKeyDown;
            }

        }

        void UpdateKeyStatus(float deltaTime)
        {
            // check the key status.
            for (int idx = 0; idx < KeyStates.Length; idx++)
            {
                KeyState keyStatus = KeyStates[idx];
                keyStatus.PressedTime += deltaTime;
                keyStatus.ReleasedTime += deltaTime;

                if (string.IsNullOrEmpty(keyStatus.AxisName))
                    continue;

                // check the key state.
                bool pressed = Input.GetAxis(keyStatus.AxisName) > 0;
                if (pressed && keyStatus.Pressed == 0)
                    OnKeyDown(keyStatus);

                if (!pressed && keyStatus.Pressed != 0)
                    OnKeyUp(keyStatus);
            }
        }

        public void OnKeyDown(EKeyList key)
        {
            OnKeyDown(KeyStates[(int)key]);
        }

        public void OnKeyUp(EKeyList key)
        {
            OnKeyUp(KeyStates[(int)key]);
        }

        public void OnKeyDown(KeyState keyStatus)
        {
            keyStatus.Pressed = keyStatus.PressedTime < DoubleClickTime ? 2 : 1;
            keyStatus.PressedTime = 0.0f;
        }

        public void OnKeyUp(KeyState keyStatus)
        {
            keyStatus.Pressed = 0;
            keyStatus.ReleasedTime = 0.0f;
        }

        void UpdateMoveInput(float deltaTime)
        {
            if (mController == null ||
                mActionStatus == null ||
                mActionStatus.ActiveAction == null)
            {
                return;
            }

            // check the rotate & move.
            Vector2 direction = mInputVector;
            if (direction == Vector2.zero)
                return;

            // process moving.
            if (mActionStatus.CanMove && mActionStatus.ActiveAction.MoveSpeed > 0)
            {
                direction.Normalize();

                float moveSpeed = mOwner.Speed * 0.01f;
                Vector2 moveTrans = direction * moveSpeed * deltaTime;

                float x = -moveTrans.x, y = moveTrans.y;
                MathUtility.Rotate(ref x, ref y, mController.CameraModify);

                // rotating
                if (mActionStatus.CanRotate && mInputVector.x != 0)
                    mOwner.SetOrientation(Mathf.Atan2(x, 0));

                // moving.
                mOwner.Move(new Vector3(x, 0, y));

                if (mActionStatus.Listener != null)
                    mActionStatus.Listener.OnInputMove();
            }
            else if (mActionStatus.CanRotate) // just rotate.
            {
                float x = -mInputVector.x, y = mInputVector.y;
                MathUtility.Rotate(ref x, ref y, mController.CameraModify);

                if (mInputVector.x != 0)
                    mOwner.SetOrientation(Mathf.Atan2(x, 0));
            }
        }

        //--------------------------------------------------------------------------------------
        bool checkInputType(EKeyList inpuKey, EInputType inputType, float deltaTime)
        {
            int pressed = KeyStates[(int)inpuKey].Pressed;
            float pressedTime = KeyStates[(int)inpuKey].PressedTime;
            float releasedTime = KeyStates[(int)inpuKey].ReleasedTime;

            bool ret = false;
            switch (inputType)
            {
                case EInputType.EIT_Click:
                    ret = (pressedTime == 0);
                    break;
                case EInputType.EIT_DoubleClick:
                    ret = (pressed == 2 && pressedTime == 0);
                    break;
                case EInputType.EIT_Press:
                    ret = (pressed != 0 && (pressedTime < LongPressedTime && pressedTime + deltaTime >= LongPressedTime));
                    break;
                case EInputType.EIT_Release:
                    ret = (pressed == 0 && releasedTime == 0);
                    break;
                case EInputType.EIT_Pressing:
                    ret = (pressed != 0);
                    break;
                case EInputType.EIT_Releasing:
                    ret = (pressed == 0);
                    break;
            }
            return ret;
        }
        //--------------------------------------------------------------------------------------
        bool HasInput(EOperation operation, EInputType inputType, float deltaTime)
        {
            bool ret = false;
            switch (operation)
            {
                case EOperation.EO_Attack:
                    ret = checkInputType(EKeyList.KL_Attack, inputType, deltaTime);
                    break;
                case EOperation.EO_SpAttack:
                    ret = checkInputType(EKeyList.KL_SubAttack, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill:
                    ret = checkInputType(EKeyList.KL_SkillAttack, inputType, deltaTime);
                    break;
                case EOperation.EO_Move:
                    ret = checkInputType(EKeyList.KL_Move, inputType, deltaTime);
                    break;
                case EOperation.EO_Jump:
                    ret = checkInputType(EKeyList.KL_Jump, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill0:
                    ret = checkInputType(EKeyList.KL_Skill0, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill1:
                    ret = checkInputType(EKeyList.KL_Skill1, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill2:
                    ret = checkInputType(EKeyList.KL_Skill2, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill3:
                    ret = checkInputType(EKeyList.KL_Skill3, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill4:
                    ret = checkInputType(EKeyList.KL_Skill4, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill5:
                    ret = checkInputType(EKeyList.KL_Skill5, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill6:
                    ret = checkInputType(EKeyList.KL_Skill6, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill7:
                    ret = checkInputType(EKeyList.KL_Skill7, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill8:
                    ret = checkInputType(EKeyList.KL_Skill8, inputType, deltaTime);
                    break;
                case EOperation.EO_Skill9:
                    ret = checkInputType(EKeyList.KL_Skill9, inputType, deltaTime);
                    break;
            }
            return ret;
        }

        public bool HasInput(int operation, int inputType, float deltaTime)
        {
            return HasInput((EOperation)operation, (EInputType)inputType, deltaTime);
        }

        public void ResetInput()
        {
            foreach (KeyState keyState in KeyStates)
            {
                if (keyState.Pressed != 0)
                    OnKeyUp(keyState);
            }
            mMoveKeyDown = false;
        }
    }
}
