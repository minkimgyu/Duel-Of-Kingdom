using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI.TARGET
{
    public interface ITarget
    {
        public int PlayerId { get; set; } // ���� ����� �������� ������ �ִ� �÷��̾��� id;

        public float ReturnColliderSize(); // �ݶ��̴��� ���̸� ��ȯ�Ѵ�.

        public string ReturnName(); // ������Ʈ�� �̸��� ��ȯ�Ѵ�. 

        public string ReturnTag(); // ������Ʈ�� �±׸� ��ȯ�Ѵ�.

        public Vector3 ReturnPosition(); // Ÿ���� ��ġ�� ��ȯ�Ѵ�.

        public IDamagable ReturnDamagable(); // Ÿ�ٿ� �������� �����ų �������̽��� ��ȯ�Ѵ�.
    }
}