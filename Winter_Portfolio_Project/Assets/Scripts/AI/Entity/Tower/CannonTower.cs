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

            _maxHp = hp; // 최대 체력 지정
            HP = hp;
            _damage = damage;
            _hitSpeed = hitSpeed;
            _range = range;
            LifeTime = lifeTime; // 라이프 타임 초기화
            PassedTime = 0;

            InitializeComponent();

            _captureComponent.Initialize(targetTag); // 이런 식으로 세부 변수를 할당해준다.
        }

        public void DecreaseHp()
        {
            PassedTime += Time.deltaTime;

            float tickTime = 0.5f; // 0.5초 마다 체력이 조금씩 떨어짐
            if(PassedTime >= tickTime)
            {
                float damagePerSecond = (_maxHp / LifeTime) * tickTime; // tickTime 마다 줄어들 체력
                HP -= damagePerSecond; // 계속 체력을 줄여줌
                PassedTime = 0;
            }
        }

        protected override void Update()
        {
            base.Update();
            DecreaseHp(); // 여기서 지속적으로 체력이 달게 해줘야함
        }
    }
}
