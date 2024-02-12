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
        // 추적하는 대상의 위치가 일정 거리 이상 벌어진다면 다시 PathFinding하도록 지정하기 

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

        // 플레이어 id를 비교해서 상대방의 타워로 인식되는 오브젝트를 리스트 안에 추가함
        void AddTowers()
        {
            // 이런식으로 찾아서 _captureComponent에 집어넣어줌
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
            // 만약 ITarget이 없다면 null을 리턴함
            // targets 리스트가 tmpTarget를 포함하면 리턴

            if (CheckContainTag(tmpTarget.ReturnTag()) == false) return;
            // 포착할 태그에 포함되지 않는 경우 배열에 넣지 않음

            _targets.Add(tmpTarget);
        }

        private void OnTriggerExit(Collider other)
        {
            ITarget tmpTarget = other.GetComponent<ITarget>();
            if (tmpTarget == null || _targets.Contains(tmpTarget) == false || IsTower(tmpTarget)) return;

            _targets.Remove(tmpTarget);
        }

        // 오브젝트 이름에 타워가 들어가면 타겟을 해제하지 않음
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

        // 우선 가장 가까운 대상을 리턴함
        public ITarget ReturnTarget()
        {
            // _storedTarget이 null이 아닌 경우 해당 타겟을 리턴해준다.
            if (_storedTarget != null) return _storedTarget;

            float distanceBetween = 0;
            int indexOfTarget = -1;

            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i].Equals(null))
                {
                    // 여기서 아이템이 존재하지 않는다면 지워주기
                    _targets.RemoveAt(i);
                    int lastIndex = _targets.Count - 1;

                    if (i > lastIndex) break; //  // 마지막 인덱스에 비교해서 더 큰 경우
                    else continue;
                }

                if (_targets[i].ReturnOwnershipId() == _ownershipId) continue; // 대상과 자신의 플레이어 아이디가 같은 경우 타겟으로 삼지 않음

                if (i == 0)
                {
                    distanceBetween = Vector3.Distance(transform.position, _targets[i].ReturnPosition());
                    indexOfTarget = 0; // 오브젝트가 있을 경우 0으로 초기화 해줌
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