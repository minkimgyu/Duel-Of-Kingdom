using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AttackComponent : MonoBehaviour
{
    // ���� �ĵ� ������Ѻ�����
    protected Timer _preDelayTimer = new Timer();
    public Timer PreDelayTimer { get { return _preDelayTimer; } }

    protected Timer _afterDelayTimer = new Timer();

    protected bool _isFinish = false;
    protected bool _cancelAttack = false;

    protected Animator _animator;

    protected void CancelAttackAnimation() => _animator.SetTrigger("CancelAttack");

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void CancelAttack() { }
    public virtual void CatchCancel() { }

    public void StartAfterDelay(float attackDelay)
    {
        if (_isFinish == false) return;

        _afterDelayTimer.Start(attackDelay);
        _isFinish = false;
    }

    public void ResetAfterDelayTimer()
    {
        if(_afterDelayTimer.IsFinish) _afterDelayTimer.Reset();
    }

    // ���ο��� getcomponent ����ؼ� ���� ����
    public virtual void Attack(Transform target, float damage) { }
    public virtual void Attack(GameObject target, float damage) { }
}

public class MeleeAttackComponent : AttackComponent
{
    [SerializeField] private float _maxDistance = 20.0f;
    [SerializeField] Transform rayStartPoint;

    public override void CancelAttack() 
    {
        _cancelAttack = true; // ���� ������ֱ�
        CancelAttackAnimation(); // �ִϸ��̼� ĵ�����ֱ�
    }

    public override void CatchCancel()
    {
        if (_cancelAttack == false) return;

        _cancelAttack = false; // ���� ������ֱ�
        _afterDelayTimer.Reset(); // Ÿ�̸� ���� �� Success ����
    }

    public override void Attack(GameObject target, float damage)
    {
        // �����ɽ�Ʈ ����� �ƴ϶� ������Ʈ�� ���� �������� ������ ����� ä���غ���
        Debug.Log("Attack");
        _animator.SetTrigger("NowAttack");
        _isFinish = true;
    }
}
