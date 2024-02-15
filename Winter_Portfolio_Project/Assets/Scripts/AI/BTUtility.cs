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
using WPP.AI.GRID;
using WPP.Collection;
using WPP.ClientInfo.Card;

namespace WPP.AI.BTUtility
{
    public class Attack : Node // �� �ָ� FindPath, ������ GoDirectToPoint�� ��带 2���� �������ֱ�
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
            // Ÿ�� ã�Ƽ� ����
            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return NodeState.FAILURE;

            //_captureComponent.FixTarget(target); // ���� Ÿ���� ��� ����� �ı��Ǳ� ������ ��� ������Ŵ

            _attackComponent.Attack(target);
            return NodeState.SUCCESS;
        }
    }

    public class CanFindTarget : Node // �� �ָ� FindPath, ������ GoDirectToPoint�� ��带 2���� �������ֱ�
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

    abstract public class CheckDistance : Node // �� �ָ� FindPath, ������ GoDirectToPoint�� ��带 2���� �������ֱ�
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
    /// ��������� SUCCESS�� ��ȯ����. �ѹ� SUCCESS�� ��ȯ����ٸ� FAILURE�� ��ȯ�ϴ� �Ÿ��� �����
    /// </summary>
    public class CheckIsNearAndCancelAttackWhenExit : CheckDistance
    {
        bool _isNearBefore = false; // ������ �ѹ� ��������� ���
        AttackComponent _attackComponent;

        public CheckIsNearAndCancelAttackWhenExit(CaptureComponent captureComponent, float nearDistance, float offsetDistance, AttackComponent attackComponent, bool useOffset = false, float myColliderOffset = 0) : base(captureComponent, nearDistance, offsetDistance, useOffset, myColliderOffset)
        {
            _attackComponent = attackComponent;
        }

        public override NodeState Evaluate()
        {
            bool isClose = false;

            // ������ ����� ���� ������ ���� Exit �Ÿ��� ��� ����ش�.
            if (_isNearBefore == true) isClose = NowEnoughClose(true);
            else isClose = NowEnoughClose();

            if (isClose == true)
            {
                if (_isNearBefore == false) _isNearBefore = true;
                return NodeState.SUCCESS;
            }

            if (_isNearBefore == true && isClose == false)
            {
                if(_attackComponent.Fix) // ���� �̹� ������ ���� ��� ����������
                {
                    return NodeState.RUNNING;
                }
                else
                {
                    // ���� ĵ�������ֱ�
                    _attackComponent.CancelAttack();
                    _isNearBefore = false; // �ٽ� ��������ֱ�
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

    public class FollowPath : Node // �� �ָ� FindPath, ������ GoDirectToPoint�� ��带 2���� �������ֱ�
    {
        CaptureComponent _captureComponent;
        PathFinder _pathFinder;
        MoveComponent _moveComponent;
        ViewComponent _viewComponent;

        float _resetOffset = 1.5f;
        Vector3 _storedTargetPosition = Vector3.zero;

        float minDistance = 0.8f; //  y ��ǥ ������ �׷� ��
        int _nowIndexOfPath;
        List<Vector3> _path = new List<Vector3>(); // �����Ͱ� �������� �ʴ´ٸ� ó���� �ѹ� �����༭ �ʱ�ȭ �������

        protected Action<List<Vector3>> OnResetPathRequested;

        bool _isMyEntity;
        bool _ignoreWall;

        public FollowPath(MoveComponent moveComponent, ViewComponent viewComponent, CaptureComponent captureComponent, PathFinder pathFinder, Action<List<Vector3>> onResetPathRequested, bool isMyEntity, bool ignoreWall)
        {
            _captureComponent = captureComponent;
            _pathFinder = pathFinder;
            _moveComponent = moveComponent;
            _viewComponent = viewComponent;

            OnResetPathRequested = onResetPathRequested;
            _isMyEntity = isMyEntity;
            _ignoreWall = ignoreWall;

        }

        bool IsTargetPositionChanged()
        {
            ITarget target = _captureComponent.ReturnTarget();
            if(target == null) return false;

            Vector3 nowTargetPosition = target.ReturnPosition();

            if (_storedTargetPosition == Vector3.zero) // ������ ���
            {
                _storedTargetPosition = nowTargetPosition;
                return true;
            }

            if (Vector3.Distance(nowTargetPosition, _storedTargetPosition) < _resetOffset) return false; // ���̰� ���� ���� ���


            _storedTargetPosition = nowTargetPosition; // ���̰� ���� ���� ���
            return true;
        }

        void ResetPath()
        {
            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return;

            List<Vector3> newPath = _pathFinder.ReturnPath(_captureComponent.transform.position, target.ReturnPosition(), _isMyEntity, _ignoreWall);

            _nowIndexOfPath = 0;
            _path = newPath;
            OnResetPathRequested?.Invoke(_path);
        }

        void UpPathIndexWhenClose()
        {
            float distanceToPoint = Vector3.Distance(_moveComponent.transform.position, _path[_nowIndexOfPath]);
            if (distanceToPoint <= minDistance && _nowIndexOfPath < _path.Count - 1) // �ִ� �ε����� ������ �� ��
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

        bool IsPathBlock() { return _pathFinder.IsPathBlock(_path, _isMyEntity); }

        public override NodeState Evaluate()
        {
            // Ÿ���� ������ ����
            ITarget target = _captureComponent.ReturnTarget();
            if (target == null) return NodeState.FAILURE;

            // ��ΰ� �����ִ� ���
            bool nowBlock = IsPathBlock();
            if (nowBlock) ResetPath();

            // Ÿ���� ��ġ�� �ٲ� ���
            bool nowChanged = IsTargetPositionChanged();
            if (nowBlock == false && nowChanged) ResetPath(); // ��ΰ� �������� ������ Ÿ���� ��ġ�� �ٲ�ų� �ʱ��� ���

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
            if (isEmpty == false) OnResetPathRequested?.Invoke(new List<Vector3>()); // ����Ʈ�� �ʱ�ȭ���ش�.

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
        int _level;
        int _ownershipId;
        Transform _spawnPoint;
        Vector3[] _offsets;
        Func<string, int, int, Vector3, Vector3[], Entity[]> OnSpawnRequested;

        string _cardId;

        CardData _cardData;

        public Spawn(int unitId, int level, int ownershipId, Transform spawnPoint, Vector3[] offsets)
        {
            _unitId = unitId;
            _level = level;
            _ownershipId = ownershipId;
            _spawnPoint = spawnPoint;
            _offsets = offsets;

            GameObject go = GameObject.FindWithTag("Spawner");
            if (go == null) return;

            Spawner spawner = go.GetComponent<Spawner>();
            if (spawner == null) return;

            _cardData = CardCollection.Instance().FindCard(_unitId, _level);
            _cardId = _cardData.unit._name;

            OnSpawnRequested = spawner.Spawn;
        }

        public override NodeState Evaluate()
        {
            OnSpawnRequested?.Invoke(_cardId, _level, _ownershipId, _spawnPoint.position, _offsets);
            return NodeState.SUCCESS;
        }
    }
}