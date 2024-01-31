using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI.TARGET
{
    public interface ITarget
    {
        public int PlayerId { get; set; } // 현재 대상의 소유권을 가지고 있는 플레이어의 id;

        public float ReturnColliderSize(); // 콜라이더의 길이를 반환한다.

        public string ReturnName(); // 오브젝트의 이름을 반환한다. 

        public string ReturnTag(); // 오브젝트의 태그를 반환한다.

        public Vector3 ReturnPosition(); // 타겟의 위치를 반환한다.

        public IDamagable ReturnDamagable(); // 타겟에 데미지를 적용시킬 인터페이스를 반환한다.
    }
}