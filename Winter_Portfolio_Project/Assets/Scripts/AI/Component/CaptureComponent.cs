using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;
using System;

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

        bool CheckContainTag(string tag)
        {
            for (int i = 0; i < _tagsToCapture.Length; i++)
            {
                if (_tagsToCapture[i].ToString() == tag) return true;
            }

            return false;
        }

        public void Initialize(CaptureTag[] tagsToCapture)
        {
            _tagsToCapture = tagsToCapture;
        }

        private void OnTriggerEnter(Collider other)
        {
            ITarget tmpTarget = other.GetComponent<ITarget>();
            if (tmpTarget == null || _targets.Contains(tmpTarget) == true) return;
            // ���� ITarget�� ���ٸ� null�� ������
            // targets ����Ʈ�� tmpTarget�� �����ϸ� ����

            if (CheckContainTag(tmpTarget.ReturnTag()) == false) return;
            // ������ �±׿� ���Ե��� �ʴ� ��� �迭�� ���� ����

            _targets.Add(tmpTarget);
        }

        private void OnTriggerExit(Collider other)
        {
            ITarget tmpTarget = other.GetComponent<ITarget>();
            if (tmpTarget == null || _targets.Contains(tmpTarget) == false) return;

            _targets.Remove(tmpTarget);
        }

        public bool IsContainTarget()
        {
            return _targets.Count != 0;
        }

        // �켱 ���� ����� ����� ������
        public ITarget ReturnTarget()
        {
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