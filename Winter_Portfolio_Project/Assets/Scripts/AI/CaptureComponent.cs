using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;

public class CaptureComponent : MonoBehaviour
{
    // �����ϴ� ����� ��ġ�� ���� �Ÿ� �̻� �������ٸ� �ٽ� PathFinding�ϵ��� �����ϱ� 

    [SerializeField] List<ITarget> targets = new List<ITarget>();

    private void OnTriggerEnter(Collider other)
    {
        ITarget tmpTarget = other.GetComponent<ITarget>();
        if (tmpTarget == null || targets.Contains(tmpTarget) == true) return;
        // ���� ITarget�� ���ٸ� null�� ������
        // targets ����Ʈ�� tmpTarget�� �����ϸ� ����

        targets.Add(tmpTarget);
    }

    private void OnTriggerExit(Collider other)
    {
        ITarget tmpTarget = other.GetComponent<ITarget>();
        if (tmpTarget == null || targets.Contains(tmpTarget) == false) return;

        targets.Remove(tmpTarget);
    }

    public bool IsContainTarget()
    {
        return targets.Count != 0;
    }

    // �켱 ���� ����� ����� ������
    public ITarget ReturnTarget()
    {
        float distanceBetween = 0;
        int indexOfTarget = -1;

        for (int i = 0; i < targets.Count; i++)
        {
            if(targets[i].Equals(null))
            {
                // ���⼭ �������� �������� �ʴ´ٸ� �����ֱ�
                targets.RemoveAt(i);
                int lastIndex = targets.Count - 1;

                if(i > lastIndex) break; //  // ������ �ε����� ���ؼ� �� ū ���
                else continue;
            }

            if(i == 0)
            {
                distanceBetween = Vector3.Distance(transform.position, targets[i].ReturnPosition());
                indexOfTarget = 0; // ������Ʈ�� ���� ��� 0���� �ʱ�ȭ ����
            }
            else
            {
                float tmpDistance = Vector3.Distance(transform.position, targets[i].ReturnPosition());
                if (tmpDistance >= distanceBetween) continue;

                indexOfTarget = i;
                distanceBetween = tmpDistance;
            }
        }

        if (indexOfTarget == -1) return null;
        else return targets[indexOfTarget];
    }
}
