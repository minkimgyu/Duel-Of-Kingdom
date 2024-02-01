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


    // json 데이터를 Stat 형태로 파싱해서 가져오기
    [Serializable]
    public class BaseStat
    {
        public int _id; // Entity 아이디
        public int _level; // Entity 레벨
        public string _name; // 이름
        public float _hp; // 체력

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
        public float _damage; // 데미지
        public CaptureTag[] _targetTag; // 타켓 태그
        public float _hitSpeed; // 공격 속도
        public float _range; // 범위
        public float _captureRange; // 탐지 범위

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
        public float _damage; // 데미지
        public CaptureTag[] _targetTag; // 타켓 태그
        public float _hitSpeed; // 공격 속도
        public float _range; // 범위
        public float _captureRange; // 탐지 범위

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
        public float _lifeTime; // 생존 시간

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
        public float _lifeTime; // 생존 시간
        public int _spawnUnitId; // 스폰시킬 유닛의 id
        public float _spawnDelay; // 스폰 딜레이
        public SerializableVector3[] _spawnOffset; // 스폰 오프셋

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
