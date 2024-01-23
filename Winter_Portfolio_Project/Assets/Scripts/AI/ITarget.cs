using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI.TARGET
{
    public interface ITarget
    {
        public float ReturnColliderLength(); // �ݶ��̴��� ���̸� ��ȯ�Ѵ�.

        public Vector3 ReturnPosition(); // Ÿ���� ��ġ�� ��ȯ�Ѵ�.

        public IDamagable ReturnDamagable(); // Ÿ�ٿ� �������� �����ų �������̽��� ��ȯ�Ѵ�.
    }
}