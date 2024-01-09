using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Tree = BehaviorTree.Tree;

public class GroundUnit : Unit
{
    protected override void Start()
    {
        base.Start();
        _type = Type.Ground;
    }
}

public class Unit : Entity
{
    // 기본적인 겹치는 기능은 여기서 구현

    // 이동성을 가짐

    public enum Type
    {
        Air,
        Ground
    }

    protected Type _type;

    protected Tree _bt;
    protected MoveComponent _moveComponent;
    protected CaptureComponent _captureComponent;

    protected List<Vector3> _posListForDrawingGizmo = new List<Vector3>(); // 이거는 BT에서 지정해주기

    

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _moveComponent = GetComponent<MoveComponent>();
        _captureComponent = GetComponentInChildren<CaptureComponent>();

        _bt = new Tree();

        InitializeBT(); // 여기서 초기화 진행
    }

    // Update is called once per frame
    void Update()
    {
        _bt.OnUpdate();
    }

    protected virtual void InitializeBT() { }

    protected void ResetPosListForDrawingGizmo(List<Vector3> posList) => _posListForDrawingGizmo = posList;

    protected bool IsPosListEmpty() { return _posListForDrawingGizmo.Count == 0; }

    void OnDrawGizmos() // 이것도 이벤트로 넘겨주자
    {
        // 가장 최근 리턴한 길을 보여줌
        if (_posListForDrawingGizmo.Count == 0) return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < _posListForDrawingGizmo.Count - 1; i++)
            Gizmos.DrawLine(
                new Vector3(_posListForDrawingGizmo[i].x, 1.5f, _posListForDrawingGizmo[i].z),
                new Vector3(_posListForDrawingGizmo[i + 1].x, 1.5f, _posListForDrawingGizmo[i + 1].z));
    }
}

//protected virtual void InitializeBT()
//{
//    GameObject go = GameObject.FindWithTag("GridManager");
//    if (go == null) return;

//    PathFinder pathFinder = go.GetComponent<PathFinder>();
//    if (pathFinder == null) return;

//    posList = pathFinder.ReturnPath(transform.position, target.position);

//    List<Node> _childNodes = new List<Node>()
//    {
//        //new Chase(pathFinder)
//    };

//    Node rootNode = new Selector(_childNodes);

//    _bt.SetUp(rootNode);
//}

//public class DelayForNextAttack : Node
//{
//    public DelayForNextAttack(PathFinder pathFinder)
//    {

//    }

//    public override NodeState Evaluate()
//    {

//        return NodeState.SUCCESS;
//    }
//}

//public class Attack : Node
//{
//    public Attack(PathFinder pathFinder)
//    {

//    }

//    public override NodeState Evaluate()
//    {

//        return NodeState.SUCCESS;
//    }
//}

//public class AssignToAttackTarget : Node
//{
//    public AssignToAttackTarget(PathFinder pathFinder)
//    {

//    }

//    public override NodeState Evaluate()
//    {

//        return NodeState.SUCCESS;
//    }
//}

//public class FindTarget : Node
//{
//    public FindTarget(PathFinder pathFinder)
//    {

//    }

//    public override NodeState Evaluate()
//    {

//        return NodeState.SUCCESS;
//    }
//}

//public class Chase : Node // 좀 멀면 FindPath, 가까우면 GoDirectToPoint로 노드를 2개로 구분해주기
//{
//    // FindTarget이 필요함
//    // Trigger를 통해서 이벤트 받아서 진행

//    // 만약 Trigger에 잡히는 대상이 없다면 가장 근처 타워를 대상으로 지정해준다.

//    // 가장 가까운 대상 --> 타겟 우선 순위 --> 체력이 낮은 순서로 찾음
//    // 우선 순위는 Entity, Building 이런 식으로 해야할 듯

//    // 특정 개체를 때리기 시작하면 해당 개체가 죽을 때까지 공격함
//    // 이후 대상을 바꿈

//    // 먼저 타겟을 찾아서 위치를 반환해줌
//    // 이 위치를 Int 형식으로 반환해서 만약 같다면 PathFind를 생략함 --> 이거는 Int로 반환하고 위치가 일정 이상 달라질 경우 다시 받아오자
//    // 다르다면 PathFind를 다시 돌림

//    // 대상과의 거리가 일정 이상 가까워지면 PathFind를 멈추고 
//    // 코드를 통해 다가감

//    // 기즈모는 추적의 경우에만 그려줌

//    // 일정거리 이상 다가가면 공격대상으로 지정해버림 --> 여기서부터 "특정 개체를 때리기 시작하면 해당 개체가 죽을 때까지 공격함" 적용

//    public Chase(PathFinder pathFinder)
//    {

//    }

//    public override NodeState Evaluate() 
//    {

//        return NodeState.SUCCESS;
//    }
//}