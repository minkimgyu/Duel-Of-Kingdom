using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AttackComponent : MonoBehaviour
{
    protected bool _isFinish = false;
    public bool IsFinish { get { return _isFinish; } set { _isFinish = value; } }

    protected Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void Attack(float damage) { }
    public virtual void Attack(Vector3 targetDir, float damage) { }
}

public class MeleeAttackComponent : AttackComponent
{
    [SerializeField] private float _maxDistance = 20.0f;
    [SerializeField] Transform rayStartPoint;

    public override void Attack(Vector3 targetDir, float damage)
    {
        // �����ɽ�Ʈ ����� �ƴ϶� ������Ʈ�� ���� �������� ������ ����� ä���غ���

        Debug.Log("Attack");
        _animator.SetTrigger("NowAttack");
        _isFinish = true;

        Physics.Raycast(rayStartPoint.position, targetDir, out RaycastHit hit, _maxDistance);

        if (hit.collider == null) return;
        Debug.DrawRay(transform.position, targetDir * hit.distance, Color.red, 3f);
    }
}
