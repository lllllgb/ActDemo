using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{
    public abstract class ActUnit : IActUnit
    {
        public const int MAX_VARIABLE_NUM = 20;

        bool mOnGround = false;
        bool mOnTouchWall = false;
        bool mOnHighest = false;
        bool mDead = false;

        IActUnit mHitTarget = null;
        Animation mCachedAnimation;
        AnimationState mCachedAnimationState;
        ActionStatus mActionStatus;
        CharacterController mController;
        Collider mCollider;
        GameObject mGameObject;
        Transform mTransform;
        Transform mModelTrans;
        CustomVariable[] mVariables = new CustomVariable[MAX_VARIABLE_NUM];

        long mUUID;
        int mUnitID;
        int mActionID;
        int mActionGroupIdx;
        int mAIDiff;
        int mLevel;
        float mRadius = 0.5f;
        EUnitCamp mCamp = EUnitCamp.EUC_NONE;
        EUnitState mState = EUnitState.Normal;
        Vector3 mPosition = Vector2.zero;
        float mOrientation = 0.0f;
        float mAnimSpeed = 1.0f;
        bool mCollisionEnabled = true;
        int mCacheLayerMask = 0;

        Vector3 mSyncOffset = Vector3.zero;
        float mSyncRotate = 0.0f;
        float mSyncTime = 0.0f;
        
        public long UUID { get { return mUUID; } }
        public int UnitID { get { return mUnitID; } protected set { mUnitID = value; } }
        public int Level { get { return mLevel; } protected set { mLevel = value; } }
        public float Radius { get { return mRadius; } }
        public GameObject UGameObject { get { return mGameObject; } }
        public Transform Transform { get { return mTransform; } }
        public Transform ModelTrans { get { return mModelTrans; } }
        public EUnitCamp Camp { get { return mCamp; } protected set { mCamp = value; } }
        public int ActionID { get { return mActionID; } protected set { mActionID = value; } }
        public int ActionGroupIdx { get { return mActionGroupIdx; } }
        public int AIDiff { get { return mAIDiff; } protected set { mAIDiff = value; } }
        public ActionStatus ActStatus { get { return mActionStatus; } }
        public abstract int Speed { get; }
        public abstract bool CanHurt { get; }
        public abstract int CurHp { get; }
        public abstract int HpMax { get; }
        public bool OnGround { get { return mOnGround; } }
        public bool OnTouchWall { get { return mOnTouchWall; } }
        public bool OnHighest { get { return mOnHighest; } }
        public bool Dead { get { return mDead; } }
        public IActUnit HitTarget { get { return mHitTarget; } }
        public Vector3 Position { get { return mPosition; } }
        public float Orientation { get { return mOrientation; } }
        public EUnitState State { get { return mState; } }
        public CustomVariable GetVariable(int idx) { return mVariables[idx]; }
        public IActUnit Owner { get; set; }

        public float MoveZMultiple { get; set; } = 2;

        public ActUnit()
        {
        }

        protected void InitActUnit(GameObject go, Transform model)
        {
            mGameObject = go;
            mTransform = go.transform;
            mModelTrans = model;

            if (null != mModelTrans)
            {
                mCachedAnimation = mModelTrans.GetComponent<Animation>();
            }

            // controller.
            mController = go.GetComponent<CharacterController>();
            if (mController)
            {
                mController.enabled = false;
                mRadius = Mathf.Max(mRadius, mController.radius);
            }

            // setup radius.
            mCollider = go.GetComponent<Collider>();
            if (mCollider && mCollider is CapsuleCollider)
                mRadius = (mCollider as CapsuleCollider).radius;

            for (int i = 0; i < mVariables.Length; i++)
                mVariables[i] = new CustomVariable();

            // build the cache layer mask
            // for unit raycast params.
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(go.layer, i))
                    mCacheLayerMask |= (1 << i);
            }

            mDead = false;
            mOnGround = false;
            mOnTouchWall = false;
            mOnHighest = false;
            mDead = false;
            mHitTarget = null;

            mState = EUnitState.Normal;
            mPosition = mTransform.position;
            mOrientation = mTransform.eulerAngles.y * Mathf.Deg2Rad;
            mActionStatus = new ActionStatus(this);
            mActionStatus.ChangeActionGroup(mActionGroupIdx);
        }

        public virtual void Update(float deltaTime)
        {
            mActionStatus.Update(deltaTime);

            if (mSyncTime > 0)
            {
                UpdateSyncPosition(deltaTime);
            }
        }

        public virtual void Dispose()
        {
            mActionStatus.Release();
        }

        protected void SetIsDead(bool dead)
        {
            mDead = dead;
            if (State != EUnitState.Die && mDead)
                OnDead();
        }

        protected virtual void OnDead()
        {
            // disable the controller.
            mState = EUnitState.Die;

            if (ActStatus != null && ActStatus.HeightState == ActData.HeightStatusFlag.Stand)
                ActStatus.ChangeAction(ActStatus.ActionGroup.StandDeath, 0);
            else if (ActStatus != null && ActStatus.HeightState == ActData.HeightStatusFlag.Ground)
                ActStatus.ChangeAction(ActStatus.ActionGroup.DownDeath, 0);
        }

        void UpdateSyncPosition(float deltaTime)
        {
            float timePass = Mathf.Min(mSyncTime, deltaTime);
            float passLerp = timePass / mSyncTime;
            float leftLerp = 1.0f - passLerp;
            mSyncTime -= timePass;
            Vector3 offset = mSyncOffset * passLerp;

            // do linear interpret location.
            Move(offset);

            // lerp orientation.
            if (mSyncRotate != 0)
            {
                SetOrientation(mOrientation + mSyncRotate * passLerp);
                mSyncRotate *= leftLerp;
            }

            mSyncOffset *= leftLerp;
        }

        public void ClearSyncMove()
        {
            mSyncTime = 0; // clear the sync flag.
        }

        public void SetSyncMove(float x, float z, float time, bool faceDir)
        {
            // the unit should not be in hurt mode. [ActionStatus == 3]
            if (mActionStatus.ActionState == ActData.EActionState.Hit)
                return;

            // force the unit to be run action.
            if (mActionStatus.ActiveAction.Id != ActData.CommonAction.Run)
                PlayAction(ActData.CommonAction.Run);

            mSyncOffset = new Vector3(x - mPosition.x, 0, z - mPosition.z);
            mSyncTime = time;

            // apply orientation immediately.
            if (faceDir)
            {
                float targetRotate = Mathf.Atan2(mSyncOffset.x, mSyncOffset.z);
                mSyncRotate = (targetRotate - mOrientation) % (Mathf.PI * 2);
                if (mSyncRotate > Mathf.PI)
                    mSyncRotate -= Mathf.PI * 2;
                else if (mSyncRotate < -Mathf.PI)
                    mSyncRotate += Mathf.PI * 2;
            }
            else
                mSyncRotate = 0;
        }

        public void SetPosition(Vector3 pos)
        {
            mPosition = pos;
            mTransform.position = mPosition;
        }

        public void SetOrientation(float orient)
        {
            mOrientation = orient;
            Vector3 eulerAngles = mTransform.eulerAngles;
            eulerAngles.y = mOrientation * Mathf.Rad2Deg;
            mTransform.eulerAngles = eulerAngles;
        }

        public void PlayAction(string action)
        {
            if (!string.IsNullOrEmpty(action))
                mActionStatus.ChangeAction(action, 0);
        }

        public void EnableCollision(bool enable)
        {
            mCollisionEnabled = enable;

            if (mController)
                mController.enabled = enable;

            if (mCollider)
                mCollider.enabled = enable;
        }

        public void Move(Vector3 trans)
        {
            trans.z *= MoveZMultiple;

            if (null != mController)
            {
                if (trans.y != 0) mOnGround = false;
                if (mController != null && mController.enabled)
                {
                    CollisionFlags collisionFlags = mController.Move(trans);
                    if ((collisionFlags & CollisionFlags.Below) == CollisionFlags.Below)
                        mOnGround = true;

                    if ((collisionFlags & CollisionFlags.Sides) == CollisionFlags.Sides)
                        mOnTouchWall = true;
                    else if (trans.x != 0 || trans.z != 0)
                        mOnTouchWall = false;
                }
                else
                    mTransform.position += trans;

            }
            else
            {
                if (mCollisionEnabled)
                {
                    float addtiveCheckLength = mRadius * 2;
                    if (trans.x != 0 || trans.z != 0)
                    {
                        // normalize direction.
                        Vector3 direction = new Vector3(trans.x, 0, trans.z);
                        float length = direction.magnitude;
                        direction /= length;

                        float backCheckOffset = 0.05f;
                        Vector3 checkPos = new Vector3(mPosition.x, mPosition.y + addtiveCheckLength, mPosition.z) - direction * backCheckOffset;
                        float checkLength = length + addtiveCheckLength + backCheckOffset;

                        RaycastHit hitInfo;
                        mOnTouchWall = false;

                        if (Physics.Raycast(checkPos, direction, out hitInfo, checkLength, mCacheLayerMask))
                        {
                            mOnTouchWall = true;
                            float hitDistance = hitInfo.distance - addtiveCheckLength;
                            if (hitDistance > 0)
                            {
                                trans.x = direction.x * hitDistance;
                                trans.z = direction.z * hitDistance;
                            }
                            else
                            {
                                trans.x = 0;
                                trans.z = 0;
                            }
                        }
                    }

                    mOnGround = false;
                    if (trans.y < 0)
                    {
                        RaycastHit hitInfo;
                        Vector3 checkPos = new Vector3(mPosition.x, mPosition.y + addtiveCheckLength, mPosition.z);
                        float checkLength = addtiveCheckLength - trans.y;
                        if (Physics.Raycast(checkPos, Vector3.down, out hitInfo, checkLength, mCacheLayerMask))
                        {
                            float hitDistance = hitInfo.distance - addtiveCheckLength;
                            if (Mathf.Abs(hitDistance) > 0.001f)
                                trans.y = -hitDistance;
                            else
                                trans.y = 0;
                            mOnGround = true;
                        }
                    }
                }
                else
                {
                    mOnGround = false;
                }

                if (trans == Vector3.zero)
                    return;

                mTransform.position += trans;
            }

            mPosition = mTransform.position;
        }

        AnimationState FetchAnimation(ActData.Action action, float speed)
        {
            if (action == null || action.AnimSlotList.Count == 0 || !mCachedAnimation)
                return null;

            ActData.AnimSlot animSlot = action.AnimSlotList[UnityEngine.Random.Range(0, action.AnimSlotList.Count)];
            AnimationState animState = mCachedAnimation[animSlot.Animation];
            if (animState == null)
            {
                Debug.LogError(string.Format("Fail to change animation: {0}/{1}/{2}", UnitID, action.Id, animSlot.Animation));
                return null;
            }

            animState.normalizedTime = animSlot.Start * 0.01f;
            animState.speed = speed * (animSlot.End - animSlot.Start) * animState.length * 10.0f / (action.AnimTime);
            return animState;
        }

        public void PlayAnimation(ActData.Action action, float speed)
        {
            AnimationState animState = FetchAnimation(action, speed);
            if (animState == null)
                return;

            mCachedAnimationState = animState;
            mAnimSpeed = animState.speed;

            float fadeLength = action.BlendTime * 0.001f;
            if (fadeLength == 0)
                mCachedAnimation.Play(animState.name);
            else
                mCachedAnimation.CrossFade(animState.name, fadeLength);
        }

        public void OnEnterPoseTime()
        {
            if (mCachedAnimationState != null)
                mCachedAnimationState.speed = 0.001f;
        }

        public void BeginStaight()
        {
            if (mCachedAnimationState != null)
                mCachedAnimationState.speed = 0.001f;
        }

        public void EndStaight()
        {
            if (mCachedAnimationState != null)
                mCachedAnimationState.speed = mAnimSpeed;
        }

        public void OnReachHighest(bool value)
        {
            mOnHighest = value;
        }

        public void OnHitGround(bool value)
        {
            mOnGround = value;
        }

        public void OnHitTarget(IActUnit target)
        {
            mHitTarget = target;
        }

        public void ClearFlags()
        {
            mHitTarget = null;
        }

        public virtual ECombatResult Combat(IActUnit target, ISkillItem skillItem)
        {
            return ECombatResult.ECR_Normal;
        }

        public void OnHit(HitData hitData, bool pvp)
        {
            // setup position.
            Vector3 position = Vector3.zero;
            float rotate = 0;
            NetCommon.Decode(hitData.HitX, hitData.HitY, hitData.HitZ, hitData.HitDir, ref position, ref rotate);

            // if the pos is too large than [xxx = 1.0f] ignore this hit data
            // to avoid flash moving...
            Vector3 offset = position - mPosition;
            if (pvp && offset.sqrMagnitude > 1.0f)
                return;

            Move(offset);
            SetOrientation(rotate);

            // avoid replay action.
            if (hitData.HitAction != byte.MaxValue && hitData.HitAction != mActionStatus.ActiveAction.ActionCache)
                mActionStatus.ChangeAction(hitData.HitAction, 0);

            // setup straight time.
            mActionStatus.SetStraightTime(hitData.StraightTime, true);

            // setup lash time.
            if (hitData.LashTime > 0)
            {
                mActionStatus.SetLashVelocity(
                    hitData.LashX * 0.01f,
                    hitData.LashY * 0.01f,
                    hitData.LashZ * 0.01f,
                    hitData.LashTime);
            }
        }
    }
}
