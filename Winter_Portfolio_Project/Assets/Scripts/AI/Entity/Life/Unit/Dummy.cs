using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.AI
{
    public class Dummy : Life
    {
        protected CapsuleCollider _capsuleCollider;

        void Start()
        {
            InitializeComponent();
            _capsuleCollider = GetComponent<CapsuleCollider>();
        }

        public override float ReturnColliderSize()
        {
            return _capsuleCollider.radius * (transform.localScale.x + transform.localScale.z) / 2; // �̰Ŵ� ĸ���ݶ��̴��� �����
        }
    }
}
