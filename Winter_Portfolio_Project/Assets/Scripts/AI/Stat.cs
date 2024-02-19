using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.CAPTURE;
using WPP.AI.GRID;
using System;

namespace WPP.AI.STAT
{
    [Serializable]
    public struct SerializableVector3
    {
        public float _x, _y, _z;

        public SerializableVector3(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Vector3 ConvertToV3() { return new Vector3(_x, _y, _z); }
    }

    [Serializable]
    public struct SerializableVector2
    {
        public float _x, _y;

        public SerializableVector2(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public Vector2 ConvertToV2() { return new Vector2(_x, _y); }
    }


    // json �����͸� Stat ���·� �Ľ��ؼ� ��������
    [Serializable]
    public class BaseStat
    {
        public int _id; // Entity ���̵�
        public int _level; // Entity ����
        public string _name; // �̸�

        public BaseStat(int id, int level, string name)
        {
            _id = id;
            _level = level;
            _name = name;
        }

        public virtual void ResetData(Entity entity) { }
    }

    // json �����͸� Stat ���·� �Ľ��ؼ� ��������
    [Serializable]
    public class MagicStat : BaseStat
    {
        public float _range; // ����
        public float _durationBeforeDestroy; // Task ���� �� �Ⱓ

        public MagicStat(int id, int level, string name, float range, float durationBeforeDestroy) : base(id, level, name)
        {
            _range = range;
            _durationBeforeDestroy = durationBeforeDestroy;
        }
    }

    // json �����͸� Stat ���·� �Ľ��ؼ� ��������
    [Serializable]
    public class ProjectileMagicStat : MagicStat
    {
        // �⺻ �ɷ�ġ
        public float _damage; // ������
        public float _speed; // Task ���� �� ������

        public ProjectileMagicStat(int id, int level, string name, float range, float durationBeforeDestroy, float damage, float speed) : base(id, level, name, range, durationBeforeDestroy)
        {
            _damage = damage;
            _speed = speed;
        }

        public override void ResetData(Entity entity)
        => entity.Initialize(_level, _name, _range, _durationBeforeDestroy, _damage, _speed);
    }

    [Serializable]
    public class LifeStat : BaseStat
    {
        public float _hp; // ü��

        public LifeStat(int id, int level, string name, float hp) : base(id, level, name)
        {
            _hp = hp;
        }
    }

    [Serializable]
    public class UnitStat : LifeStat
    {
        public float _damage; // ������
        public CaptureTag[] _targetTag; // Ÿ�� �±�
        public float _hitSpeed; // ���� �ӵ�
        public float _range; // ����
        public float _captureRange; // Ž�� ����

        public float _moveSpeed; // ������ �ӵ�
        public float _rotationSpeed; // ȸ�� �ӵ�

        public UnitStat(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange) : base(id, level, name, hp)
        {
            _damage = damage;
            _targetTag = targetTag;
            _hitSpeed = hitSpeed;
            _range = range;
            _captureRange = captureRange;
        }

        public override void ResetData(Entity entity)
        => entity.Initialize(_level, _name, _hp, _targetTag, _damage, _hitSpeed, _range, _captureRange);
    }

    [Serializable]
    public class BuildingStat : LifeStat
    {
        public OffsetRect _fillOffset;

        public BuildingStat(int id, int level, string name, float hp, OffsetRect fillOffset) : base(id, level, name, hp)
        {
            _fillOffset = fillOffset;
        }
    }

    [Serializable]
    public class AttackBuildingStat : BuildingStat
    {
        public float _damage; // ������
        public CaptureTag[] _targetTag; // Ÿ�� �±�
        public float _hitSpeed; // ���� �ӵ�
        public float _range; // ����
        public float _captureRange; // Ž�� ����

        public AttackBuildingStat(int id, int level, string name, float hp, OffsetRect fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange) : base(id, level, name, hp, fillOffset)
        {
            _damage = damage;
            _targetTag = targetTag;
            _hitSpeed = hitSpeed;
            _range = range;
            _captureRange = captureRange;
        }

        public override void ResetData(Entity entity)
        => entity.Initialize(_level, _name, _hp, _fillOffset, _targetTag, _damage, _hitSpeed, _range, _captureRange);
    }

    [Serializable]
    public class LivingOutAttackBuildingStat : AttackBuildingStat
    {
        public float _lifeTime; // ���� �ð�

        public LivingOutAttackBuildingStat(int id, int level, string name, float hp, OffsetRect fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange, float lifeTime) : base(id, level, name, hp, fillOffset, targetTag, damage, hitSpeed, range, captureRange)
        {
            _lifeTime = lifeTime;
        }

        public override void ResetData(Entity entity)
        => entity.Initialize(_level, _name, _hp, _fillOffset, _targetTag, _damage, _hitSpeed, _range, _captureRange, _lifeTime);
    }

    [Serializable]
    public class LivingOutSpawnBuildingStat : BuildingStat
    {
        public float _lifeTime; // ���� �ð�
        public int _spawnUnitId; // ������ų ������ id
        public float _spawnDelay; // ���� ������
        public int _spawnUnitCount; // ���� ���� ����
        public SerializableVector3[] _spawnOffset; // ���� ������

        public LivingOutSpawnBuildingStat(int id, int level, string name, float hp, OffsetRect fillOffset, float lifeTime, int spawnUnitId, float spawnDelay, SerializableVector3[] spawnOffsets) : base(id, level, name, hp, fillOffset)
        {
            _spawnOffset = new SerializableVector3[_spawnUnitCount];

            _lifeTime = lifeTime;
            _spawnUnitId = spawnUnitId;
            _spawnDelay = spawnDelay;
            _spawnOffset = spawnOffsets;
        }

        public override void ResetData(Entity entity)
        => entity.Initialize(_level, _name, _hp, _fillOffset, _lifeTime, _spawnUnitId, _spawnDelay, _spawnOffset);
    }
}
