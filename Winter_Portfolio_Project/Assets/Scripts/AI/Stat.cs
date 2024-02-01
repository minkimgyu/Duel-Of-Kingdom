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


    // json �����͸� Stat ���·� �Ľ��ؼ� ��������
    [Serializable]
    public class BaseStat
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

        public virtual void Initialize(Entity entity) { }
    }

    [Serializable]
    public class UnitStat : BaseStat
    {
        public float _damage; // ������
        public CaptureTag[] _targetTag; // Ÿ�� �±�
        public float _hitSpeed; // ���� �ӵ�
        public float _range; // ����
        public float _captureRange; // Ž�� ����

        public UnitStat(int id, int level, string name, float hp, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange) : base(id, level, name, hp)
        {
            _damage = damage;
            _targetTag = targetTag;
            _hitSpeed = hitSpeed;
            _range = range;
            _captureRange = captureRange;
        }

        public override void Initialize(Entity entity)
        => entity.Initialize(_id, _level, _name, _hp, _targetTag, _damage, _hitSpeed, _range, _captureRange);
    }

    [Serializable]
    public class BuildingStat : BaseStat
    {
        public OffsetFromCenter _filltOffset;

        public BuildingStat(int id, int level, string name, float hp, OffsetFromCenter fillOffset) : base(id, level, name, hp)
        {
            _filltOffset = fillOffset;
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

        public AttackBuildingStat(int id, int level, string name, float hp, OffsetFromCenter fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange) : base(id, level, name, hp, fillOffset)
        {
            _damage = damage;
            _targetTag = targetTag;
            _hitSpeed = hitSpeed;
            _range = range;
            _captureRange = captureRange;
        }

        public override void Initialize(Entity entity)
        => entity.Initialize(_id, _level, _name, _hp, _filltOffset, _targetTag, _damage, _hitSpeed, _range, _captureRange);
    }

    [Serializable]
    public class LivingOutAttackBuildingStat : AttackBuildingStat
    {
        public float _lifeTime; // ���� �ð�

        public LivingOutAttackBuildingStat(int id, int level, string name, float hp, OffsetFromCenter fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange, float lifeTime) : base(id, level, name, hp, fillOffset, targetTag, damage, hitSpeed, range, captureRange)
        {
            _lifeTime = lifeTime;
        }

        public override void Initialize(Entity entity)
        => entity.Initialize(_id, _level, _name, _hp, _filltOffset, _targetTag, _damage, _hitSpeed, _range, _captureRange, _lifeTime);
    }

    [Serializable]
    public class LivingOutSpawnBuildingStat : BuildingStat
    {
        public float _lifeTime; // ���� �ð�
        public int _spawnUnitId; // ������ų ������ id
        public float _spawnDelay; // ���� ������
        public SerializableVector3[] _spawnOffset; // ���� ������

        public LivingOutSpawnBuildingStat(int id, int level, string name, float hp, OffsetFromCenter fillOffset, float lifeTime, int spawnUnitId, float spawnDelay, SerializableVector3[] spawnOffsets) : base(id, level, name, hp, fillOffset)
        {
            _lifeTime = lifeTime;
            _spawnUnitId = spawnUnitId;
            _spawnDelay = spawnDelay;
            _spawnOffset = spawnOffsets;
        }

        public override void Initialize(Entity entity)
        => entity.Initialize(_id, _level, _name, _hp, _filltOffset, _lifeTime, _spawnUnitId, _spawnDelay, _spawnOffset);
    }
}
