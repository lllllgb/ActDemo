using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{
    public interface IActEffect
    {
        Transform EffectTrans { get; }

        bool IsInvalid { get; }

        void Init(string name, float duration, Vector3 pos, Quaternion rotation);

        void Play();

        void Update(float deltaTime);

        void Stop();

        void Dispose();
    }
}
