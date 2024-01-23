using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI.TARGET
{
    public interface ITarget
    {
        public float ReturnColliderLength(); // 콜라이더의 길이를 반환한다.

        public Vector3 ReturnPosition(); // 타겟의 위치를 반환한다.

        public IDamagable ReturnDamagable(); // 타겟에 데미지를 적용시킬 인터페이스를 반환한다.
    }
}