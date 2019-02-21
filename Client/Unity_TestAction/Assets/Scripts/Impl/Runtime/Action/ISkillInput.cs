using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{
    public interface ISkillInput
    {
        void PlaySkill();

        void OnHitTarget(IActUnit target);

        void OnHit(IActUnit target);

        void OnHurt(IActUnit target);
    }
}
