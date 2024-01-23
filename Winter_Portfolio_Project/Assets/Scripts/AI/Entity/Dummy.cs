using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI
{
    public class Dummy : Entity
    {
        protected CapsuleCollider _capsuleCollider;

        protected override void Start()
        {
            base.Start();
            _capsuleCollider = GetComponent<CapsuleCollider>();
        }

        public override float ReturnColliderLength()
        {
            return _capsuleCollider.radius; // 이거는 캡슐콜라이더를 사용함
        }
    }
}
