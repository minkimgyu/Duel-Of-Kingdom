using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI.TARGET
{
    public interface ITarget
    {
       /// <summary>
       /// 객체를 소유한 플레이어의 Id를 반환한다.
       /// </summary>
       /// <returns></returns>
        public int ReturnOwnershipId();

        /// <summary>
        /// 콜라이더의 길이를 반환한다.
        /// </summary>
        public float ReturnColliderSize();

        /// <summary>
        /// 오브젝트의 이름을 반환한다. 
        /// </summary>
        public string ReturnName(); 

        /// <summary>
        /// 오브젝트의 태그를 반환한다.
        /// </summary>
        public string ReturnTag();

        /// <summary>
        /// 타겟의 위치를 반환한다.
        /// </summary>
        public Vector3 ReturnPosition();

        /// <summary>
        /// 타겟에 데미지를 적용시킬 인터페이스를 반환한다.
        /// </summary>
        public IDamagable ReturnDamagable();
    }
}