using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.EFFECT;

namespace WPP
{
    public class ParticleEffect : BaseEffect
    {
        ParticleSystem[] _particleSystems;
        float _duration;

        void Awake()
        {
            InitializeParticles();
            _duration = ReturnMaxDuration();
        }

        public override void StopEffect()
        {
            for (int i = 0; i < _particleSystems.Length; i++) _particleSystems[i].Stop();
            DisableObject();
        }

        public override void PlayEffect()
        {
            for (int i = 0; i < _particleSystems.Length; i++) _particleSystems[i].Play();
            Invoke("DisableObject", _duration); // ���� �ʰ� ������ ��ƼŬ �������� ��Ȱ��ȭ ����
        }

        void InitializeParticles()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        float ReturnMaxDuration()
        {
            float duration = 0;
            for (int i = 0; i < _particleSystems.Length; i++)
            {
                if (i == 0) duration = _particleSystems[i].main.duration;
                else
                {
                    // ����� ������ �� ū ���
                    if (duration < _particleSystems[i].main.duration)
                        duration = _particleSystems[i].main.duration;
                }
            }

            return duration;
        }
    }
}
