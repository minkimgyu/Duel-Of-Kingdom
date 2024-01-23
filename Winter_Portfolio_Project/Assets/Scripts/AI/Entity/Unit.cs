using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI.TARGET;
using Tree = BehaviorTree.Tree;

namespace WPP.AI.UNIT
{
    abstract public class GroundUnit : Unit
    {
        protected override void Start()
        {
            base.Start();
            _type = Type.Ground;
        }
    }

    abstract public class Unit : EntityAI
    {
        // �⺻���� ��ġ�� ����� ���⼭ ����
        // �̵����� ����

        public enum Type
        {
            Air,
            Ground
        }

        protected Type _type;

        protected MoveComponent _moveComponent;
        protected ViewComponent _viewComponent;
        protected CaptureComponent _captureComponent; // ���� ����� ��ž�̶� ���� Ŭ������ ������ ����

        protected CapsuleCollider _capsuleCollider;

        protected List<Vector3> _posListForDrawingGizmo = new List<Vector3>(); // �̰Ŵ� BT���� �������ֱ�

        // Start is called before the first frame update
        protected override void Start()
        {
            _moveComponent = GetComponent<MoveComponent>();
            _viewComponent = GetComponent<ViewComponent>();

            _captureComponent = GetComponentInChildren<CaptureComponent>();

            _capsuleCollider = GetComponent<CapsuleCollider>();
            base.Start();
        }

        protected void ResetPosListForDrawingGizmo(List<Vector3> posList) => _posListForDrawingGizmo = posList;

        protected bool IsPosListEmpty() { return _posListForDrawingGizmo.Count == 0; }

        void OnDrawGizmos() // �̰͵� �̺�Ʈ�� �Ѱ�����
        {
            // ���� �ֱ� ������ ���� ������
            if (_posListForDrawingGizmo.Count == 0) return;

            Gizmos.color = Color.blue;

            for (int i = 0; i < _posListForDrawingGizmo.Count - 1; i++)
                Gizmos.DrawLine(
                    new Vector3(_posListForDrawingGizmo[i].x, 1.5f, _posListForDrawingGizmo[i].z),
                    new Vector3(_posListForDrawingGizmo[i + 1].x, 1.5f, _posListForDrawingGizmo[i + 1].z));
        }

        public override float ReturnColliderLength()
        {
            return _capsuleCollider.radius; // �̰Ŵ� ĸ���ݶ��̴��� �����
        }
    }
}