using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ActData.Helper;

namespace ACT
{
    public class ActionStatus
    {
        private ActData.ActionGroup mActionGroup = null;
        private ActData.Action mActiveAction = null;
        private ActData.ActionInterrupt mQueuedInterrupt = null;

        public string StartupAction = ActData.CommonAction.Idle;
        public float PushPower = 1.0f;

        bool mInitialized = false;
        bool mIgnoreMove = false;
        bool mIgnoreGravity = false;
        bool mCanMove = false;
        bool mCanRotate = false;
        bool mCanHurt = false;
        bool mFaceTarget = false;
        bool mOnStarightHit = false;
        int mActionTime = 0;
        int mActionKey = -1;
        int mEventIndex = 0;
        int mHitDefIndex = 0;
        int mActionInterruptEnabled = 0;
        ActData.HeightStatusFlag mHeightState = ActData.HeightStatusFlag.None;
        int mActionLevel = 0;
        int mLashTime = 0;
        int mStraightTime = 0;
        float mTotalTime = 0.0f;
        float mGravity = 0.0f;
        float mStraighExtent = 0.1f;
        float mActionScale = 1.0f;
        bool mRotateOnHit = false;
        Vector3 mMoveRelDistance = Vector3.zero;
        Vector3 mVelocity = Vector3.zero;
        Vector3 mBounding = Vector3.zero;
        IActUnit mOwner = null;
        IActionListener mListener = null;
        ActEffectMgr mActionEffectMgr = new ActEffectMgr();
        ISkillInput mQueuedSkillInput = null;

        GameObject mListTargetFrame;
        public static bool ShowListTarFrame = false;
        GameObject mBeatenFramObj;
        public static bool ShowBeatenFrame = false;

        public IActUnit ActionTarget { get; set; } //目标
        public ISkillItem SkillItem { get; set; } //技能
        
        public ActData.ActionGroup ActionGroup { get { return mActionGroup; } }
        public ActData.Action ActiveAction { get { return mActiveAction; } }
        public ActData.HeightStatusFlag HeightState { get { return mHeightState; } }
        public ActData.EActionState ActionState { get { return (ActData.EActionState)mActiveAction.ActionStatus; } }
        public Vector3 Bounding { get { return mBounding; } }
        public Vector3 Velocity { get { return mVelocity; } }
        public bool HasQueuedAction { get { return mQueuedInterrupt != null; } }
        public bool RotateOnHit { get { return mRotateOnHit; } }
        public bool CanMove { get { return mCanMove; } }
        public bool CanRotate { get { return mCanRotate; } }
        public bool CanHurt { get { return mOwner.CanHurt && mCanHurt; } }
        public bool FaceTarget { get { return mFaceTarget; } }
        public int ActionLevel { get { return mActionLevel; } }
        public IActionListener Listener { get { return mListener; } }
        public void Bind(IActionListener listener) { mListener = listener; }

        public ActionStatus(IActUnit owner)
        {
            mOwner = owner;
        }

        /// <summary>
        /// the main update entry.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (!mInitialized)
                mInitialized = ChangeActionGroup(mOwner.ActionGroupIdx);

            // modify the action scale.
            if (mActionScale != 1.0f)
                deltaTime *= mActionScale;

            int preTime = (int)(mTotalTime * 1000.0f);
            mTotalTime += deltaTime;
            int curTime = (int)(mTotalTime * 1000.0f);

            if (mActiveAction != null || mStraightTime > 0)
                TickAction(curTime - preTime);

            if (mListener != null)
                mListener.Update(deltaTime);

            UpdateBeatenFram();

            UpdateEffect(deltaTime);
        }

        void UpdateBeatenFram()
        {
            if (!ShowBeatenFrame)
            {
                if (mBeatenFramObj != null)
                    GameObject.Destroy(mBeatenFramObj);
                return;
            }
            float BoundOffsetX = mActiveAction.BoundingOffsetX;
            float BoundOffsetY = mActiveAction.BoundingOffsetY;
            float BoundOffsetZ = mActiveAction.BoundingOffsetZ;
            //Debug.Log("BoundOffsetX:" +BoundOffsetX+"BoundOffsetY:"+BoundOffsetY+"BoundOffsetZ"+BoundOffsetZ);
            MathUtility.Rotate(ref BoundOffsetX, ref BoundOffsetZ, mOwner.Orientation);

            Vector3 AttackeePos = mOwner.Position + new Vector3(
                BoundOffsetX, BoundOffsetY, BoundOffsetZ) * 0.01f;
            AttackeePos.y += Bounding.y / 2.0f;

            if (mBeatenFramObj != null)
                GameObject.Destroy(mBeatenFramObj);
            mBeatenFramObj = GameObject.Instantiate(Resources.Load("BeatenFrameCube")) as GameObject;
            mBeatenFramObj.transform.position = AttackeePos;
            mBeatenFramObj.transform.localScale = Bounding;
            mBeatenFramObj.transform.localEulerAngles = mOwner.UGameObject.transform.localEulerAngles;
            GameObject.Destroy(mBeatenFramObj, 1.0f);
        }

        void Reset()
        {
            mEventIndex = 0;
            mHitDefIndex = 0;
            mActionTime = 0;
            mActionKey = -1;
            mActionScale = 1.0f;
            mActionInterruptEnabled = 0;
            mQueuedInterrupt = null;
            mIgnoreMove = false;
            mIgnoreGravity = mActiveAction.IgnoreGravity;
            mCanHurt = mActiveAction.CanHurt;
            mGravity = mActiveAction != null ? mActionGroup.Gravtity * 0.01f : 9.8f;
            mMoveRelDistance = Vector3.zero;
            mRotateOnHit = mActionGroup.RotateOnHit;
            mFaceTarget = mActiveAction != null ? mActiveAction.FaceTarget : false;

            // tell the unit clear flags while action changed.
            mOwner.ClearFlags();

            if (mActiveAction != null)
            {
                float sizeModifiy = 0.01f * 0.01f;
                mBounding.x = mActionGroup.BoundingWidth * mActiveAction.BoundingWidthRadio * sizeModifiy;
                mBounding.y = mActionGroup.BoundingHeight * mActiveAction.BoundingHeightRadio * sizeModifiy;
                mBounding.z = mActionGroup.BoundingLength * mActiveAction.BoundingLengthRadio * sizeModifiy;

                mOwner.EnableCollision(mActiveAction.HasCollision != 2);

                if (mActiveAction.RotateOnHit != 0)
                    mRotateOnHit = (mActiveAction.RotateOnHit == 1);

                if (mActiveAction.ActionLevel != 0)
                    mActionLevel = mActiveAction.ActionLevel;
                else
                    mActionLevel = mActionGroup.DefaultActionLevel;

                mHeightState = (ActData.HeightStatusFlag)(1 << mActiveAction.HeightStatus);
                mCanMove = mActiveAction.CanMove;
                mCanRotate = mActiveAction.CanRotate;

                // copy the action request enabled/disabled flags.
                for (int i = 0; i < mActiveAction.ActionInterrupts.Count; i++)
                {
                    ActData.ActionInterrupt actionInterrupt = mActiveAction.ActionInterrupts[i];
                    if (actionInterrupt.Enabled)
                        mActionInterruptEnabled |= (1 << i);
                }

                if (mActiveAction.Id[0] == 'N' && SkillItem != null)
                    SkillItem = null;
            }

            mActionEffectMgr.Clear();

            if (mListTargetFrame)
            {
                GameObject.Destroy(mListTargetFrame);
                mListTargetFrame = null;
            }
        }

        public void Release()
        {
            mActionEffectMgr.Clear();
        }

        public bool ChangeActionGroup(int groupIndex)
        {
            if (ActionManager.Instance == null)
            {
                Debug.LogError("ChangeActionGroup failed");
                return false;
            }

            ActData.UnitActionInfo unitInfo = ActionManager.Instance.GetUnitActionInfo(mOwner.ActionID);
            foreach (ActData.UnitActionInfo.Types.UnitVarible v in unitInfo.UnitVaribleList)
                mOwner.GetVariable(v.Index).Set(v.Value, v.Max);

            mActionGroup = unitInfo.ActionGroups[groupIndex];
            ChangeAction(mActionGroup.StartupAction, 0);

            mInitialized = true;
            return true;
        }

        /// <summary>
        /// tick this action.
        /// </summary>
        /// <param name="deltaTime"></param>
        void TickAction(int deltaTime)
        {
            if (mActiveAction == null)
                return;

            //mActionChanged = false;
            // Œì²âŽŠÓÚÓ²Ö±×ŽÌ¬¡£
            if (ProcessStraighting(ref deltaTime))
                return;

            if (ProcessQueuedAction(deltaTime))
                return;

            if (ProcessExtraChangeAction(deltaTime))
                return;

            // check we are going to finished, tick current action to the end.
            int nextActionTime = 0;
            bool thisActionIsFinished = false;
            if ((mActionTime + deltaTime) > mActiveAction.TotalTime)
            {
                // get the new action tick time.
                nextActionTime = deltaTime;

                deltaTime = mActiveAction.TotalTime - mActionTime;
                nextActionTime -= deltaTime;

                thisActionIsFinished = true;
            }

            // next action key.
            int nextActionKey = GetNextKey(deltaTime);

            // tick the action need check the keys.
            if (nextActionKey > mActionKey)
            {
                // check the Events
                ProcessEventList(nextActionKey, deltaTime);

                // check the HitDefines
                ProcessHitDefineList(nextActionKey);

                // check the interrupt list.
                if (ProcessActionInterruptList(mActionKey, nextActionKey))
                    return;

                if (mActiveAction.PoseTime > 0 && mActionKey < 100 && nextActionKey >= 100)
                    mOwner.OnEnterPoseTime();

                // hack the event interrupts.
                mOwner.OnReachHighest(false);
            }

            // do relative & absolute moving.
            ProcessMoving(deltaTime);

            // tick time to this action.
            mActionTime += deltaTime;
            mActionKey = nextActionKey;

            // ³å»÷ŽŠÀí¡£
            ProcessLash(deltaTime);

            // this action is done!!
            if (thisActionIsFinished)
                ProcessTickFinish(nextActionTime);

        }

        bool ProcessQueuedAction(int deltaTime)
        {
            if (mQueuedInterrupt == null)
                return false;

            int actualQueuedTime = 0;
            if (!CheckTime(deltaTime, mQueuedInterrupt.ConnectTime, ref actualQueuedTime))
                return false;

            // get the new action tick time.
            int nextActionTime = mActionTime + deltaTime - actualQueuedTime;

            // change to the queued actions.
            if (mQueuedSkillInput != null)
            {
                mQueuedSkillInput.PlaySkill();
                mQueuedSkillInput = null;
            }
            else
                ChangeAction(mQueuedInterrupt.ActionCache, nextActionTime);

            // trun off queued actions.
            mQueuedInterrupt = null;

            return true;
        }

        bool ProcessExtraChangeAction(int deltaTime)
        {
            if (mActiveAction.AirStatus == (int)ActData.EAirStatus.Normal)
            {
                return false;
            }

            if (!mOwner.OnGround)
            {
                return false;
            }

            if (string.IsNullOrEmpty(mActionGroup.Diaup2FloorAction))
            {
                Logger.LogError($"未配置击飞落地动作 {mOwner.ActionID}");
                return false;
            }

            ChangeAction(mActionGroup.Diaup2FloorAction, 0);
            return true;
        }

        bool CheckTime(int deltaTime, int checkRatio, ref int checkTime)
        {
            if (mActiveAction == null)
                return false;

            // check the queued time.
            checkTime = (checkRatio <= 100) ?
                mActiveAction.AnimTime * checkRatio / 100 : // [0-100] AnimTime
                mActiveAction.AnimTime + mActiveAction.PoseTime * (checkRatio - 100) / 100; // [100-200] PoseTime

            // match the trigger time options.
            return (mActionTime == 0 && checkTime == 0) || (mActionTime < checkTime && mActionTime + deltaTime >= checkTime);
        }

        //---------------------------------------------------------------------
        public int GetCheckTime(int checkRatio)
        {
            if (mActiveAction == null) return -1;

            // check the queued time.
            return (checkRatio <= 100) ?
                mActiveAction.AnimTime * checkRatio / 100 : // [0-100] AnimTime
                mActiveAction.AnimTime + mActiveAction.PoseTime * (checkRatio - 100) / 100; // [100-200] PoseTime
        }

        bool ProcessMoving(int deltaTime)
        {
            // do relative moving.
            if (mIgnoreMove)
            {
                mIgnoreMove = false;
                return true;
            }

            float dt = deltaTime * 0.001f;
            ProcessActionMove(mVelocity, dt);

            float x = mMoveRelDistance.x, z = mMoveRelDistance.z;
            if (x != 0 || z != 0)
                MathUtility.Rotate(ref x, ref z, mOwner.Orientation);

            mOwner.Move(new Vector3(x, mMoveRelDistance.y, z));

            if (!mIgnoreGravity)
            {
                float velocityModify = -mGravity * dt;
                if (mVelocity.y > 0 && mVelocity.y <= -velocityModify)
                    mOwner.OnReachHighest(true);
                mVelocity.y += velocityModify;
            }

            mMoveRelDistance = Vector3.zero;
            return true;
        }

        void ProcessActionMove(Vector3 velocity, float dt)
        {
            mMoveRelDistance.x += velocity.x * dt;
            mMoveRelDistance.z += velocity.z * dt;

            // we need handle gravity effects here.
            if (mIgnoreGravity)
                mMoveRelDistance.y += velocity.y * dt;
            else
                mMoveRelDistance.y += velocity.y * dt - mGravity * dt * dt * 0.5f;
        }

        void ProcessTickFinish(int nextActionTime)
        {
            int nextAction = mActiveAction.NextActionCache;
            if (mOwner.Dead && mActionGroup.CheckDeath)
            {
                if (mHeightState == ActData.HeightStatusFlag.Stand)
                {
                    nextAction = mActionGroup.GetActionIdx(mActionGroup.StandDeath);
                }
                else if (mHeightState == ActData.HeightStatusFlag.Ground)
                {
                    nextAction = mActionGroup.GetActionIdx(mActionGroup.DownDeath);
                }

                if (nextAction == mActiveAction.ActionCache)
                    nextAction = mActiveAction.NextActionCache;
            }

            ChangeAction(nextAction, nextActionTime);
        }

        public void ChangeAction(string id, int deltaTime)
        {
            if (mActionGroup == null)
                return;

            int idx = mActionGroup.GetActionIdx(id);
            if (idx < 0)
            {
                Debug.LogError("Fail change action to: " + id);
                return;
            }

            ChangeAction(idx, deltaTime);
        }

        public void ChangeAction(int actionIdx, int deltaTime)
        {
            ActData.Action oldAction = mActiveAction;
            ActData.Action action = mActionGroup.GetAction(actionIdx);

            // velocity.
            if (action.ResetVelocity)
                mVelocity = Vector3.zero;

            mActiveAction = action;

            Reset();

            // tick action now.
            if (deltaTime > 0)
                TickAction(deltaTime);

            if (mListener != null)
                mListener.OnActionChanging(oldAction, action);

            // change the moving speed.
            //if (mOwner.UnitType == EUnitType.EUT_LocalPlayer || mOwner.UnitType == EUnitType.EUT_OtherPlayer)
            //{
            //    if (action.Id == Data1.CommonAction.Run)
            //    {
            //        Player player = (Player)mOwner;
            //        int baseSpeed = (action.MoveSpeed > 0) ? action.MoveSpeed : 300;
            //        mActionScale = (float)player.GetAttrib(EPA.MoveSpeed) / baseSpeed;
            //    }
            //}

            // check is should we skip animation for optimazation...
            mOwner.PlayAnimation(mActiveAction, mActionScale);
        }

        //---------------------------------------------------------------------
        int GetNextKey(int deltaTime)
        {
            if (mActiveAction == null) return -1;

            int currentTime = mActionTime + deltaTime;

            // [0-100]
            if (currentTime <= mActiveAction.AnimTime)
                return currentTime * 100 / mActiveAction.AnimTime;

            // [200-...]
            if (currentTime >= mActiveAction.TotalTime)
                return 200;

            // [101-199]
            int leftTime = currentTime - mActiveAction.AnimTime;
            return 100 + leftTime * 100 / mActiveAction.PoseTime;
        }

        //---------------------------------------------------------------------
        bool ProcessEventList(int nextKey, int deltaTime)
        {
            if (mActiveAction.Events.Count == 0 || mEventIndex >= mActiveAction.Events.Count)
                return false;

            bool ret = false;
            while (mEventIndex < mActiveAction.Events.Count)
            {
                ActData.Event actionEvent = mActiveAction.Events[mEventIndex];
                if (actionEvent.TriggerTime > nextKey)
                    break;

                // trigger this event.
                if (OnTriggerEvent(actionEvent, deltaTime))
                {
                    TriggerEvent(actionEvent, deltaTime);
                    ret = true;
                }
                mEventIndex++;
            }
            return ret;
        }

        bool OnTriggerEvent(ActData.Event actionEvent, int deltaTime)
        {
            return true;
        }

        void SwitchStatus(string name, bool on)
        {
            switch (name)
            {
                case "IgnoreGravity":
                    mIgnoreGravity = on;
                    break;
                case "CanMove":
                    mCanMove = on;
                    break;
                case "CanRotate":
                    mCanRotate = on;
                    break;
                case "CanHurt":
                    mCanHurt = on;
                    break;
                case "FaceTarget":
                    mFaceTarget = on;
                    break;
            }
        }

        void SetVelocity(ActData.Event actionEvent, float x, float y, float z, int deltaTime)
        {
            int triggerTime = GetCheckTime(actionEvent.TriggerTime);

            ProcessMoving(triggerTime - mActionTime);

            mVelocity.x = x;
            mVelocity.y = y;
            mVelocity.z = z;

            ProcessMoving(deltaTime + mActionTime - triggerTime);

            // ignore move because we already moved.
            mIgnoreMove = true;
        }

        void SetDirection(ActData.Event actionEvent, int angle, bool local)
        {
            float rad = Mathf.Deg2Rad * angle;
            mOwner.SetOrientation(local ? mOwner.Orientation + rad : rad);
        }

        void PlaySound(ActData.Event actionEvent, int soundIndex, bool checkMaterial)
        {
            //float volume = checkMaterial ? 0.5f : 1.0f;
            //SoundManager.Instance.Play3DSound(soundIndex, mOwner.Position, volume);
        }

        void PlayEffect(ActData.EventPlayEffect data)
        {
            
            //[Description("0=所有玩家阵营可见（默认） 1=自己与友方阵营可见 2=敌方阵营可见 3=仅自己可见")]//
            switch (data.VisibleType)
            {
                case 1:
                    if (mOwner.Camp != EUnitCamp.EUC_FRIEND)
                        return;
                    break;
                case 2:
                    if (mOwner.Camp != EUnitCamp.EUC_ENEMY)
                        return;
                    break;
                case 3:
                    //if (mOwner.UnitType != EUnitType.EUT_LocalPlayer)
                    //    return;
                    break;
            }

            Vector3 tmpPos = mOwner.UGameObject.transform.position;
            Quaternion tmpRotation = Quaternion.identity;
            Vector3 tmpOffset = new Vector3(data.OffsetX * 0.01f, data.OffsetY * 0.01f, data.OffsetZ * 0.01f);
            Transform tmpParent = null;

            if (data.BindMode == 0)
            {
                tmpParent = mOwner.Transform;
                tmpPos = tmpOffset;
            }
            else
            {
                tmpRotation = mOwner.UGameObject.transform.rotation;
                tmpPos += tmpRotation * tmpOffset;
            }

            // stop while action changed.
            if (data.StopMode == 1)
            {
                mActionEffectMgr.PlayEffect(data.EffectName, data.Duration * 0.001f, tmpParent, tmpPos, tmpRotation);
            }
            else
            {
                ActionSystem.Instance.EffectMgr.PlayEffect(data.EffectName, data.Duration * 0.001f, tmpParent, tmpPos, tmpRotation);
            }
        }

        void UpdateEffect(float deltaTime)
        {
            mActionEffectMgr.Update(deltaTime);
        }

        const float FTOffset = 0.2f;
        void ListTargets(ActData.EventListTargets data)
        {
            ActionTarget = null;

            Vector3 forward = mOwner.UGameObject.transform.forward;
            Vector3 right = mOwner.UGameObject.transform.right;
            Vector3 basePos = mOwner.Position + forward * FTOffset;
            float fanDistance = data.FanRadius;
            float fanSqrSqrDistance = fanDistance * fanDistance;
            float fanAngle = data.FanAngle;
            float fanRadin = Mathf.Cos(fanAngle * 0.5f);
            float minCheck = 0;

            if (ShowListTarFrame)
            {
                switch (data.ListType)
                {
                    case ActData.ListTargetFrameType.CuboidListType:
                        if (mListTargetFrame != null)
                            GameObject.Destroy(mListTargetFrame);
                        GameObject cubeListTarObj = GameObject.Instantiate(Resources.Load("ListTargetCube")) as GameObject;
                        UpdateMaterialCol(cubeListTarObj, data.ListMode);
                        cubeListTarObj.transform.parent = mOwner.UGameObject.transform;
                        //cube型的listtarget位置
                        Vector3 cubeListTarPos = Vector3.zero;
                        cubeListTarPos.x = (data.Right + data.Left) * 0.01f / 2.0f; //x的坐标
                        cubeListTarPos.y = (data.Top + data.Bottom) * 0.01f / 2.0f; //y的坐标
                        cubeListTarPos.z = (data.Front + data.Back) * 0.01f / 2.0f; //z的坐标
                        cubeListTarObj.transform.localPosition = cubeListTarPos;
                        //listTarget的角度
                        cubeListTarObj.transform.localEulerAngles = Vector3.zero;
                        //重置父节点
                        cubeListTarObj.transform.parent = mOwner.UGameObject.transform.parent;
                        //listTarget的大小
                        Vector3 cubeListTarScale = Vector3.one;
                        cubeListTarScale.x = (data.Right - data.Left) * 0.01f; //x方向上的大小
                        cubeListTarScale.y = (data.Top - data.Bottom) * 0.01f; //y方向上的大小
                        cubeListTarScale.z = (data.Front - data.Back) * 0.01f; //z方向上的大小
                        cubeListTarObj.transform.localScale = cubeListTarScale;
                        mListTargetFrame = cubeListTarObj;
                        break;

                    case ActData.ListTargetFrameType.FanListType:
                        if (mListTargetFrame != null)
                            GameObject.Destroy(mListTargetFrame);
                        GameObject fanListTarObj = GameObject.Instantiate(Resources.Load("ListTargetCylinder")) as GameObject;
                        UpdateMaterialCol(fanListTarObj, data.ListMode);
                        fanListTarObj.transform.parent = mOwner.UGameObject.transform;
                        //fan型的listTarget的位置
                        Vector3 fanListTarpos = Vector3.zero;
                        fanListTarpos.y = (data.Top + data.Bottom) * 0.01f / 4.0f; //y轴坐标
                        fanListTarObj.transform.localPosition = fanListTarpos;
                        //fan型的listTarget的角度
                        fanListTarObj.transform.localEulerAngles = Vector3.zero;
                        //fan型的listTarget的大小
                        Vector3 fanListTarScale = Vector3.one;
                        fanListTarScale.x = fanListTarScale.z = data.FanRadius * 0.01f * 2.0f;
                        fanListTarScale.y = (data.Top - data.Bottom) * 0.01f / 2.0f; //y方向上的大小
                        fanListTarObj.transform.localScale = fanListTarScale;
                        //画出表示角度的两条线
                        DrawAngleLine(fanListTarObj, data.FanAngle, data.FanRadius);
                        //重置父节点
                        fanListTarObj.transform.parent = mOwner.UGameObject.transform.parent;
                        mListTargetFrame = fanListTarObj;
                        break;
                    default:
                        break;
                }
                if (mListTargetFrame != null)
                    GameObject.Destroy(mListTargetFrame, 2.0f);
            }

            for (int i = 0, max = ActionSystem.Instance.ActUnitMgr.Units.Count; i < max; ++i)
            {
                var tmpTarget = ActionSystem.Instance.ActUnitMgr.Units[i];

                if (tmpTarget == mOwner ||
                    tmpTarget.Dead ||
                    tmpTarget.Camp == EUnitCamp.EUC_FRIEND ||
                    tmpTarget.Camp == mOwner.Camp)
                    return;

                Vector3 trans = tmpTarget.Position - basePos;
                trans.y = 0;

                float sqrMagnitude = trans.sqrMagnitude;
                if (data.ListType == ActData.ListTargetFrameType.FanListType)
                {
                    if (sqrMagnitude > fanSqrSqrDistance ||
                        Vector3.Dot(trans, forward) < fanRadin)
                        return;
                }
                else
                {
                    float length = Vector3.Dot(trans, forward);
                    if (length < data.Back * 0.01f || length > data.Front * 0.01f)
                        return;

                    Vector3 side = trans - length * forward;
                    float width = Vector3.Dot(side, right);
                    if (width < data.Left * 0.01f || width > data.Right * 0.01f)
                        return;
                }

                float thisCheck = sqrMagnitude;
                if (data.ListMode == ActData.ListTargetMode.MinAngle)
                    thisCheck = Mathf.Abs(Mathf.Atan2(trans.x, trans.z) - mOwner.Orientation);
                else if (data.ListMode == ActData.ListTargetMode.Random)
                    thisCheck = Random.Range(1, 100);

                if (ActionTarget == null || thisCheck < minCheck)
                {
                    ActionTarget = tmpTarget;
                    minCheck = thisCheck;
                }
            }
        }

        void DrawAngleLine(GameObject go, int angle, int radius)
        {
            Vector3 topCenterPos = go.transform.localPosition;
            topCenterPos.y += go.transform.localScale.y;
            LineRenderer line = go.AddComponent<LineRenderer>();
            line.startWidth = 0.02f;
            line.endWidth = 0.02f;
            line.positionCount = 3;
            Vector3 leftPos = topCenterPos;
            float angleLeftOffset = go.transform.parent.localEulerAngles.y + angle / 2.0f;
            float angelRightOffset = go.transform.parent.localEulerAngles.y - angle / 2.0f;

            leftPos.x += Mathf.Sin(Mathf.Deg2Rad * angleLeftOffset) * radius * 0.01f;
            leftPos.z += Mathf.Cos(Mathf.Deg2Rad * angleLeftOffset) * radius * 0.01f;
            Vector3 rightPos = topCenterPos;
            rightPos.x += Mathf.Sin(Mathf.Deg2Rad * angelRightOffset) * radius * 0.01f;
            rightPos.z += Mathf.Cos(Mathf.Deg2Rad * angelRightOffset) * radius * 0.01f;

            Vector3 parentPos = go.transform.parent.localPosition;
            line.SetPosition(0, leftPos + parentPos);
            line.SetPosition(1, topCenterPos + parentPos);
            line.SetPosition(2, rightPos + parentPos);
        }

        void UpdateMaterialCol(GameObject go, ActData.ListTargetMode mode)
        {
            switch (mode)
            {
                case ActData.ListTargetMode.MinAngle:
                    go.transform.GetComponent<Renderer>().material.SetColor("_TintColor", Color.yellow);
                    Debug.Log("The Color of MinAngle is red");
                    break;
                case ActData.ListTargetMode.MinDistance:
                    go.transform.GetComponent<Renderer>().material.SetColor("_TintColor", Color.blue);
                    Debug.Log("The Color of MinDistance is blue");
                    break;
                case ActData.ListTargetMode.Random:
                    go.transform.GetComponent<Renderer>().material.SetColor("_TintColor", Color.green);
                    Debug.Log("The Color of Random is green");
                    break;
                default:
                    break;
            }
        }
        void FaceTargets(ActData.EventFaceTargets data)
        {
            if (ActionTarget == null || !ActionTarget.UGameObject || ActionTarget.Dead)
                return;

            float x = ActionTarget.Position.x - mOwner.Position.x;
            float z = ActionTarget.Position.z - mOwner.Position.z;
            float dir = Mathf.Atan2(x, z);
            mOwner.SetOrientation(dir);

            if (mListener != null)
                mListener.OnFaceTarget();
        }

        void GoToTargets(ActData.EventGoToTargets data)
        {
            if (ActionTarget == null || !ActionTarget.UGameObject || ActionTarget.Dead)
                return;

            int offsetX = data.Random ? Random.Range(0, data.OffsetX) : data.OffsetX;
            int offsetY = data.Random ? Random.Range(0, data.OffsetY) : data.OffsetY;
            int offsetZ = data.Random ? Random.Range(0, data.OffsetZ) : data.OffsetZ;
            Vector3 offset = new Vector3(offsetX * 0.01f, offsetY * 0.01f, offsetZ * 0.01f);
            if (offset != Vector3.zero)
            {
                if (data.Local)
                    offset = ActionTarget.UGameObject.transform.rotation * offset;
                offset += ActionTarget.Radius * offset.normalized;
            }
            else
            {
                offset = ActionTarget.Radius * (mOwner.Position - ActionTarget.Position).normalized;
            }

            Vector3 targetPos = ActionTarget.Position + offset;
            mOwner.Move(targetPos - mOwner.Position);
        }

        bool SenseTarget(ActData.ActionInterrupt interrupt)
        {
            if (ActionTarget == null || !ActionTarget.UGameObject || ActionTarget.Dead)
                return false;

            float distance = MathUtility.DistanceSqr(mOwner.Position, ActionTarget.Position) * 10000.0f;
            return distance <= interrupt.TargetDistanceMax * interrupt.TargetDistanceMax &&
                distance >= interrupt.TargetDistanceMin * interrupt.TargetDistanceMin;
        }

        void AddUnit(ActData.EventAddUnit data)
        {
            /*
            UnitBase unitBase = UnitBaseManager.Instance.GetItem(data.Id);
            UnityEngine.Object prefab = unitBase != null ? Resources.Load(unitBase.Prefab) : null;
            if (prefab == null)
            {
                Debug.Log(string.Format("Fail to find prefab, Unit={0} Action={1} ID={2} Prefab={3}",
                    mOwner.UnitID,
                    mActiveAction.ID,
                    data.Id,
                    unitBase != null ? unitBase.Prefab : "NotFound"));
                return;
            }

            Vector3 offset = new Vector3(data.PosX, data.PosY, data.PosZ);
            Vector3 pos = mOwner.Position + mOwner.UUnitInfo.transform.rotation * (offset * 0.01f);

            GameObject obj = GameObject.Instantiate(prefab) as GameObject;
            UnitInfo unitInfo = obj.GetComponent<UnitInfo>();
            unitInfo.Unit.SetPosition(pos);
            if (data.Local)
                unitInfo.Unit.SetOrientation(mOwner.Orientation + data.Angle * Mathf.Deg2Rad);
            else
                unitInfo.Unit.SetOrientation(data.Angle * Mathf.Deg2Rad);
            unitInfo.Unit.PlayAction(data.ActionId);
            unitInfo.Camp = mOwner.Camp;
            unitInfo.Unit.Owner = mOwner;
            unitInfo.Unit.ActionStatus.SkillItem = SkillItem;*/
        }

        bool TriggerEvent(ActData.Event actionEvent, int deltaTime)
        {
            switch (actionEvent.EventType)
            {
                case ActData.EventType.StatusOn:
                    {
                        ActData.EventStatusOn data = actionEvent.EventDetailData.EventStatusOn;
                        SwitchStatus(data.StatusName, true);
                    }
                    break;
                case ActData.EventType.StatusOff:
                    {
                        ActData.EventStatusOff data = actionEvent.EventDetailData.EventStatusOff;
                        SwitchStatus(data.StatusName, false);
                    }
                    break;
                case ActData.EventType.SetVelocity:
                    {
                        ActData.EventSetVelocity data = actionEvent.EventDetailData.EventSetVelocity;
                        SetVelocity(actionEvent, data.VelocityX * -0.01f, data.VelocityY * 0.01f, data.VelocityZ * 0.01f, deltaTime);
                    }
                    break;
                case ActData.EventType.SetVelocityX:
                    {
                        ActData.EventSetVelocity_X data = actionEvent.EventDetailData.EventSetVelocityX;
                        SetVelocity(actionEvent, data.VelocityX * -0.01f, mVelocity.y, mVelocity.z, deltaTime);
                    }
                    break;
                case ActData.EventType.SetVelocityY:
                    {
                        ActData.EventSetVelocity_Y data = actionEvent.EventDetailData.EventSetVelocityY;
                        SetVelocity(actionEvent, mVelocity.x, data.VelocityY * 0.01f, mVelocity.z, deltaTime);
                    }
                    break;
                case ActData.EventType.SetVelocityZ:
                    {
                        ActData.EventSetVelocity_Z data = actionEvent.EventDetailData.EventSetVelocityZ;
                        SetVelocity(actionEvent, mVelocity.x, mVelocity.y, data.VelocityZ * 0.01f, deltaTime);
                    }
                    break;
                case ActData.EventType.SetDirection:
                    {
                        ActData.EventSetDirection data = actionEvent.EventDetailData.EventSetDirection;
                        SetDirection(actionEvent, data.Angle, data.Local);
                    }
                    break;
                case ActData.EventType.PlaySound:
                    {
                        ActData.EventPlaySound data = actionEvent.EventDetailData.EventPlaySound;
                        if (data.SoundIndex == -2)
                        {
                            //               data.SoundIndex = SoundManager.Instance.GetSoundIndex(data.SoundName);
                            //if (data.SoundIndex < 0)
                            //	Debug.LogError(string.Format("Fail to playsound: [{0}][{1}][{2}]", mOwner.UnitID, mActiveAction.ID, data.SoundName));
                        }

                        if (data.SoundIndex >= 0)
                            PlaySound(actionEvent, data.SoundIndex, data.CheckMatril);
                    }
                    break;
                case ActData.EventType.SetGravity:
                    {
                        ActData.EventSetGravity data = actionEvent.EventDetailData.EventSetGravity;
                        mGravity = data.Gravity * 0.01f;
                    }
                    break;
                case ActData.EventType.RemoveMyself:
                    //Debug.Log("Unit " + mOwner.UnitID + " RemoveMyself");
                    mOwner.Destory();
                    break;
                case ActData.EventType.AdjustVarible:
                    {
                        ActData.EventAdjustVarible data = actionEvent.EventDetailData.EventAdjustVarible;
                        CustomVariable variable = mOwner.GetVariable(data.Slot);
                        variable.Adjust(data.Value);
                    }
                    break;
                case ActData.EventType.SetVariable:
                    {
                        ActData.EventSetVariable data = actionEvent.EventDetailData.EventSetVariable;
                        CustomVariable variable = mOwner.GetVariable(data.Slot);
                        variable.Set(data.Value, data.MaxValue);
                    }
                    break;
                case ActData.EventType.ListTargets:
                    ListTargets(actionEvent.EventDetailData.EventListTargets);
                    break;
                case ActData.EventType.ClearTargets:
                    ActionTarget = null;
                    break;
                case ActData.EventType.FaceTargets:
                    FaceTargets(actionEvent.EventDetailData.EventFaceTargets);
                    break;
                case ActData.EventType.GoToTargets:
                    GoToTargets(actionEvent.EventDetailData.EventGoToTargets);
                    break;
                case ActData.EventType.PlayEffect:
                    PlayEffect(actionEvent.EventDetailData.EventPlayEffect);
                    break;
                case ActData.EventType.HasCollision:
                    {
                        ActData.EventHasCollision data = actionEvent.EventDetailData.EventHasCollision;
                        mOwner.EnableCollision(data.HasCollision);
                    }
                    break;
                case ActData.EventType.ExeScript:
                    {
                        ActData.EventExeScript data = actionEvent.EventDetailData.EventExeScript;
                        string[] strs = data.ScriptCmd.Split('(');
                        string scriptname = strs[0];
                        string parameter = strs[1];
                        mOwner.UGameObject.SendMessage(scriptname, parameter.Remove(parameter.Length - 1));
                        //Debug.Log(scriptname);
                    }
                    break;
                case ActData.EventType.ActionLevel:
                    {
                        ActData.EventActionLevel data = actionEvent.EventDetailData.EventActionLevel;
                        mActionLevel = data.Level;
                    }
                    break;
                case ActData.EventType.AddUnit:
                    AddUnit(actionEvent.EventDetailData.EventAddUnit);
                    break;
            }

            return true;
        }

        bool ProcessHitDefineList(int nextKey)
        {
            if (mActiveAction.AttackDefs.Count == 0 || mHitDefIndex >= mActiveAction.AttackDefs.Count)
                return false;

            bool ret = false;
            while (mHitDefIndex < mActiveAction.AttackDefs.Count)
            {
                ActData.AttackDef hit_data = mActiveAction.AttackDefs[mHitDefIndex];
                if (hit_data.TriggerTime > nextKey)
                    break;

                if (hit_data.EventOnly == 0)
                {
                    CreateHitDefine(hit_data, Vector3.zero, mActiveAction.Id);
                    ret = true;
                }
                mHitDefIndex++;
            }
            return ret;
        }

        bool CreateHitDefine(ActData.AttackDef hit_data, Vector3 position, string action)
        {
            ActionSystem.Instance.HitDefMgr.CreateHitDefinition(hit_data, mOwner, action, SkillItem);
            return true;
        }

        //---------------------------------------------------------------------
        bool ProcessActionInterruptList(int preKey, int nextKey)
        {
            if (mQueuedInterrupt != null)
                return false;

            // check the action interrupts
            if (mActiveAction.ActionInterrupts.Count == 0)
                return false;

            int interruptIdx = 0;
            foreach (ActData.ActionInterrupt interrupt in mActiveAction.ActionInterrupts)
            {
                if (interrupt.EnableBegin != 0 && interrupt.EnableBegin > preKey && interrupt.EnableBegin <= nextKey)
                    EnableActionRequest(interruptIdx, true);

                if (interrupt.EnableEnd != 200 && interrupt.EnableEnd > preKey && interrupt.EnableEnd <= nextKey)
                    EnableActionRequest(interruptIdx, false);

                if (GetInterruptEnabled(interruptIdx++) && interrupt.ConditionInterrupte)
                {
                    if (ProcessActionInterrupt(interrupt))
                        return true;
                }
            }
            return false;
        }

        //---------------------------------------------------------------------
        void EnableActionRequest(int statusIdx, bool enabled)
        {
            if (enabled)
                mActionInterruptEnabled |= (1 << statusIdx);
            else
                mActionInterruptEnabled &= ~(1 << statusIdx);
        }

        public bool GetInterruptEnabled(int idx)
        {
            return (mActionInterruptEnabled & (1 << idx)) != 0;
        }

        bool ProcessActionInterrupt(ActData.ActionInterrupt interrupt)
        {
            if (!interrupt.ConditionInterrupte)
                return false;

            // the [interrupt.ConditionInterrupte] need user input.
            // do not process it here.
            if (interrupt.CheckAllCondition && interrupt.CheckInput1)
                return false;

            if (!CheckActionInterrupt(interrupt))
                return false;

            return LinkAction(interrupt, null);
        }

        bool DetectVariable(ActData.ActionInterrupt interrupt)
        {
            switch (interrupt.Variable)
            {
                case (int)EVariableIdx.EVI_HP:
                    return CustomVariable.Compare((ECompareType)interrupt.CompareType, mOwner.CurHp, interrupt.CompareValue);
                case (int)EVariableIdx.EVI_HPPercent:
                    return CustomVariable.Compare((ECompareType)interrupt.CompareType,
                        mOwner.CurHp * 100 / mOwner.HpMax,
                        interrupt.CompareValue);
                case (int)EVariableIdx.EVI_Level:
                    return CustomVariable.Compare((ECompareType)interrupt.CompareType, mOwner.Level, interrupt.CompareValue);
                default:
                    {
                        int varIndex = interrupt.Variable - (int)EVariableIdx.EVI_Custom;
                        CustomVariable variable = mOwner.GetVariable(varIndex);
                        return variable.Compare((ECompareType)interrupt.CompareType, interrupt.CompareValue);
                    }
            }
        }

        //---------------------------------------------------------------------
        public bool CheckActionInterrupt(ActData.ActionInterrupt interrupt)
        {
            bool ret = false;
            if (interrupt.CheckAllCondition)
            {
                ret = true;
                ret = ret && (!interrupt.TouchGround || mOwner.OnGround);
                ret = ret && (!interrupt.TouchWall || mOwner.OnTouchWall);
                ret = ret && (!interrupt.ReachHighest || mOwner.OnHighest);
                ret = ret && (!interrupt.UnitDead || mOwner.Dead);
                ret = ret && (!interrupt.HitTarget || (mOwner.HitTarget != null));
                ret = ret && (!interrupt.DetectVariable || DetectVariable(interrupt));
                ret = ret && (!interrupt.SenseTarget || SenseTarget(interrupt));
            }
            else
            {
                ret = false;
                ret = ret || (interrupt.TouchGround && mOwner.OnGround);
                ret = ret || (interrupt.TouchWall && mOwner.OnTouchWall);
                ret = ret || (interrupt.ReachHighest && mOwner.OnHighest);
                ret = ret || (interrupt.UnitDead && mOwner.Dead);
                ret = ret || (interrupt.HitTarget && (mOwner.HitTarget != null));
                ret = ret || (interrupt.DetectVariable && DetectVariable(interrupt));
                ret = ret || (interrupt.SenseTarget && SenseTarget(interrupt));
            }

            return ret;
        }
        //---------------------------------------------------------------------
        public bool LinkAction(ActData.ActionInterrupt interrupt, ISkillInput skillInput)
        {
            if (interrupt.ConnectMode >= 2)
                return false;

            bool connectImmediately = (interrupt.ConnectMode == 0);
            if (!connectImmediately && mActiveAction != null)
            {
                // check the queued time.
                int actualQueuedTime = (interrupt.ConnectTime <= 100) ?
                    mActiveAction.AnimTime * interrupt.ConnectTime / 100 :  // [0-100] AnimTime
                    mActiveAction.AnimTime + mActiveAction.PoseTime * (interrupt.ConnectTime - 100) / 100; // [100-200] PoseTime

                // if the time already passed, do it immediately.
                if (actualQueuedTime <= mActionTime)
                    connectImmediately = true;
            }

            // do it immediately if the request is this.
            if (!connectImmediately)
            {
                mQueuedInterrupt = interrupt;
                mQueuedSkillInput = skillInput;
            }
            else
            {
                mQueuedInterrupt = null;
                mQueuedSkillInput = null;
                if (skillInput != null)
                    skillInput.PlaySkill();
                else
                    ChangeAction(interrupt.ActionCache, 0);
            }

            return true;
        }

        public bool OnHit(ActData.HitResultType HitResult, bool remoteAttacks)
        {
            // copy the action request enabled/disabled flags.
            for (int interruptIdx = 0; interruptIdx < mActiveAction.ActionInterrupts.Count; interruptIdx++)
            {
                ActData.ActionInterrupt interrupt = mActiveAction.ActionInterrupts[interruptIdx];
                if (GetInterruptEnabled(interruptIdx) &&
                    interrupt.Hurted &&
                    (interrupt.HurtType & (1 << (int)HitResult)) != 0 &&
                    (!interrupt.RemoteOnly || remoteAttacks))
                {
                    LinkAction(interrupt, null);
                    Debug.Log("OnHit Link Action: " + interrupt.ActionID);
                    return true;
                }
            }

            string changeAction = "";

            bool handled = true;
            switch (HitResult)
            {
                case ActData.HitResultType.StandHit:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandStandHit;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirStandHit;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorStandHit;
                    }
                    break;
                case ActData.HitResultType.KnockOut:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandKnockOut;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirKnockOut;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorKnockOut;
                    }
                    break;
                case ActData.HitResultType.KnockBack:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandKnockBack;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirKnockBack;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorKnockBack;
                    }
                    break;
                case ActData.HitResultType.KnockDown:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandKnockDown;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirKnockDown;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorKnockDown;
                    }
                    break;
                case ActData.HitResultType.DiagUp:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandDiagUp;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirDiagUp;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorDiagUp;
                    }
                    break;
                case ActData.HitResultType.HitResultHold:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandHold;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirHold;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorHold;
                    }
                    break;
                case ActData.HitResultType.AirHit:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandAirHit;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirAirHit;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorAirHit;
                    }
                    break;
                case ActData.HitResultType.DownHit:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandDownHit;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirDownHit;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorDownHit;
                    }
                    break;
                case ActData.HitResultType.FallDown:
                    {
                        if (mHeightState == ActData.HeightStatusFlag.Stand)
                            changeAction = mActionGroup.StandFallDown;
                        else if (mHeightState == ActData.HeightStatusFlag.LowAir || mHeightState == ActData.HeightStatusFlag.HighAir)
                            changeAction = mActionGroup.AirFallDown;
                        else if (mHeightState == ActData.HeightStatusFlag.Ground)
                            changeAction = mActionGroup.FloorFallDown;
                    }
                    break;
                default:
                    handled = false;
                    break;
            }

            if (!string.IsNullOrEmpty(changeAction))
            {
                Logger.Log($"OnHit action -> {changeAction}");
                ChangeAction(changeAction, 0);
            }

            return handled;
        }


        public void SetLashVelocity(float x, float y, float z, int lashTime)
        {
            mVelocity.x = x * mActionGroup.LashModifier.X;
            mVelocity.y = y * mActionGroup.LashModifier.Y;
            mVelocity.z = z * mActionGroup.LashModifier.Z;
            mLashTime = lashTime;
        }

        bool ProcessLash(int deltaTime)
        {
            if (mLashTime <= 0)
                return false;

            mLashTime -= deltaTime;
            if (mLashTime > 0)
                return false;

            // ³å»÷Ê±ŒäÍê³É¡£
            if (mHeightState == ActData.HeightStatusFlag.Stand || mHeightState == ActData.HeightStatusFlag.Ground)
            {
                mVelocity.x = 0.0f;
                mVelocity.z = 0.0f;
            }

            return true;
        }

        public void SetStraightTime(int time, bool onHit)
        {
            mStraightTime = time;
            mOnStarightHit = onHit;
            if (mStraightTime > 0)
                mOwner.BeginStaight();
        }

        bool ProcessStraighting(ref int deltaTime)
        {
            if (mStraightTime > 0)
            {
                mStraightTime -= deltaTime;
                if (mStraightTime <= 0)
                {
                    if (mOwner.ModelTrans)
                        mOwner.ModelTrans.localPosition = Vector3.zero;

                    mOwner.EndStaight();
                    deltaTime = -mStraightTime;
                }
                else if (mOnStarightHit)
                {
                    //Vector3 straighMove = new Vector3(mStraighExtent, 0f, 0f);
                    //if (mOwner.ModelTrans)
                    //    mOwner.ModelTrans.localPosition = straighMove;
                    //mStraighExtent = -mStraighExtent;
                }

                return true;
            }

            return false;
        }
    }
}
