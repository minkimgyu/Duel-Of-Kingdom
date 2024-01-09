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
                            new CanFindTarget(_captureComponent), // ���� Ÿ���� ���ٸ� Ÿ���� Ÿ������ �������ش�.
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
                                                    new CheckIsNearAndCancelAttackWhenExit(_captureComponent, 1.5f, 0.3f, _meleeAttackComponent), // DelayForAttack�� �־��ֱ�

                                                    new StopAndLookAtTarget(_captureComponent, _moveComponent),

                                                    new Sequence
                                                    (
                                                        new List<Node>
                                                        {
                                                            new DelayForAttack(_meleeAttackComponent), // ������ Ȯ��
                                                            new Attack(_meleeAttackComponent)
                                                        }
                                                    ),

                                                    
                                                    // ���� ����
                                                    // ���� ������ ���� ���� ��� �Ÿ��� �־����� ��� ����
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

public class Attack : Node // �� �ָ� FindPath, ������ GoDirectToPoint�� ��带 2���� �������ֱ�
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

        if(_delayTimer.IsRunning == false) _delayTimer.Start(_damageApplyDelay); // �̸�ŭ �����̸� ������

        if (_delayTimer.IsFinish == true)
        {
            // Ÿ�� ã�Ƽ� ����
            Transform target = _captureComponent.ReturnTarget();
            _attackComponent.Attack(target, damage);
            _delayTimer.Reset();
        }
        
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
/// ��������� SUCCESS�� ��ȯ����. �ѹ� SUCCESS�� ��ȯ����ٸ� FAILURE�� ��ȯ�ϴ� �Ÿ��� �����
/// </summary>
public class CheckIsNearAndCancelAttackWhenExit : CheckDistance
{
    bool _isNearBefore = false; // ������ �ѹ� ��������� ���
    AttackComponent _attackComponent;

    public CheckIsNearAndCancelAttackWhenExit(CaptureComponent captureComponent, float nearDistance, float offsetDistance, AttackComponent attackComponent) : base(captureComponent, nearDistance, offsetDistance) 
    {
        _attackComponent = attackComponent;
    }

    public override NodeState Evaluate()
    {
        bool isClose = false;

        // ������ ����� ���� ������ ���� Exit �Ÿ��� ��� ����ش�.
        if (_isNearBefore == true) isClose = NowEnoughClose(true);
        else isClose = NowEnoughClose();

        if(isClose == true)
        {
            if(_isNearBefore == false) _isNearBefore = true;
            return NodeState.SUCCESS;
        }

        if(_isNearBefore == true && isClose == false)
        {
            _isNearBefore = false; // ���� ������ ����� ����ε� ���� ������ ������ �� ���
            _attackComponent.CancelAttack = true; // ���� ������ֱ�
            _attackComponent.CancelAttackAnimation(); // �ִϸ��̼� ĵ�����ֱ�
        }

        return NodeState.FAILURE;
    }
}

public class DelayForAttack : Node
{
    // IsNear�� ��� SUCCESS ��ȯ
    // DelayForAttack�� ��� Running ��ȯ
    Timer _delayTimer;
    AttackComponent _attackComponent; // AttackComponent ���� Delay�� ���ش�.
    float _attackDelay = 2.5f;

    public DelayForAttack(AttackComponent attackComponent)
    {
        _attackComponent = attackComponent;
        _delayTimer = new Timer();
    }

    public override NodeState Evaluate()
    {
        _delayTimer.Update();

        if(_attackComponent.CancelAttack == true) // �ٸ� ������ ������ ĵ���� �� ���
        {
            _attackComponent.CancelAttack = false;
            _delayTimer.Reset(); // Ÿ�̸� ���� �� Success ����
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