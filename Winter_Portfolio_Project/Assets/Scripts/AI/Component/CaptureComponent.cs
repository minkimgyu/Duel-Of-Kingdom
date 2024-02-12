using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;
using System;
using WPP.AI.BUILDING;

namespace WPP.AI.CAPTURE
{
    [Serializable]
    public enum CaptureTag
    {
        GroundUnit,
        AirUnit,
        Building
    }

    public class CaptureComponent : MonoBehaviour
    {
        // �����ϴ� ����� ��ġ�� ���� �Ÿ� �̻� �������ٸ� �ٽ� PathFinding�ϵ��� �����ϱ� 

        List<ITarget> _targets = new List<ITarget>();
        CaptureTag[] _tagsToCapture;
        float _ownershipId;

        ITarget _storedTarget = null;

        bool CheckContainTag(string tag)
        {
            for (int i = 0; i < _tagsToCapture.Length; i++)
            {
                if (_tagsToCapture[i].ToString() == tag) return true;
            }

            return false;
        }

        // �÷��̾� id�� ���ؼ� ������ Ÿ���� �νĵǴ� ������Ʈ�� ����Ʈ �ȿ� �߰���
        void AddTowers()
        {
            // �̷������� ã�Ƽ� _captureComponent�� ����־���
            PrincessTower[] princessTowers = FindObjectsOfType<PrincessTower>();
            KingTower[] kingTowers = FindObjectsOfType<KingTower>();

            for (int i = 0; i < princessTowers.Length; i++)
                if(princessTowers[i].OwnershipId != _ownershipId) _targets.Add(princessTowers[i]);

            for (int i = 0; i < kingTowers.Length; i++)
                if (kingTowers[i].OwnershipId != _ownershipId) _targets.Add(kingTowers[i]);
        }

        public void Initialize(CaptureTag[] tagsToCapture, int ownershipId, float range)
        {
            _tagsToCapture = tagsToCapture;
            _ownershipId = ownershipId;

            AddTowers();

            CapsuleCollider collider = GetComponent<CapsuleCollider>();
            if (collider == null) return;

            collider.radius = range;
        }

        private void OnTriggerEnter(Collider other)
        {
            ITarget tmpTarget = other.GetComponent<ITarget>();
            if (tmpTarget == null || _targets.Contains(tmpTarget) == true || IsTower(tmpTarget)) return;
            // ���� ITarget�� ���ٸ� null�� ������
            // targets ����Ʈ�� tmpTarget�� �����ϸ� ����

            if (CheckContainTag(tmpTarget.ReturnTag()) == false) return;
            // ������ �±׿� ���Ե��� �ʴ� ��� �迭�� ���� ����

            _targets.Add(tmpTarget);
        }

        private void OnTriggerExit(Collider other)
        {
            ITarget tmpTarget = other.GetComponent<ITarget>();
            if (tmpTarget == null || _targets.Contains(tmpTarget) == false || IsTower(tmpTarget)) return;

            _targets.Remove(tmpTarget);
        }

        // ������Ʈ �̸��� Ÿ���� ���� Ÿ���� �������� ����
        public bool IsTower(ITarget target)
        {
            return target.ReturnName().Contains("tower");
        }

        public bool IsContainTarget()
        {
            return _targets.Count != 0;
        }

        public void FixTarget(ITarget target)
        {
            if (IsTower(target) == false) return;
            _storedTarget = target;
        }

        // �켱 ���� ����� ����� ������
        public ITarget ReturnTarget()
        {
            // _storedTarget�� null�� �ƴ� ��� �ش� Ÿ���� �������ش�.
            if (_storedTarget != null) return _storedTarget;

            float distanceBetween = 0;
            int indexOfTarget = -1;

            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i].Equals(null))
                {
                    // ���⼭ �������� �������� �ʴ´ٸ� �����ֱ�
                    _targets.RemoveAt(i);
                    int lastIndex = _targets.Count - 1;

                    if (i > lastIndex) break; //  // ������ �ε����� ���ؼ� �� ū ���
                    else continue;
                }

                if (_targets[i].ReturnOwnershipId() == _ownershipId) continue; // ���� �ڽ��� �÷��̾� ���̵� ���� ��� Ÿ������ ���� ����

                if (i == 0)
                {
                    distanceBetween = Vector3.Distance(transform.position, _targets[i].ReturnPosition());
                    indexOfTarget = 0; // ������Ʈ�� ���� ��� 0���� �ʱ�ȭ ����
                }
                else
                {
                    float tmpDistance = Vector3.Distance(transform.position, _targets[i].ReturnPosition());
                    if (tmpDistance >= distanceBetween) continue;

                    indexOfTarget = i;
                    distanceBetween = tmpDistance;
                }
            }

            if (indexOfTarget == -1) return null;
            else return _targets[indexOfTarget];
        }
    }
}