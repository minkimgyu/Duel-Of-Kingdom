using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AttackComponent : MonoBehaviour
{
    // 선딜 후딜 적용시켜봐야함
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

    // 내부에서 getcomponent 사용해서 공격 적용
    public virtual void Attack(Transform target, float damage) { }
    public virtual void Attack(GameObject target, float damage) { }
}

public class MeleeAttackComponent : AttackComponent
{
    [SerializeField] private float _maxDistance = 20.0f;
    [SerializeField] Transform rayStartPoint;

    public override void CancelAttack() 
    {
        _cancelAttack = true; // 공격 취소해주기
        CancelAttackAnimation(); // 애니메이션 캔슬해주기
    }

    public override void CatchCancel()
    {
        if (_cancelAttack == false) return;

        _cancelAttack = false; // 공격 취소해주기
        _afterDelayTimer.Reset(); // 타이머 리셋 후 Success 리턴
    }

    public override void Attack(GameObject target, float damage)
    {
        // 레이케스트 방식이 아니라 오브젝트에 직접 데미지를 입히는 방식을 채용해보자
        Debug.Log("Attack");
        _animator.SetTrigger("NowAttack");
        _isFinish = true;
    }
}
