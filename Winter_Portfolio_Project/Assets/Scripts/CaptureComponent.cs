using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureComponent : MonoBehaviour
{
    // 추적하는 대상의 위치가 일정 거리 이상 벌어진다면 다시 PathFinding하도록 지정하기 

    [SerializeField] List<Transform> targets = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (targets.Contains(other.transform) == true) return;

        targets.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (targets.Contains(other.transform) == false) return;

        targets.Remove(other.transform);
    }

    public bool IsContainTarget()
    {
        return targets.Count != 0;
    }

    // 우선 가장 가까운 대상을 리턴함
    public Transform ReturnTarget()
    {
        float distanceBetween = 0;
        int indexOfTarget = -1;

        if (targets.Count == 1) return targets[0];

        for (int i = 0; i < targets.Count; i++)
        {
            if(i == 0) distanceBetween = Vector3.Distance(transform.position, targets[i].position);
            else
            {
                float tmpDistance = Vector3.Distance(transform.position, targets[i].position);
                if (tmpDistance >= distanceBetween) continue;

                indexOfTarget = i;
                distanceBetween = tmpDistance;
            }

        }

        if (indexOfTarget == -1) return null;
        else return targets[indexOfTarget];
    }
}
