using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{
    public interface IActUnit
    {
        long UUID { get; }

        GameObject UGameObject { get; }

        Transform Transform { get; }

        Transform ModelTrans { get; }

        Vector3 Position { get; }

        int ActionID { get; }

        int Level { get; }

        int Speed { get; }

        float Radius { get; }

        bool Dead { get; }

        bool CanHurt { get; }

        int ActionGroupIdx { get; }

        float Orientation { get; }

        ActionStatus ActStatus { get; }

        EUnitCamp Camp { get; }

        int CurHp { get; }

        int HpMax { get; }

        bool OnGround { get; }

        bool OnTouchWall { get; }

        bool OnHighest { get; }

        IActUnit HitTarget { get; }

        IActUnit Owner { get; }

        int AIDiff { get; }


        void Update(float deltaTime);

        void SetOrientation(float orient);

        void Move(Vector3 trans);

        CustomVariable GetVariable(int idx);

        void EnableCollision(bool enable);

        void ClearFlags();

        void OnEnterPoseTime();

        void OnReachHighest(bool value);

        void PlayAction(string action);

        void PlayAnimation(ActData.Action action, float speed);

        void Destory();

        void BeginStaight();

        void EndStaight();

        ECombatResult Combat(IActUnit target, ISkillItem skillItem);

        void OnHitTarget(IActUnit target);

        void OnHit(HitData hitData, bool pvp);
    }
}
