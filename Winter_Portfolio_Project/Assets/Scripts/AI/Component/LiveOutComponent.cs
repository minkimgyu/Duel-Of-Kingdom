using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WPP.AI
{
    public class LiveOutComponent : MonoBehaviour
    {
        float _maxHp;
        float _lifeTime;
        float _passedTime;

        Action<float> OnDamageRequested;

        public void Initialize(float maxHp, float lifeTime, Action<float> OnDamage)
        {
            _maxHp = maxHp;
            _lifeTime = lifeTime;
            _passedTime = 0f;
            OnDamageRequested = OnDamage;
        }

        private void Update() => DecreaseHp();

        public void DecreaseHp()
        {
            _passedTime += Time.deltaTime;

            float tickTime = 1f; // 1�� ���� ü���� ���ݾ� ������
            if (_passedTime >= tickTime)
            {
                float damagePerSecond = (_maxHp / _lifeTime) * tickTime; // tickTime ���� �پ�� ü��
                OnDamageRequested?.Invoke(damagePerSecond);
                _passedTime = 0;
            }
        }
    }
}
