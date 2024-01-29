using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.CAPTURE;
using WPP.AI.UNIT;
using System;

namespace WPP.AI.STAT
{
    // json 데이터를 Stat 형태로 파싱해서 가져오기
    [Serializable]
    abstract public class BaseStat
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

        public abstract void Initialize(Entity entity);
    }

    [Serializable]
    public class UnitStat : BaseStat
    {
        public float _damage; // 데미지
        public CaptureTag[] _targetTag; // 타켓 태그
        public float _hitSpeed; // 공격 속도
        public float _range; // 범위

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
        public float _damage; // 데미지
        public CaptureTag[] _targetTag; // 타켓 태그
        public float _hitSpeed; // 공격 속도
        public float _range; // 범위

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
        public float _lifeTime; // 생존 시간

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
        public float _lifeTime; // 생존 시간
        public int _spawnUnitId; // 스폰시킬 유닛의 id
        public int _spawnUnitCount; // 스폰시킬 유닛의 개수

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
