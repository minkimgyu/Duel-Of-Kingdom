using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.CAPTURE;

namespace WPP.AI.BUILDING
{
    public class CannonTower : AttackBuilding, ILiveOut
    {
        public float LifeTime { get; set; }
        public float PassedTime { get; set; }

        public override void Initialize(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float lifeTime)
        {
            _id = id;
            _level = level;
            _name = name;

            _maxHp = hp; // �ִ� ü�� ����
            HP = hp;
            _damage = damage;
            _hitSpeed = hitSpeed;
            _range = range;
            LifeTime = lifeTime; // ������ Ÿ�� �ʱ�ȭ
            PassedTime = 0;

            InitializeComponent();

            _captureComponent.Initialize(targetTag); // �̷� ������ ���� ������ �Ҵ����ش�.
        }

        public void DecreaseHp()
        {
            PassedTime += Time.deltaTime;

            float tickTime = 0.5f; // 0.5�� ���� ü���� ���ݾ� ������
            if(PassedTime >= tickTime)
            {
                float damagePerSecond = (_maxHp / LifeTime) * tickTime; // tickTime ���� �پ�� ü��
                HP -= damagePerSecond; // ��� ü���� �ٿ���
                PassedTime = 0;
            }
        }

        protected override void Update()
        {
            base.Update();
            DecreaseHp(); // ���⼭ ���������� ü���� �ް� �������
        }
    }
}
