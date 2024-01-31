using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI.TARGET;
using WPP.AI.ATTACK;
using WPP.AI.CAPTURE;
using WPP.AI.TIMER;
using WPP.AI.SPAWNER;
using System;
using WPP.GRID;

namespace WPP.AI.BTUtility
{
    public class Attack : Node // 좀 멀면 FindPath, 가까우면 GoDirectToPoint로 노드를 2개로 구분해주기
    {
        AttackComponent _attackComponent;
        CaptureComponent _captureComponent;

        public Attack(AttackComponent attackComponent, CaptureComponent captureComponent)
        {
            _attackComponent = attackComponent;
            _captureComponent = captureComponent;
        }

        public override NodeState Evaluate()
        {
            // 타겟 찾아서 공격
            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return NodeState.FAILURE;

            _attackComponent.Attack(target);
            return NodeState.SUCCESS;
        }
    }

    public class CanFindTarget : Node // 좀 멀면 FindPath, 가까우면 GoDirectToPoint로 노드를 2개로 구분해주기
    {
        CaptureComponent _captureComponent;

        public CanFindTarget(CaptureComponent captureComponent)
        {
            _captureComponent = captureComponent;
        }

        public override NodeState Evaluate()
        {
            bool containTarget = _captureComponent.IsContainTarget();

            if (containTarget == true) return NodeState.SUCCESS;
            return NodeState.FAILURE;
        }
    }

    abstract public class CheckDistance : Node // 좀 멀면 FindPath, 가까우면 GoDirectToPoint로 노드를 2개로 구분해주기
    {
        float _offsetDistance;
        protected float _nearDistance;

        bool _useOffset;
        float _myColliderOffset;

        float Distance { get { return _nearDistance + _offsetDistance; } }

        CaptureComponent _captureComponent;

        public CheckDistance(CaptureComponent captureComponent, float nearDistance, float offsetDistance = 0, bool useOffset = false, float myColliderOffset = 0)
        {
            _captureComponent = captureComponent;
            _nearDistance = nearDistance;
            _offsetDistance = offsetDistance;

            _useOffset = useOffset;
            _myColliderOffset = myColliderOffset;
        }

        protected bool NowEnoughClose(bool useOffset = false)
        {
            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return false;

            Vector3 nowTargetPosition = target.ReturnPosition();
            float distance = Vector3.Distance(_captureComponent.transform.position, nowTargetPosition);
            if (_useOffset == true) distance -= _myColliderOffset + target.ReturnColliderSize();

            if (useOffset == true)
            {
                if (distance <= Distance) return true;
                return false;
            }
            else
            {
                if (distance <= _nearDistance) return true;
                return false;
            }
        }
    }

    /// <summary>
    /// 가까워지면 SUCCESS를 반환해줌. 한번 SUCCESS를 반환해줬다면 FAILURE을 반환하는 거리가 길어짐
    /// </summary>
    public class CheckIsNearAndCancelAttackWhenExit : CheckDistance
    {
        bool _isNearBefore = false; // 이전에 한번 가까워졌던 경우
        AttackComponent _attackComponent;

        public CheckIsNearAndCancelAttackWhenExit(CaptureComponent captureComponent, float nearDistance, float offsetDistance, AttackComponent attackComponent, bool useOffset = false, float myColliderOffset = 0) : base(captureComponent, nearDistance, offsetDistance, useOffset, myColliderOffset)
        {
            _attackComponent = attackComponent;
        }

        public override NodeState Evaluate()
        {
            bool isClose = false;

            // 이전에 실행된 경우는 다음과 같이 Exit 거리를 길게 잡아준다.
            if (_isNearBefore == true) isClose = NowEnoughClose(true);
            else isClose = NowEnoughClose();

            if (isClose == true)
            {
                if (_isNearBefore == false) _isNearBefore = true;
                return NodeState.SUCCESS;
            }

            if (_isNearBefore == true && isClose == false)
            {
                if(_attackComponent.Fix) // 현재 이미 공격이 나간 경우 고정시켜줌
                {
                    return NodeState.RUNNING;
                }
                else
                {
                    // 공격 캔슬시켜주기
                    _attackComponent.CancelAttack();
                    _isNearBefore = false; // 다시 적용시켜주기
                }
            }

            return NodeState.FAILURE;
        }
    }

    public class CheckIsNear : CheckDistance
    {
        public CheckIsNear(CaptureComponent captureComponent, float nearDistance) : base(captureComponent, nearDistance) { }

        public override NodeState Evaluate()
        {
            if (NowEnoughClose()) return NodeState.SUCCESS;
            return NodeState.FAILURE;
        }
    }

    public class FollowPath : Node // 좀 멀면 FindPath, 가까우면 GoDirectToPoint로 노드를 2개로 구분해주기
    {
        CaptureComponent _captureComponent;
        PathFinder _pathFinder;
        MoveComponent _moveComponent;
        ViewComponent _viewComponent;

        float _resetOffset = 1.5f;
        Vector3 _storedTargetPosition = Vector3.zero;

        float minDistance = 0.8f; //  y 좌표 때문에 그런 듯
        int _nowIndexOfPath;
        List<Vector3> _path; // 데이터가 존재하지 않는다면 처음에 한번 돌려줘서 초기화 해줘야함

        protected Action<List<Vector3>> OnResetPathRequested;

        public FollowPath(MoveComponent moveComponent, ViewComponent viewComponent, CaptureComponent captureComponent, PathFinder pathFinder, Action<List<Vector3>> onResetPathRequested)
        {
            _captureComponent = captureComponent;
            _pathFinder = pathFinder;
            _moveComponent = moveComponent;
            _viewComponent = viewComponent;

            OnResetPathRequested = onResetPathRequested;
        }

        bool IsTargetPositionChanged()
        {
            ITarget target = _captureComponent.ReturnTarget();
            if(target == null) return false;

            Vector3 nowTargetPosition = target.ReturnPosition();

            if (_storedTargetPosition == Vector3.zero) // 최초의 경우
            {
                _storedTargetPosition = nowTargetPosition;
                return true;
            }

            if (Vector3.Distance(nowTargetPosition, _storedTargetPosition) < _resetOffset) return false; // 차이가 범위 내인 경우


            _storedTargetPosition = nowTargetPosition; // 차이가 범위 밖인 경우
            return true;
        }

        void ResetPath()
        {
            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return;

            List<Vector3> newPath = _pathFinder.ReturnPath(_captureComponent.transform.position, target.ReturnPosition());

            _nowIndexOfPath = 0;
            _path = newPath;
            OnResetPathRequested?.Invoke(_path);
        }

        void UpPathIndexWhenClose()
        {
            float distanceToPoint = Vector3.Distance(_moveComponent.transform.position, _path[_nowIndexOfPath]);
            if (distanceToPoint <= minDistance && _nowIndexOfPath < _path.Count - 1) // 최대 인덱스를 넘으면 안 됨
            {
                _nowIndexOfPath++;
            }
        }

        void MoveAlongPath()
        {
            if (_path.Count == 0) return;

            UpPathIndexWhenClose();
            _moveComponent.Move(_path[_nowIndexOfPath]);
            _viewComponent.View(_path[_nowIndexOfPath]);
        }

        public override NodeState Evaluate()
        {
            // 타겟이 없으면 리턴
            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return NodeState.FAILURE;

            bool nowChanged = IsTargetPositionChanged();
            if (nowChanged) ResetPath();

            MoveAlongPath();

            return NodeState.SUCCESS;
        }
    }

    public class GoDirectToPoint : Node
    {
        CaptureComponent _captureComponent;
        MoveComponent _moveComponent;
        ViewComponent _viewComponent;
        Action<List<Vector3>> OnResetPathRequested;
        Func<bool> IsPathEmpty;

        public GoDirectToPoint(CaptureComponent captureComponent, MoveComponent moveComponent, ViewComponent viewComponent, Action<List<Vector3>> onResetPathRequested, Func<bool> isPathEmpty)
        {
            _captureComponent = captureComponent;
            _moveComponent = moveComponent;
            _viewComponent = viewComponent;
            OnResetPathRequested = onResetPathRequested;
            IsPathEmpty = isPathEmpty;
        }

        public override NodeState Evaluate()
        {
            bool isEmpty = IsPathEmpty();
            if (isEmpty == false) OnResetPathRequested?.Invoke(new List<Vector3>()); // 리스트를 초기화해준다.

            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return NodeState.SUCCESS;

            Vector3 nowTargetPosition = target.ReturnPosition();

            _moveComponent.Move(nowTargetPosition);
            _viewComponent.View(nowTargetPosition);
            return NodeState.SUCCESS;
        }
    }

    public class LookAtTarget : Node
    {
        ViewComponent _viewComponent;
        CaptureComponent _captureComponent;
        Transform _arm;

        public LookAtTarget(CaptureComponent captureComponent, ViewComponent viewComponent, Transform arm = null)
        {
            _captureComponent = captureComponent;
            _viewComponent = viewComponent;
            _arm = arm;
        }

        public override NodeState Evaluate()
        {
            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return NodeState.SUCCESS;

            Vector3 nowTargetPosition = target.ReturnPosition();

            if(_arm == null) _viewComponent.View(nowTargetPosition);
            else _viewComponent.View(_arm, nowTargetPosition);

            return NodeState.SUCCESS;
        }
    }

    public class Stop : Node
    {
        MoveComponent _moveComponent;

        public Stop(MoveComponent moveComponent)
        {
            _moveComponent = moveComponent;
        }

        public override NodeState Evaluate()
        {
            _moveComponent.Stop();
            return NodeState.SUCCESS;
        }
    }

    public class Delay : Node
    {
        Timer _delayTimer;
        float _duration;

        public Delay(float duration)
        {
            _duration = duration;
            _delayTimer = new Timer();
            _delayTimer.Start(_duration);
        }

        public override NodeState Evaluate()
        {
            _delayTimer.Update();

            if(_delayTimer.IsRunning == true) return NodeState.RUNNING;

            if (_delayTimer.IsFinish == true)
            {
                _delayTimer.Reset();
                _delayTimer.Start(_duration);
            }

            return NodeState.SUCCESS;
        }
    }

    public class Spawn : Node
    {
        int _unitId;
        int _playerId;
        Transform _spawnPoint;
        Vector3[] _offsets;
        Quaternion _rotation;
        Action<int, int, Vector3, Vector3[], Quaternion> OnSpawnRequested;

        public Spawn(int unitId, int playerId, Transform spawnPoint, Vector3[] offsets, Quaternion rotation)
        {
            _unitId = unitId;
            _playerId = playerId;
            _spawnPoint = spawnPoint;
            _offsets = offsets;
            _rotation = rotation;

            GameObject go = GameObject.FindWithTag("Spawner");
            if (go == null) return;

            Spawner spawner = go.GetComponent<Spawner>();
            if (spawner == null) return;

            OnSpawnRequested = spawner.Spawn;
        }

        public override NodeState Evaluate()
        {
            OnSpawnRequested?.Invoke(_unitId, _playerId, _spawnPoint.position, _offsets, _rotation);
            return NodeState.SUCCESS;
        }
    }
}