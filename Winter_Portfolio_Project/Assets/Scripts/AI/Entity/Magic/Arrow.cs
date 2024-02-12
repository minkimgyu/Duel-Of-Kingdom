using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WPP.AI;

namespace WPP
{
    public class Arrow : ProjectileMagic
    {
        // ���������� �� ���� �޾Ƽ� ȿ�� ����
        [SerializeField] Transform _projectileFlyPrefab; // ȭ���� ���󰡴� ȿ��
        [SerializeField] Transform _projectileGroundPrefab; // ���� ȭ���� ���� ȿ��

        Transform _groundTrasform; // ������ _projectileGroundPrefab�� �ӽ� �����صд�

        protected override void DoStartTask()
        {
            // ���⼭ �ʱ� �۾� ����
            // ȭ���� ������ �ð� �ȿ� ��ǥ �������� ���󰡰� ����

            Transform flyingTrasform = Instantiate(_projectileFlyPrefab, _projectileStartPosition, Quaternion.identity);
            flyingTrasform.rotation = Quaternion.LookRotation((transform.position - flyingTrasform.position).normalized);
            // �̷��� ȸ�� ���� ��������ֱ�

            flyingTrasform.DOMove(transform.position, _speed).SetSpeedBased().SetEase(Ease.Linear).onComplete = () => { Destroy(flyingTrasform.gameObject); DoMainTask(); };
        }

        protected override void DoMainTask()
        {
            // ȭ���� ���� ȿ�� ����
            // ������Ʈ ���� �ھƹ�����
            _groundTrasform = Instantiate(_projectileGroundPrefab, transform.position, Quaternion.identity);
            _rangeDamageComponent.ApplyRangeDamage(_damage, _range);
            Invoke("DestroyThis", _durationBeforeDestroy);
        }

        protected override void DestroyThis() { Destroy(gameObject); Destroy(_groundTrasform.gameObject); } // ���� ������Ʈ �ı�
    }
}
