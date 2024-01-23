using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;
using System;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed = 3;
    ITarget _target;
    Vector3 _storedPosition;
    float _damage;
    Action<IDamagable, float> ApplyDamage;

    public void Initialize(ITarget target, float damage, Action<IDamagable, float> applyDamage)
    {
        _target = target;
        _damage = damage;
        ApplyDamage = applyDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        ITarget target = other.GetComponent<ITarget>();
        if (target == null || _target != target) return;

        ApplyDamage(_target.ReturnDamagable(), _damage);
        Destroy(gameObject);
    }

    void Move(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * _speed);
        if(_target.Equals(null) == true && Vector3.Distance(transform.position, _storedPosition) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_target.Equals(null) == false) _storedPosition = _target.ReturnPosition();
        Move(_storedPosition);
    }
}
