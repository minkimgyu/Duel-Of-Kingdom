using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour, IDamagable
{
    [SerializeField] HpContainer _hpContainer;
    [SerializeField] float _maxHp;
    public float HP { get; set; }
    public bool IsDie { get; set; }

    Action<float> OnHpChange;

    private void Start()
    {
        _hpContainer = GetComponentInChildren<HpContainer>();
        OnHpChange = _hpContainer.OnHpChangeRequested;
    }

    public virtual void GetDamage(float damage)
    {
        HP -= damage;
        OnHpChange?.Invoke(HP/_maxHp);

        if (HP < 0)
        {
            HP = 0;
            Die();
        }
    }

    public virtual void Die()
    {
        if (IsDie == true) return;
    }
}
