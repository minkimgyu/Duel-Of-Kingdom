using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.DRAWING;
using WPP.AI.TARGET;
using System;

namespace WPP.AI.ATTACK
{
    public class RangeDamageComponent : MonoBehaviour
    {
        [SerializeField] AreaDrawer _drawingPrefab;
        [SerializeField] float _damageAreaDestroyDuration;
        float _drawingYPos = 1.3f;

        void DrawRange(float radius)
        {
            AreaDrawer drawer = Instantiate(_drawingPrefab, new Vector3(transform.position.x, _drawingYPos, transform.position.z), Quaternion.identity);
            drawer.Initialize(radius, _damageAreaDestroyDuration);
            drawer.Draw();
        }

        Collider[] ReturnColliders(float radius)
        {
            // 일정 범위를 탐지해서 그 안에 있는 모든 적에게 데미지를 입힌다.
            int layer = LayerMask.GetMask("GroundEntity", "AirEntity");
            return Physics.OverlapSphere(transform.position, radius, layer);
        }

        public void ApplyRangeDamage(float damage, float radius, float ownershipId)
        {
            DrawRange(radius);
            Collider[] hitColliders = ReturnColliders(radius);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                ITarget target = hitColliders[i].GetComponent<ITarget>();
                if (target == null) continue;

                int targetId = target.ReturnOwnershipId();
                if (ownershipId == targetId) continue; // ownershipId != targetId 이면 공격 진행

                target.ReturnDamagable().GetDamage(damage);
            }
        }
    }
}
