using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;
using WPP.AI.ATTACK;
using System;

namespace WPP.AI.PROJECTILE
{
    public class RangeAttackProjectile : BaseProjectile
    {
        [SerializeField] float _radius;
        [SerializeField] DrawingCircle _drawingPrefab;

        RangeDamageComponent _rangeDamageComponent;

        public override void Initialize(ITarget target, float damage)
        {
            base.Initialize(target, damage);
            _rangeDamageComponent = GetComponent<RangeDamageComponent>();
        }

        protected override void DoDamageTask()
        {
            _rangeDamageComponent.ApplyRangeDamage(_damage, _radius);
        }
    }
}
