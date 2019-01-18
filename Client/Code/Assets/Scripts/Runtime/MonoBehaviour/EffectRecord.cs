using UnityEngine;
using System.Collections;

public class EffectRecord : MonoBehaviour
{
    public ParticleSystem[] Particles;
    public Animation[] Animations;

    public bool mIsInit = false;

    public void Play()
    {
        if (!mIsInit)
        {
            Init();
        }

        if (Particles != null && Particles.Length > 0)
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                ParticleSystem tmpParticle = Particles[i];

                if (null != tmpParticle)
                {
                    tmpParticle.Stop();
                    tmpParticle.Play();
                }
            }
        }

        if (Animations != null && Animations.Length > 0)
        {
            for (int i = 0; i < Animations.Length; i++)
            {
                Animation tmpAnima = Animations[i];

                if (null != tmpAnima)
                {
                    tmpAnima.Play();
                }
            }
        }
    }

    public void Stop()
    {
        if (Particles != null && Particles.Length > 0)
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                ParticleSystem tmpParticle = Particles[i];

                if (null != tmpParticle)
                {
                    tmpParticle.Stop();
                }
            }
        }

        if (Animations != null && Animations.Length > 0)
        {
            for (int i = 0; i < Animations.Length; i++)
            {
                Animation tmpAnima = Animations[i];

                if (null != tmpAnima)
                {
                    tmpAnima.Stop();
                }
            }
        }
    }

    [ContextMenu("Init")]
    public void Init()
    {
        mIsInit = true;
        Particles = GetParticles(gameObject);
        Animations = GetAnimations(gameObject);
    }

    public static ParticleSystem[] GetParticles(GameObject go)
    {
        return go?.GetComponentsInChildren<ParticleSystem>(true);
    }

    public static Animation[] GetAnimations(GameObject go)
    {
        return go?.GetComponentsInChildren<Animation>(true);
    }
}
