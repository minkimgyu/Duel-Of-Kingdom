using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WPP.AI;

namespace WPP
{
    public class Arrow : ProjectileMagic
    {
        // 프리팹으로 두 개를 받아서 효과 적용
        [SerializeField] Transform _projectileFlyPrefab; // 화살이 날라가는 효과
        [SerializeField] Transform _projectileGroundPrefab; // 땅에 화살이 박힌 효과

        Transform _groundTrasform; // 스폰된 _projectileGroundPrefab를 임시 저장해둔다

        protected override void DoStartTask()
        {
            // 여기서 초기 작업 진행
            // 화살이 딜레이 시간 안에 목표 지점까지 날라가게 지정

            Transform flyingTrasform = Instantiate(_projectileFlyPrefab, _projectileStartPosition, Quaternion.identity);
            flyingTrasform.rotation = Quaternion.LookRotation((transform.position - flyingTrasform.position).normalized);
            // 이렇게 회전 값을 적용시켜주기

            flyingTrasform.DOMove(transform.position, _speed).SetSpeedBased().SetEase(Ease.Linear).onComplete = () => { Destroy(flyingTrasform.gameObject); DoMainTask(); };
        }

        protected override void DoMainTask()
        {
            // 화살이 박힌 효과 구현
            // 오브젝트 땅에 박아버리기
            _groundTrasform = Instantiate(_projectileGroundPrefab, transform.position, Quaternion.identity);
            _rangeDamageComponent.ApplyRangeDamage(_damage, _range);
            Invoke("DestroyThis", _durationBeforeDestroy);
        }

        protected override void DestroyThis() { Destroy(gameObject); Destroy(_groundTrasform.gameObject); } // 게임 오브젝트 파괴
    }
}
