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


    // json 데이터를 Stat 형태로 파싱해서 가져오기
    [Serializable]
    public class BaseStat
    {
        public int _id; // Entity 아이디
        public int _level; // Entity 레벨
        public string _name; // 이름

        public BaseStat(int id, int level, string name)
        {
            _id = id;
            _level = level;
            _name = name;
        }

        public virtual void ResetData(Entity entity) { }
    }

    // json 데이터를 Stat 형태로 파싱해서 가져오기
    [Serializable]
    public class MagicStat : BaseStat
    {
        public float _range; // 범위
        public float _durationBeforeDestroy; // Task 적용 후 기간

        public MagicStat(int id, int level, string name, float range, float durationBeforeDestroy) : base(id, level, name)
        {
            _range = range;
            _durationBeforeDestroy = durationBeforeDestroy;
        }
    }

    // json 데이터를 Stat 형태로 파싱해서 가져오기
    [Serializable]
    public class ProjectileMagicStat : MagicStat
    {
        // 기본 능력치
        public float _damage; // 데미지
        public float _speed; // Task 적용 전 딜레이

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
        public float _hp; // 체력

        public LifeStat(int id, int level, string name, float hp) : base(id, level, name)
        {
            _hp = hp;
        }
    }

    [Serializable]
    public class UnitStat : LifeStat
    {
        public float _damage; // 데미지
        public CaptureTag[] _targetTag; // 타켓 태그
        public float _hitSpeed; // 공격 속도
        public float _range; // 범위
        public float _captureRange; // 탐지 범위

        public float _moveSpeed; // 움직임 속도
        public float _rotationSpeed; // 회전 속도

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
        public float _damage; // 데미지
        public CaptureTag[] _targetTag; // 타켓 태그
        public float _hitSpeed; // 공격 속도
        public float _range; // 범위
        public float _captureRange; // 탐지 범위

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
        public float _lifeTime; // 생존 시간

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
        public float _lifeTime; // 생존 시간
        public int _spawnUnitId; // 스폰시킬 유닛의 id
        public float _spawnDelay; // 스폰 딜레이
        public int _spawnUnitCount; // 스폰 유닛 개수
        public SerializableVector3[] _spawnOffset; // 스폰 오프셋

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
