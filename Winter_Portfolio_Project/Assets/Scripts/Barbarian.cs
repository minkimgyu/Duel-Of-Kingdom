using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;
using Tree = BehaviorTree.Tree;

public class Barbarian : GroundUnit
{
    MeleeAttackComponent _meleeAttackComponent;

    protected override void Start()
    {
        _meleeAttackComponent = GetComponent<MeleeAttackComponent>();
        base.Start();
    }

    protected override void InitializeBT()
    {
        GameObject go = GameObject.FindWithTag("GridManager");
        if (go == null) return;

        PathFinder pathFinder = go.GetComponent<PathFinder>();
        if (pathFinder == null) return;

        List<Node> _childNodes = new List<Node>()
        {
            new Selector
            (
                new List<Node>()
                {
                    new Sequence
                    (
                        new List<Node>
                        {
                            new CanFindTarget(_captureComponent), // 만약 타겟이 없다면 타워를 타겟으로 지정해준다.
                            new Selector
                            (
                                new List<Node>
                                {
                                    new Selector
                                    (
                                        new List<Node>
                                        {
                                            new Sequence
                                            (
                                                new List<Node>
                                                {
                                                    new CheckIsNearAndCancelAttackWhenExit(_captureComponent, 1.5f, 0.3f, _meleeAttackComponent), // DelayForAttack도 넣어주기

                                                    new StopAndLookAtTarget(_captureComponent, _moveComponent),

                                                    new Sequence
                                                    (
                                                        new List<Node>
                                                        {
                                                            new DelayForAttack(_meleeAttackComponent), // 딜레이 확인
                                                            new Attack(_meleeAttackComponent)
                                                        }
                                                    ),

                                                    
                                                    // 공격 진행
                                                    // 만약 공격이 진행 중인 경우 거리가 멀어져도 계속 진행
                                                }
                                            ),
                                            new Sequence
                                            (
                                                new List<Node>
                                                {
                                                    new CheckIsNear(_captureComponent, 2.5f),
                                                    new GoDirectToPoint(_captureComponent, _moveComponent, ResetPosListForDrawingGizmo, IsPosListEmpty)
                                                }
                                            )
                                        }
                                    ),
                                    new FollowPath(_moveComponent, _captureComponent, pathFinder, ResetPosListForDrawingGizmo)
                                }
                            )
                        }
                    ),
                    new Stop(_moveComponent)
                }
            )
        };

        Node rootNode = new Selector(_childNodes);
        _bt.SetUp(rootNode);
    }
}

public class Attack : Node // 좀 멀면 FindPath, 가까우면 GoDirectToPoint로 노드를 2개로 구분해주기
{
    Timer _delayTimer;
    AttackComponent _attackComponent;
    CaptureComponent _captureComponent;

    float damage = 10;
    float _damageApplyDelay;

    public Attack(AttackComponent attackComponent, CaptureComponent captureComponent, float damageApplyDelay)
    {
        _attackComponent = attackComponent;
        _delayTimer = new Timer();

        _damageApplyDelay = damageApplyDelay;
        _captureComponent = captureComponent;
    }

    public override NodeState Evaluate()
    {
        _delayTimer.Update();

        if(_delayTimer.IsRunning == false) _delayTimer.Start(_damageApplyDelay); // 이만큼 딜레이를 시켜줌

        if (_delayTimer.IsFinish == true)
        {
            // 타겟 찾아서 공격
            Transform target = _captureComponent.ReturnTarget();
            _attackComponent.Attack(target, damage);
            _delayTimer.Reset();
        }
        
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

    float Distance { get { return _nearDistance + _offsetDistance; } }

    CaptureComponent _captureComponent;

    public CheckDistance(CaptureComponent captureComponent, float nearDistance, float offsetDistance = 0)
    {
        _captureComponent = captureComponent;
        _nearDistance = nearDistance;
        _offsetDistance = offsetDistance;
    }

    protected bool NowEnoughClose(bool useOffset = false)
    {
        Transform target = _captureComponent.ReturnTarget();

        float distance = Vector3.Distance(_captureComponent.transform.position, target.position);

        if(useOffset == true)
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

    public CheckIsNearAndCancelAttackWhenExit(CaptureComponent captureComponent, float nearDistance, float offsetDistance, AttackComponent attackComponent) : base(captureComponent, nearDistance, offsetDistance) 
    {
        _attackComponent = attackComponent;
    }

    public override NodeState Evaluate()
    {
        bool isClose = false;

        // 이전에 실행된 경우는 다음과 같이 Exit 거리를 길게 잡아준다.
        if (_isNearBefore == true) isClose = NowEnoughClose(true);
        else isClose = NowEnoughClose();

        if(isClose == true)
        {
            if(_isNearBefore == false) _isNearBefore = true;
            return NodeState.SUCCESS;
        }

        if(_isNearBefore == true && isClose == false)
        {
            _isNearBefore = false; // 만약 이전에 실행된 경우인데 범위 밖으로 나가게 될 경우
            _attackComponent.CancelAttack = true; // 공격 취소해주기
            _attackComponent.CancelAttackAnimation(); // 애니메이션 캔슬해주기
        }

        return NodeState.FAILURE;
    }
}

public class DelayForAttack : Node
{
    // IsNear의 경우 SUCCESS 반환
    // DelayForAttack의 경우 Running 반환
    Timer _delayTimer;
    AttackComponent _attackComponent; // AttackComponent 내로 Delay를 빼준다.
    float _attackDelay = 2.5f;

    public DelayForAttack(AttackComponent attackComponent)
    {
        _attackComponent = attackComponent;
        _delayTimer = new Timer();
    }

    public override NodeState Evaluate()
    {
        _delayTimer.Update();

        if(_attackComponent.CancelAttack == true) // 다른 곳에서 공격이 캔슬이 난 경우
        {
            _attackComponent.CancelAttack = false;
            _delayTimer.Reset(); // 타이머 리셋 후 Success 리턴
            return NodeState.SUCCESS;
        }

        if (_attackComponent.IsFinish == true)
        {
            _delayTimer.Start(_attackDelay);
            _attackComponent.IsFinish = false;
        }

        if (_delayTimer.IsRunning)
        {
            return NodeState.RUNNING;
        }

        if (_delayTimer.IsFinish) _delayTimer.Reset();

        return NodeState.SUCCESS;
    }
}

public class CheckIsNear : CheckDistance
{
    public CheckIsNear(CaptureComponent captureComponent, float nearDistance) : base(captureComponent, nearDistance) { }

    public override NodeState Evaluate()
    {
        if(NowEnoughClose()) return NodeState.SUCCESS;
        return NodeState.FAILURE;
    }
}

public class IsCloseAndWhileAttack : CheckDistance // 가깝고 공격 중인 경우
{
    public IsCloseAndWhileAttack(CaptureComponent captureComponent, float nearDistance) : base(captureComponent, nearDistance) { }

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

    float _resetOffset = 1.5f;
    Vector3 _storedTargetPosition = Vector3.zero;

    float minDistance = 0.8f; //  y 좌표 때문에 그런 듯
    int _nowIndexOfPath;
    List<Vector3> _path; // 데이터가 존재하지 않는다면 처음에 한번 돌려줘서 초기화 해줘야함

    protected Action<List<Vector3>> OnResetPathRequested;

    public FollowPath(MoveComponent moveComponent, CaptureComponent captureComponent, PathFinder pathFinder, Action<List<Vector3>> onResetPathRequested)
    {
        _captureComponent = captureComponent;
        _pathFinder = pathFinder;
        _moveComponent = moveComponent;

        OnResetPathRequested = onResetPathRequested;
    }
    
    bool IsTargetPositionChanged()
    {
        Transform target = _captureComponent.ReturnTarget();
        Vector3 nowTargetPosition = target.position;

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
        Transform target = _captureComponent.ReturnTarget();
        List<Vector3> newPath = _pathFinder.ReturnPath(_captureComponent.transform.position, target.position);

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
        UpPathIndexWhenClose();
        _moveComponent.Move(_path[_nowIndexOfPath]);
    }

    public override NodeState Evaluate()
    {
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
    Action<List<Vector3>> OnResetPathRequested;
    Func<bool> IsPathEmpty;

    public GoDirectToPoint(CaptureComponent captureComponent, MoveComponent moveComponent, Action<List<Vector3>> onResetPathRequested, Func<bool> isPathEmpty)
    {
        _captureComponent = captureComponent;
        _moveComponent = moveComponent;
        OnResetPathRequested = onResetPathRequested;
        IsPathEmpty = isPathEmpty;
    }

    public override NodeState Evaluate()
    {
        bool isEmpty = IsPathEmpty();
        if(isEmpty == false) OnResetPathRequested?.Invoke(new List<Vector3>()); // 리스트를 초기화해준다.

        Transform target = _captureComponent.ReturnTarget();

        _moveComponent.Move(target.position);
        return NodeState.SUCCESS;
    }
}

public class StopAndLookAtTarget : Node
{
    MoveComponent _moveComponent;
    CaptureComponent _captureComponent;

    public StopAndLookAtTarget(CaptureComponent captureComponent, MoveComponent moveComponent)
    {
        _moveComponent = moveComponent;
        _captureComponent = captureComponent;
    }

    public override NodeState Evaluate()
    {
        Transform target = _captureComponent.ReturnTarget();

        _moveComponent.Stop(target.position);
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