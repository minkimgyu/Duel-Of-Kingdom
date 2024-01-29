using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;

namespace WPP.AI.PROJECTILE
{
    public class RangeAttackProjectile : BaseProjectile
    {
        [SerializeField] float _radius;
        [SerializeField] DrawingCircle _drawingPrefab;
        float _drawingYPos = 1.3f;

        protected override void DoDamageTask()
        {
            // ���� ������ Ž���ؼ� �� �ȿ� �ִ� ��� ������ �������� ������.
            int layer = LayerMask.GetMask("GroundEntity", "AirEntity");

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius, layer);
            DrawingCircle circle = Instantiate(_drawingPrefab, new Vector3(transform.position.x, _drawingYPos, transform.position.z), Quaternion.identity);
            circle.Initialize(_radius);


            for (int i = 0; i < hitColliders.Length; i++)
            {
                ITarget target = hitColliders[i].GetComponent<ITarget>();
                if (target == null) continue;

                ApplyDamage(target.ReturnDamagable(), _damage);
            }
        }
    }
}
