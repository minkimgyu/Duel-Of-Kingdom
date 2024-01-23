using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.TARGET;

public class CaptureComponent : MonoBehaviour
{
    // 추적하는 대상의 위치가 일정 거리 이상 벌어진다면 다시 PathFinding하도록 지정하기 

    [SerializeField] List<ITarget> targets = new List<ITarget>();

    private void OnTriggerEnter(Collider other)
    {
        ITarget tmpTarget = other.GetComponent<ITarget>();
        if (tmpTarget == null || targets.Contains(tmpTarget) == true) return;
        // 만약 ITarget이 없다면 null을 리턴함
        // targets 리스트가 tmpTarget를 포함하면 리턴

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

    // 우선 가장 가까운 대상을 리턴함
    public ITarget ReturnTarget()
    {
        float distanceBetween = 0;
        int indexOfTarget = -1;

        for (int i = 0; i < targets.Count; i++)
        {
            if(targets[i].Equals(null))
            {
                // 여기서 아이템이 존재하지 않는다면 지워주기
                targets.RemoveAt(i);
                int lastIndex = targets.Count - 1;

                if(i > lastIndex) break; //  // 마지막 인덱스에 비교해서 더 큰 경우
                else continue;
            }

            if(i == 0)
            {
                distanceBetween = Vector3.Distance(transform.position, targets[i].ReturnPosition());
                indexOfTarget = 0; // 오브젝트가 있을 경우 0으로 초기화 해줌
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
