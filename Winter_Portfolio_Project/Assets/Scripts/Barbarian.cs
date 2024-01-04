using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;
using Tree = BehaviorTree.Tree;

public class Barbarian : GroundEntity
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
                            new CanFindTarget(_captureComponent),
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
                                                    new IsNearAndDelayForAttack(_captureComponent, 1.5f, _meleeAttackComponent), // DelayForAttack�� �־��ֱ�

                                                    new StopAndLookAtTarget(_captureComponent, _moveComponent),
                                                    new Attack(_meleeAttackComponent),
                                                    // ���� ����
                                                    // ���� ������ ���� ���� ��� �Ÿ��� �־����� ��� ����
                                                }
                                            ),
                                            new Sequence
                                            (
                                                new List<Node>
                                                {
                                                    new IsNear(_captureComponent, 2.5f),
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

public class Attack : Node // �� �ָ� FindPath, ������ GoDirectToPoint�� ��带 2���� �������ֱ�
{
    AttackComponent _attackComponent;
    float damage = 10;

    public Attack(AttackComponent attackComponent)
    {
        _attackComponent = attackComponent;
    }

    public override NodeState Evaluate()
    {
        _attackComponent.Attack(damage);
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
    float _nearDistance;
    CaptureComponent _captureComponent;

    public CheckDistance(CaptureComponent captureComponent, float nearDistance)
    {
        _captureComponent = captureComponent;
        _nearDistance = nearDistance;
    }

    protected bool NowEnoughClose()
    {
        Transform target = _captureComponent.ReturnTarget();

        float distance = Vector3.Distance(_captureComponent.transform.position, target.position);
        if (distance <= _nearDistance) return true;

        return false;
    }
}

public class IsNearAndDelayForAttack : CheckDistance
{
    // IsNear�� ��� SUCCESS ��ȯ
    // DelayForAttack�� ��� Running ��ȯ
    Timer _delayTimer;
    AttackComponent _attackComponent;
    float _attackDelay = 2.5f;

    public IsNearAndDelayForAttack(CaptureComponent captureComponent, float nearDistance, AttackComponent attackComponent) : base(captureComponent, nearDistance) 
    {
        _attackComponent = attackComponent;
        _delayTimer = new Timer();
    }

    public override NodeState Evaluate()
    {
        _delayTimer.Update();

        if(_attackComponent.IsFinish == true)
        {
            _delayTimer.Start(_attackDelay);
            _attackComponent.IsFinish = false;
        }

        if(_delayTimer.IsRunning)
        {
            return NodeState.RUNNING;
        }

        if (_delayTimer.IsFinish) _delayTimer.Reset();

        if (NowEnoughClose()) return NodeState.SUCCESS;
        return NodeState.FAILURE;
    }
}

public class IsNear : CheckDistance
{
    public IsNear(CaptureComponent captureComponent, float nearDistance) : base(captureComponent, nearDistance) { }

    public override NodeState Evaluate()
    {
        if(NowEnoughClose()) return NodeState.SUCCESS;
        return NodeState.FAILURE;
    }
}

public class IsCloseAndWhileAttack : CheckDistance // ������ ���� ���� ���
{
    public IsCloseAndWhileAttack(CaptureComponent captureComponent, float nearDistance) : base(captureComponent, nearDistance) { }

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

    float _resetOffset = 1.5f;
    Vector3 _storedTargetPosition = Vector3.zero;

    float minDistance = 0.8f; //  y ��ǥ ������ �׷� ��
    int _nowIndexOfPath;
    List<Vector3> _path; // �����Ͱ� �������� �ʴ´ٸ� ó���� �ѹ� �����༭ �ʱ�ȭ �������

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
        Transform target = _captureComponent.ReturnTarget();
        List<Vector3> newPath = _pathFinder.ReturnPath(_captureComponent.transform.position, target.position);

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
        if(isEmpty == false) OnResetPathRequested?.Invoke(new List<Vector3>()); // ����Ʈ�� �ʱ�ȭ���ش�.

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