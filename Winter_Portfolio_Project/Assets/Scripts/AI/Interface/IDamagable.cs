using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public float HP { get; set; }

    public bool IsDie { get; set; }

    public void GetDamage(float damage);

    public void Die();
}
