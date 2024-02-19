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
            if (_target == null) return;
            _target.ReturnDamagable().GetDamage(_damage);
        }
    }
}