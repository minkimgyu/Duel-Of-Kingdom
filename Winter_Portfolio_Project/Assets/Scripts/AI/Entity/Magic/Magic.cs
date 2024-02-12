using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.ATTACK;
using WPP.AI.TIMER;

namespace WPP.AI
{
    abstract public class Magic : Entity
    {
        // 기본 능력치
        protected float _damage; // 데미지
        protected float _range; // 범위

        public override bool CanAttachHpBar() { return false; }

        abstract protected void DoStartTask(); // 초기화될 때 작동해야하는 기능 추가
        abstract protected void DoMainTask(); // 이후 동착해야할 기능들 추가
        protected virtual void DestroyThis() { Destroy(gameObject); } // 게임 오브젝트 파괴
    }

    abstract public class ProjectileMagic : Magic
    {
        protected RangeDamageComponent _rangeDamageComponent; // 범위 공격용 컴포넌트
        protected Vector3 _projectileStartPosition; // 날라가는 오브젝트 초기 위치

        protected float _speed; // 투사체 이동 속도
        protected float _durationBeforeDestroy; // 투사체 작동 이후 파괴까지 기간

        public override void Initialize(int level, string name, float range, float durationBeforeDestroy, float damage, float speed)
        {
            _level = level;
            _name = name;

            _damage = damage;
            _range = range;

            _speed = speed;
            _durationBeforeDestroy = durationBeforeDestroy;

            InitializeComponent();
            DoStartTask();
        }

        public override void ResetMagicStartPosition(Vector3 pos) => _projectileStartPosition = pos;

        protected override void InitializeComponent() => _rangeDamageComponent = GetComponent<RangeDamageComponent>();
    }
}
