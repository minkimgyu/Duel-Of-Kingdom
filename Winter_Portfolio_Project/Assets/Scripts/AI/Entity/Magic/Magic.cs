using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.ATTACK;
using WPP.AI.TIMER;

namespace WPP.AI
{
    abstract public class Magic : Entity
    {
        // �⺻ �ɷ�ġ
        protected float _damage; // ������
        protected float _range; // ����

        public override bool CanAttachHpBar() { return false; }

        abstract protected void DoStartTask(); // �ʱ�ȭ�� �� �۵��ؾ��ϴ� ��� �߰�
        abstract protected void DoMainTask(); // ���� �����ؾ��� ��ɵ� �߰�
        protected virtual void DestroyThis() { Destroy(gameObject); } // ���� ������Ʈ �ı�
    }

    abstract public class ProjectileMagic : Magic
    {
        protected RangeDamageComponent _rangeDamageComponent; // ���� ���ݿ� ������Ʈ
        protected Vector3 _projectileStartPosition; // ���󰡴� ������Ʈ �ʱ� ��ġ

        protected float _speed; // ����ü �̵� �ӵ�
        protected float _durationBeforeDestroy; // ����ü �۵� ���� �ı����� �Ⱓ

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
