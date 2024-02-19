using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI.TARGET
{
    public interface ITarget
    {
       /// <summary>
       /// ��ü�� ������ �÷��̾��� Id�� ��ȯ�Ѵ�.
       /// </summary>
       /// <returns></returns>
        public int ReturnOwnershipId();

        /// <summary>
        /// �ݶ��̴��� ���̸� ��ȯ�Ѵ�.
        /// </summary>
        public float ReturnColliderSize();

        /// <summary>
        /// ������Ʈ�� �̸��� ��ȯ�Ѵ�. 
        /// </summary>
        public string ReturnName(); 

        /// <summary>
        /// ������Ʈ�� �±׸� ��ȯ�Ѵ�.
        /// </summary>
        public string ReturnTag();

        /// <summary>
        /// Ÿ���� ��ġ�� ��ȯ�Ѵ�.
        /// </summary>
        public Vector3 ReturnPosition();

        /// <summary>
        /// Ÿ�ٿ� �������� �����ų �������̽��� ��ȯ�Ѵ�.
        /// </summary>
        public IDamagable ReturnDamagable();
    }
}