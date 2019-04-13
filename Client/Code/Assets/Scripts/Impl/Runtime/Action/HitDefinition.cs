//HitDefinition



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ACT
{
    public class HitDefinition
    {
        public static bool ShowAttackFrame = false; //是否显示攻击框
        public const int MAX_HIT_SOUND = 1;//最大击中音效

        ActData.AttackDef mAttackDef; //攻击定义
        Transform mCacheTransform; //
        public Transform CacheTransform { get { return mCacheTransform; } }
        IActUnit mOwner;
        string mAction;
        ISkillItem mSkillItem;
        Action<HitDefinition> mFinishHandle; //结束回调

        Vector3 mInitPos = Vector3.zero;
        Vector3 mPos = Vector3.zero;
        float mOrientation = 0;
        float mDelayTime = 0.0f;
        float mLifeTime = 0.0f;
        bool mOutofDate = false;
        int mHitSucessCount = 0;
        int mLastHitCount = 0;
        int mHitSoundCount = 0;
        GameObject mAttackFrame;
        bool mHitBlocked = false;

        bool mHaveHitOrt = false;
        Vector3 mHitOrientation = Vector3.zero;

        Vector3 mCubeHitDefSize = Vector3.zero;
        Vector2 mCylinderSize = Vector2.zero;
        Vector3 mRingSize = Vector3.zero;
        Vector4 mFanSize = Vector4.zero;

        Dictionary<GameObject, int> mHitedPassedMap = new Dictionary<GameObject, int>();
        ActEffectMgr mHitDefEffectMgr = new ActEffectMgr(); //特效管理
        bool mIsFinish; //是否已结束

        public HitDefinition()
        {
            mCacheTransform = new GameObject("HitDefinition").transform;
        }

        public void Init(ActData.AttackDef data, IActUnit owner, string action, ISkillItem skillItem, 
            Action<HitDefinition> finishHandle)
        {
            mAttackDef = data;
            mOwner = owner;
            mAction = action;
            mSkillItem = skillItem;
            mFinishHandle = finishHandle;

            mIsFinish = false;
            mInitPos = owner.Position;
            mOrientation = owner.Orientation;
            mDelayTime = mAttackDef.Delay * 0.001f;
            UpdatePosition(0);
            
            if (!string.IsNullOrEmpty(mAttackDef.SelfEffect))
            {
                Vector3 tmpPos = new Vector3(mAttackDef.SelfEffectOffset.X * 0.01f,
                            mAttackDef.SelfEffectOffset.Y * 0.01f,
                            mAttackDef.SelfEffectOffset.Z * 0.01f);

                mHitDefEffectMgr.PlayEffect(mAttackDef.SelfEffect, mAttackDef.SelfEffectDuration * 0.001f,
                    mCacheTransform, tmpPos, mOwner.Transform.rotation);
            }
        }

        //重置
        public void Reset()
        {
            mLifeTime = 0.0f;
            mOutofDate = false;
            mHitSucessCount = 0;
            mLastHitCount = 0;
            mHitSoundCount = 0;
            mHitBlocked = false;
            mHaveHitOrt = false;
            mHitedPassedMap.Clear();
            mHitDefEffectMgr.Clear();
        }

        //释放
        public void Release()
        {
        }

        //更新
        public void Update(float deltaTime)
        {
            if (mIsFinish)
            {
                return;
            }

            mHitDefEffectMgr.Update(deltaTime);
            
            if (mDelayTime > deltaTime)
            {
                mDelayTime -= deltaTime;
                return;
            }

            if (!mOwner.UGameObject)
                mOutofDate = true;
            else if (mLifeTime >= mAttackDef.Duration * 0.001f)
                mOutofDate = true;
            else if (mAttackDef.MaxHitCount > 0 && mHitSucessCount >= mAttackDef.MaxHitCount)
                mOutofDate = true;
            else if (mAttackDef.OwnerActionChange == 1 && mAction != mOwner.ActStatus.ActiveAction.Id)//更换技能，并且技能不是同一个
                mOutofDate = true;

            if (mOutofDate || mHitBlocked || !mOwner.UGameObject)
            {
                mIsFinish = true;
                mFinishHandle?.Invoke(this);
                mFinishHandle = null;

                if (mAttackFrame)
                {
                    GameObject.Destroy(mAttackFrame);
                    mAttackFrame = null;
                }

                return;
            }

            UpdatePosition(mLifeTime * 100.0f * 1000.0f / mAttackDef.Duration);//mLifeTime * 100.0f * 1000.0f/mData1.Duration表示攻击的持续比例

            UpdateAttackFram(deltaTime);

            mHitSoundCount = 0;
            while (mLastHitCount < mAttackDef.HitCount)
            {
                float checkTime = mLastHitCount * mAttackDef.Duration * 0.001f / mAttackDef.HitCount;
                if (checkTime >= mLifeTime + deltaTime)
                    break;

                CheckHit(mOwner);

                mLastHitCount++;
            }

            mLifeTime += deltaTime;
        }

        void UpdatePosition(float ratio)
        {
            if (mAttackDef.FllowReleaser != 0 || mAttackDef.KeepLocal != 0 || mAttackDef.FramType == ActData.HitDefnitionFramType.SomatoType)
                mInitPos = mOwner.Position;//FllowReleaser是否跟随释放者。KeepLocal是否相对施放者位移，
                                           //FramType技能击中框的类型:CuboidType为长方体;CylinderType为立方体; RingType:为圆环形; SomatoType:为受击体"

            Vector3 pos = Vector3.zero;
            Interplate(ratio, ref pos);

            RoatatePos(ref pos);

            mPos = mInitPos + pos * 0.01f;
            mCacheTransform.position = mPos;
        }

        void UpdateAttackFram(float deltaTime)
        {
            //if(!mHaveHitOrt)
            //Debug.Log("mHaveHitOrt is false!");
            switch (mAttackDef.FramType)
            {
                case ActData.HitDefnitionFramType.CuboidType:
                    {
                        ActData.FrameCuboid data = mAttackDef.AttackType.FrameCuboid;
                        mCubeHitDefSize.x = data.Width * 0.01f;//攻击x范围
                        mCubeHitDefSize.y = data.Height * 0.01f;//攻击y范围
                        mCubeHitDefSize.z = data.Length * 0.01f;//攻击z范围						

                        if (ShowAttackFrame)
                        {
                            if (mAttackFrame != null)
                                GameObject.Destroy(mAttackFrame);

                            mAttackFrame = (GameObject)GameObject.Instantiate(Resources.Load("HitDefinitionCube"));
                            mAttackFrame.transform.localScale = mCubeHitDefSize;

                            mAttackFrame.transform.localPosition = new Vector3(
                            mCacheTransform.position.x,
                            mCacheTransform.position.y + mCubeHitDefSize.y / 2.0f,
                            mCacheTransform.position.z);
                            if (!mHaveHitOrt)
                            {
                                mHitOrientation = mOwner.UGameObject.transform.localEulerAngles;
                                mHaveHitOrt = true;
                            }
                            if (mAttackFrame != null)
                                mAttackFrame.transform.localEulerAngles = mHitOrientation;

                        }
                    }
                    break;
                case ActData.HitDefnitionFramType.CylinderType:
                    {
                        ActData.FrameCylinder data = mAttackDef.AttackType.FrameCylinder;
                        mCylinderSize.x = data.Radius * 0.01f;
                        mCylinderSize.y = data.Height * 0.01f;

                        if (ShowAttackFrame)
                        {
                            if (mAttackFrame != null)
                                GameObject.Destroy(mAttackFrame);

                            mAttackFrame = (GameObject)GameObject.Instantiate(Resources.Load("HitDefinitionCylinder"));
                            mAttackFrame.transform.localPosition = new Vector3(mCacheTransform.position.x,
                            mCacheTransform.position.y + mCylinderSize.y / 2.0f, mCacheTransform.position.z);
                            mAttackFrame.transform.localScale = new Vector3(2 * mCylinderSize.x, mCylinderSize.y / 2.0f, 2 * mCylinderSize.x);
                            if (!mHaveHitOrt)
                            {
                                mHitOrientation = mOwner.UGameObject.transform.localEulerAngles;
                                mHaveHitOrt = true;
                            }
                            if (mAttackFrame != null)
                                mAttackFrame.transform.localEulerAngles = mHitOrientation;
                        }
                    }
                    break;
                case ActData.HitDefnitionFramType.RingType:
                    {
                        ActData.FrameRing data = mAttackDef.AttackType.FrameRing;
                        mRingSize.x = data.InnerRadius * 0.01f;
                        mRingSize.y = data.Height * 0.01f;
                        mRingSize.z = data.OuterRadius * 0.01f;
                    }
                    break;
                case ActData.HitDefnitionFramType.SomatoType:
                    {
                        mCubeHitDefSize = mOwner.ActStatus.Bounding;
                    }
                    break;
                case ActData.HitDefnitionFramType.FanType:
                    {
                        ActData.FrameFan data = mAttackDef.AttackType.FrameFan;
                        mFanSize.x = data.Radius * 0.01f;
                        mFanSize.y = data.Height * 0.01f;
                        mFanSize.z = data.StartAngle;
                        mFanSize.w = data.EndAngle;
                    }
                    break;
            }

        }

        void Interplate(float ratio, ref Vector3 pos)
        {
            if (mAttackDef.Path.Count == 0) //移动路径，mData1.Path.Count 表示这个路径上的数目
                pos = Vector3.zero;
            else if (mAttackDef.Path.Count == 1) //移动路径数目为1
            {
                pos.x = mAttackDef.Path[0].X;
                pos.y = mAttackDef.Path[0].Y;
                pos.z = mAttackDef.Path[0].Z;
            }
            else//移动数目大于1的情况
            {
                for (int i = 1; i < mAttackDef.Path.Count; i++)
                {
                    ActData.AttackDef.Types.PathNode preNode = mAttackDef.Path[i - 1];//路径上的前一个节点
                    ActData.AttackDef.Types.PathNode curNode = mAttackDef.Path[i];//路径上的当前节点
                    if (ratio < curNode.Ratio)//百分比
                    {
                        float alpha = (ratio - preNode.Ratio) / (curNode.Ratio - preNode.Ratio);
                        pos.x = Mathf.Lerp(preNode.X, curNode.X, alpha);//按照alpha的比例在preNode和curNode之间插值
                        pos.y = Mathf.Lerp(preNode.Y, curNode.Y, alpha);//同上
                        pos.z = Mathf.Lerp(preNode.Z, curNode.Z, alpha);//同上
                        break;
                    }

                    if (curNode.Ratio == 0)//当前节点不占比例，直接用前面的内容来赋值
                    {
                        pos.x = preNode.X;
                        pos.y = preNode.Y;
                        pos.z = preNode.Z;
                        break;
                    }
                }
            }
        }

        void RoatatePos(ref Vector3 pos)
        {
            float x = pos.x, z = pos.z;
            MathUtility.Rotate(ref x, ref z, mOrientation);
            pos.x = x;
            pos.z = z;
        }

        void CheckHit(IActUnit self)
        {
            // monster only attack local player.
            //if (self.UnitType == EUnitType.EUT_Monster &&
            //    self.Camp == EUnitCamp.EUC_ENEMY &&
            //    mAttackDef.Race == Data1.RaceType.Enemy)
            //{
            //    Unit target = UnitManager.Instance.LocalPlayer;
            //    if (target != null && target.UUnitInfo && CanHit(self, target))
            //        CheckHit(self, target);
            //    return;
            //}

            int comboHit = 0;

            for (int i = 0, max = ActionSystem.Instance.ActUnitMgr.Units.Count; i < max; ++i)
            {
                var tmpTarget = ActionSystem.Instance.ActUnitMgr.Units[i];

                if (!tmpTarget.UGameObject || tmpTarget.Dead || !CanHit(self, tmpTarget))
                    continue;

                if (CheckHit(self, tmpTarget))
                {
                    comboHit++;
                }
            }
        }

        bool CheckHit(IActUnit self, IActUnit target)
        {
            // 转换offset到世界坐标系
            ActionStatus targetActionStatus = target.ActStatus;
            ActData.Action targetAction = targetActionStatus.ActiveAction;
            float BoundOffsetX = targetAction.BoundingOffsetX;
            float BoundOffsetY = targetAction.BoundingOffsetY;
            float BoundOffsetZ = targetAction.BoundingOffsetZ;
            MathUtility.Rotate(ref BoundOffsetX, ref BoundOffsetZ, target.Orientation);

            Vector3 AttackeePos = target.Position + new Vector3(
                BoundOffsetX, BoundOffsetY, BoundOffsetZ) * 0.01f;

            bool hitSuccess = false;

            switch (mAttackDef.FramType)
            {
                case ActData.HitDefnitionFramType.CuboidType:
                case ActData.HitDefnitionFramType.SomatoType:
                    // 四面体求交。
                    if (MathUtility.RectangleHitDefineCollision(
                        mPos, mOrientation,
                        mCubeHitDefSize,
                        AttackeePos, target.Orientation,
                        targetActionStatus.Bounding))
                    {
                        hitSuccess = true;
                    }
                    break;
                case ActData.HitDefnitionFramType.CylinderType:
                    // 圆柱求交
                    if (MathUtility.CylinderHitDefineCollision(
                        mPos, mOrientation,
                        mCylinderSize.x, mCylinderSize.y,
                        AttackeePos, target.Orientation,
                        targetActionStatus.Bounding))
                    {
                        hitSuccess = true;
                    }
                    break;
                case ActData.HitDefnitionFramType.RingType:
                    if (MathUtility.RingHitDefineCollision(
                        mPos, mOrientation,
                        mRingSize.x, mRingSize.y, mRingSize.z,
                        AttackeePos, target.Orientation,
                        targetActionStatus.Bounding))
                    {
                        hitSuccess = true;
                    }
                    break;
                case ActData.HitDefnitionFramType.FanType:
                    if (MathUtility.FanDefineCollision(
                        mPos, mOrientation,
                        mFanSize.x, mFanSize.y, mFanSize.z, mFanSize.w,
                        AttackeePos, target.Orientation,
                        targetActionStatus.Bounding))
                    {
                        hitSuccess = true;
                    }
                    break;
            }

            if (hitSuccess)
                return ProcessHit(target);

            return false;
        }

        bool CanHit(IActUnit self, IActUnit target)
        {
            if (mAttackDef.Race != ActData.RaceType.Self && self == target)
                return false;

            if (mAttackDef.Race == ActData.RaceType.Enemy && self.Camp == target.Camp)
                return false;

            if (mAttackDef.Race == ActData.RaceType.TeamMember && self.Camp != target.Camp)
                return false;

            // 如果攻击高度不符合要求，停止击中判定
            if ((mAttackDef.HeightStatusHitMaskInt & (1 << target.ActStatus.ActiveAction.HeightStatus)) == 0)
                return false;

            // 如果当前动作不接受受伤攻击，停止击中判定。
            if (!target.ActStatus.CanHurt)
                return false;

            int hitCount = 0;
            if (mAttackDef.PassNum > 0 && mHitedPassedMap.TryGetValue(target.UGameObject, out hitCount) && hitCount >= mAttackDef.PassNum)
                return false;

            return true;
        }

        bool ProcessHit(IActUnit target)
        {
            // 设置穿越次数
            int hitCount = 0;
            if (mAttackDef.PassNum > 0)
            {
                mHitedPassedMap.TryGetValue(target.UGameObject, out hitCount);
                mHitedPassedMap[target.UGameObject] = ++hitCount;
            }

            // 累加击中次数。
            mHitSucessCount++;

            // 召唤出来的单位，属性集中需要计算来之父亲的属性。
            IActUnit owner = mOwner;

            if (owner.Owner != null)
                owner = owner.Owner;

            // 击中目标的技能Buff
            if (mSkillItem != null && mSkillItem.SkillInput != null)
                mSkillItem.SkillInput.OnHitTarget(target);

            // 被格挡住了，执行格挡回弹动作。
            if (owner.Combat(target, mSkillItem) == ECombatResult.ECR_Block)
            {
                // do not process hit result in pvp mode.
                //if (PvpClient.Instance != null && mOwner.UnitType != EUnitType.EUT_LocalPlayer)
                //    return false;

                //if (mAttackDef.IsRemoteAttacks == 0 && mSkillItem == null)
                //{
                //    owner.PlayAction(ActData.CommonAction.Bounce);
                //}

                mHitBlocked = true;

                owner.PlayAction(ActData.CommonAction.Bounce);
                return false;
            }

            // 击中目标。
            Hit(owner, target);

            // 设置攻击者的硬直时间和速度。
            mOwner.ActStatus.SetStraightTime(mAttackDef.AttackerStraightTime, false);
            return true;
        }

        void Hit(IActUnit self, IActUnit target)
        {
            ActionStatus targetActionStatus = target.ActStatus;

            // hit target.
            self.OnHitTarget(target);

            // sound.
            /*
            if (!string.IsNullOrEmpty(mData1.HitedSound) && mHitSoundCount < MAX_HIT_SOUND)
            {
                if (mData1.HitedSoundIndex == -2)
                {
                    mData1.HitedSoundIndex = SoundManager.Instance.GetSoundIndex(mData1.HitedSound);
                    if (mData1.HitedSoundIndex < 0)
                        Debug.LogError(string.Format("Fail to load hit sound: [{0}/{1}]", mOwner.UnitID, mData1.HitedSound));
                }
                if (mData1.HitedSoundIndex > 0)
                    SoundManager.Instance.Play3DSound(mData1.HitedSoundIndex, mOwner.Position, 1.0f);
                mHitSoundCount++;
            }*/

            // effect
            if (!string.IsNullOrEmpty(mAttackDef.HitedEffect))
            {
                Vector3 effectPos = target.Position;
                effectPos.y += targetActionStatus.Bounding.y * 0.5f;

                ActionSystem.Instance.EffectMgr.PlayEffect(mAttackDef.HitedEffect, mAttackDef.HitedEffectDuration * 0.001f, 
                    null, effectPos,
                    0 == mAttackDef.BaseGround ? Quaternion.identity : Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
            }

            // do not process my hit result in pvp mode.
            //if (PvpClient.Instance != null && self.UnitType == EUnitType.EUT_LocalPlayer)
            //    return;

            HitData tmpHitData = new HitData();
            tmpHitData.TargetUUID = target.UUID;

            // 击中转向
            float targetRotate = target.Orientation;
            bool rotateOnHit = targetActionStatus.RotateOnHit;
            if (rotateOnHit)
            {
                if (mAttackDef.FramType == ActData.HitDefnitionFramType.CylinderType)
                {
                    float x = target.Position.x - mPos.x;
                    float z = target.Position.z - mPos.z;
                    float modify = Mathf.Atan2(x, 0);
                    targetRotate = modify + Mathf.PI;
                }
                else
                    targetRotate = self.Orientation + Mathf.PI;
            }

            NetCommon.Encode(
                target.Position,
                targetRotate,
                ref tmpHitData.HitX,
                ref tmpHitData.HitY,
                ref tmpHitData.HitZ,
                ref tmpHitData.HitDir);

            // 单位在墙边上的时候，近战攻击者需要反弹。
            bool bounceBack = mAttackDef.IsRemoteAttacks == 0 && target.OnTouchWall;

            // 单位处于非霸体状态，需要被击中移动～
            bool processLash = true;

            //受击动作
            ActData.HeightStatusFlag targetHeightStatus = targetActionStatus.HeightState;

            if (targetActionStatus.ActiveAction.SuperArmor || target.IsPabodyState)
            {
                // 设置受击者的霸体硬直时间?
                tmpHitData.HitAction = byte.MaxValue;

                // 单位处于霸体状态，不需要移动～
                processLash = false;

                // 攻击结果为霸体的情况系，非远程攻击的冲击速度转换为攻击者。受击者不受冲击速度影响
                bounceBack = mAttackDef.IsRemoteAttacks == 0;
            }
            else if (targetActionStatus.OnHit(mAttackDef.HitResult, mAttackDef.IsRemoteAttacks != 0))
            {
                tmpHitData.HitAction = (byte)targetActionStatus.ActiveAction.ActionCache;
            }

            // 处理buff的东东
            if (targetActionStatus.SkillItem != null && targetActionStatus.SkillItem.SkillInput != null)
                targetActionStatus.SkillItem.SkillInput.OnHit(self);

            // 设置攻击者的冲击速度及冲击时间。
            int attackerLashTime = mAttackDef.AttackerTime;
            Vector3 attackerLash = attackerLashTime == 0 ? self.ActStatus.Velocity : new Vector3(
                mAttackDef.AttackerLash.X * 0.01f,
                mAttackDef.AttackerLash.Y * 0.01f,
                mAttackDef.AttackerLash.Z * 0.01f);
            if (bounceBack)
            {
                attackerLash.x = mAttackDef.AttackeeLash.X * 0.01f;
                attackerLash.z = mAttackDef.AttackeeLash.Z * 0.01f;
                attackerLashTime = mAttackDef.AttackeeTime;
            }

            if (attackerLashTime > 0)
            {
                self.ActStatus.SetLashVelocity(
                    attackerLash.x,
                    attackerLash.y,
                    attackerLash.z,
                    attackerLashTime);
            }

            // 处理受击者的冲击速度～
            LashProcess(mAttackDef, ref tmpHitData, target, targetHeightStatus, processLash, rotateOnHit);

            // I was hited, tell the others.
            //if (self.UnitType == EUnitType.EUT_OtherPlayer && target.UnitType == EUnitType.EUT_LocalPlayer)
            //{
            //    if (target.ActStatus.Listener != null)
            //        target.ActStatus.Listener.OnHitData(tmpHitData);
            //}

            target.OnHit(tmpHitData, false);
        }

        void LashProcess(ActData.AttackDef attackDef,
            ref HitData hitData,
            IActUnit target,
            ActData.HeightStatusFlag targetHeightStatus,
            bool processLash,
            bool rotateOnHit)
        {
            int AttackeeStraightTime = attackDef.AttackeeStraightTime;
            float AttackeeLashX = attackDef.AttackeeLash.X;
            float AttackeeLashY = attackDef.AttackeeLash.Y;
            float AttackeeLashZ = attackDef.AttackeeLash.Z;
            int AttackeeTime = attackDef.AttackeeTime;

            ActData.AttackDef.Types.HitResultData hitResultData = null;
            switch (targetHeightStatus)
            {
                case ActData.HeightStatusFlag.Ground:
                    hitResultData = attackDef.GroundHit;
                    break;
                case ActData.HeightStatusFlag.LowAir:
                    hitResultData = attackDef.LowAirHit;
                    break;
                case ActData.HeightStatusFlag.HighAir:
                    hitResultData = attackDef.HighAirHit;
                    break;
            }

            if (hitResultData != null && hitResultData.Enabled)
            {
                AttackeeLashX = hitResultData.AttackeeLash.X;
                AttackeeLashY = hitResultData.AttackeeLash.Y;
                AttackeeLashZ = hitResultData.AttackeeLash.Z;
                AttackeeTime = hitResultData.AttackeeTime;
                AttackeeStraightTime = hitResultData.AttackeeStraightTime;
            }

            if (processLash)
            {
                // 非受击转向的时候，冲击速度需要转换为本地坐标。
                if (!rotateOnHit)
                {
                    Quaternion rotate = Quaternion.AngleAxis(mOrientation * Mathf.Rad2Deg + 180, Vector3.up);
                    if (mAttackDef.FramType != ActData.HitDefnitionFramType.CuboidType)
                    {
                        Vector3 targetToOwner = mPos - target.Position;
                        targetToOwner.y = 0;
                        rotate = Quaternion.LookRotation(targetToOwner);
                    }
                    Vector3 lashVector = rotate * new Vector3(AttackeeLashX, AttackeeLashY, AttackeeLashZ);
                    lashVector = target.UGameObject.transform.InverseTransformDirection(lashVector);

                    AttackeeLashX = (short)lashVector.x;
                    AttackeeLashZ = (short)lashVector.z;
                }

                hitData.LashX = (short)AttackeeLashX;
                hitData.LashY = (short)AttackeeLashY;
                hitData.LashZ = (short)AttackeeLashZ;
                hitData.LashTime = (short)AttackeeTime;
            }

            hitData.StraightTime = (short)AttackeeStraightTime;
        }
    }
}


