using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    Rigidbody _rigid;
    Animator _animator;
    [SerializeField] float _moveSpeed = 5f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
    }

    public void Stop()
    {
        _rigid.MovePosition(transform.position);
        _animator.SetBool("IsMove", false);
    }

    //public void Stop(Vector3 targetPos)
    //{
    //    _rigid.MovePosition(transform.position);
    //    _animator.SetBool("IsMove", false);

    //    Vector3 dir = ReturnDirection(targetPos);
    //    View(dir);
    //}

    Vector3 ReturnDirection(Vector3 targetPos)
    {
        return (targetPos - transform.position).normalized;
    }

    public void Move(Vector3 targetPos)
    {
        Vector3 dir = ReturnDirection(targetPos);
        _rigid.MovePosition(transform.position + dir * _moveSpeed * Time.deltaTime);

        _animator.SetBool("IsMove", true);
    }
}
