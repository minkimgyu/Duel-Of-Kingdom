using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.CAPTURE;
using WPP.AI.UNIT;
using System;

namespace WPP.AI.STAT
{
    // json �����͸� Stat ���·� �Ľ��ؼ� ��������
    [Serializable]
    abstract public class BaseStat
    {
        public int _id; // Entity ���̵�
        public int _level; // Entity ����
        public string _name; // �̸�
        public float _hp; // ü��

        public BaseStat(int id, int level, string name, float hp)
        {
            _id = id;
            _level = level;
            _name = name;
            _hp = hp;
        }

        public abstract void Initialize(Entity entity);
    }

    [Serializable]
    public class UnitStat : BaseStat
    {
        public float _damage; // ������
        public CaptureTag[] _targetTag; // Ÿ�� �±�
        public float _hitSpeed; // ���� �ӵ�
        public float _range; // ����

        public UnitStat(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range) : base(id, level, name, hp)
        {
            _damage = damage;
            _targetTag = targetTag;
            _hitSpeed = hitSpeed;
            _range = range;
        }

        public override void Initialize(Entity entity)
        => entity.Initialize(_id, _level, _name, _hp, _targetTag, _damage, _hitSpeed, _range);
    }

    [Serializable]
    public class AttackBuildingStat : BaseStat
    {
        public float _damage; // ������
        public CaptureTag[] _targetTag; // Ÿ�� �±�
        public float _hitSpeed; // ���� �ӵ�
        public float _range; // ����

        public AttackBuildingStat(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range) : base(id, level, name, hp)
        {
            _damage = damage;
            _targetTag = targetTag;
            _hitSpeed = hitSpeed;
            _range = range;
        }

        public override void Initialize(Entity entity)
        => entity.Initialize(_id, _level, _name, _hp, _targetTag, _damage, _hitSpeed, _range);
    }

    [Serializable]
    public class LivingOutAttackBuildingStat : AttackBuildingStat
    {
        public float _lifeTime; // ���� �ð�

        public LivingOutAttackBuildingStat(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float lifeTime) : base(id, level, name, hp, targetTag, damage, hitSpeed, range)
        {
            _lifeTime = lifeTime;
        }

        public override void Initialize(Entity entity)
        => entity.Initialize(_id, _level, _name, _hp, _targetTag, _damage, _hitSpeed, _range, _lifeTime);
    }

    [Serializable]
    public class LivingOutSpawnBuildingStat : BaseStat
    {
        public float _lifeTime; // ���� �ð�
        public int _spawnUnitId; // ������ų ������ id
        public int _spawnUnitCount; // ������ų ������ ����

        public LivingOutSpawnBuildingStat(int id, int level, string name, float hp, float lifeTime, int spawnUnitId, int spawnUnitCount) : base(id, level, name, hp)
        {
            _lifeTime = lifeTime;
            _spawnUnitId = spawnUnitId;
            _spawnUnitCount = spawnUnitCount;
        }

        public override void Initialize(Entity entity)
        => entity.Initialize(_id, _level, _name, _hp, _lifeTime, _spawnUnitId, _spawnUnitCount);
    }
}
