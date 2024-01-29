using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;
using System;

namespace WPP.AI.PROJECTILE
{
    public class SingleAttackProjectile : BaseProjectile
    {
        protected override void DoDamageTask()
        {
            ApplyDamage(_target.ReturnDamagable(), _damage);
        }
    }
}