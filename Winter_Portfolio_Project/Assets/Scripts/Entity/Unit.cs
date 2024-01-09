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
    // �⺻���� ��ġ�� ����� ���⼭ ����

    // �̵����� ����

    public enum Type
    {
        Air,
        Ground
    }

    protected Type _type;

    protected Tree _bt;
    protected MoveComponent _moveComponent;
    protected CaptureComponent _captureComponent;

    protected List<Vector3> _posListForDrawingGizmo = new List<Vector3>(); // �̰Ŵ� BT���� �������ֱ�

    

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _moveComponent = GetComponent<MoveComponent>();
        _captureComponent = GetComponentInChildren<CaptureComponent>();

        _bt = new Tree();

        InitializeBT(); // ���⼭ �ʱ�ȭ ����
    }

    // Update is called once per frame
    void Update()
    {
        _bt.OnUpdate();
    }

    protected virtual void InitializeBT() { }

    protected void ResetPosListForDrawingGizmo(List<Vector3> posList) => _posListForDrawingGizmo = posList;

    protected bool IsPosListEmpty() { return _posListForDrawingGizmo.Count == 0; }

    void OnDrawGizmos() // �̰͵� �̺�Ʈ�� �Ѱ�����
    {
        // ���� �ֱ� ������ ���� ������
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

//public class Chase : Node // �� �ָ� FindPath, ������ GoDirectToPoint�� ��带 2���� �������ֱ�
//{
//    // FindTarget�� �ʿ���
//    // Trigger�� ���ؼ� �̺�Ʈ �޾Ƽ� ����

//    // ���� Trigger�� ������ ����� ���ٸ� ���� ��ó Ÿ���� ������� �������ش�.

//    // ���� ����� ��� --> Ÿ�� �켱 ���� --> ü���� ���� ������ ã��
//    // �켱 ������ Entity, Building �̷� ������ �ؾ��� ��

//    // Ư�� ��ü�� ������ �����ϸ� �ش� ��ü�� ���� ������ ������
//    // ���� ����� �ٲ�

//    // ���� Ÿ���� ã�Ƽ� ��ġ�� ��ȯ����
//    // �� ��ġ�� Int �������� ��ȯ�ؼ� ���� ���ٸ� PathFind�� ������ --> �̰Ŵ� Int�� ��ȯ�ϰ� ��ġ�� ���� �̻� �޶��� ��� �ٽ� �޾ƿ���
//    // �ٸ��ٸ� PathFind�� �ٽ� ����

//    // ������ �Ÿ��� ���� �̻� ��������� PathFind�� ���߰� 
//    // �ڵ带 ���� �ٰ���

//    // ������ ������ ��쿡�� �׷���

//    // �����Ÿ� �̻� �ٰ����� ���ݴ������ �����ع��� --> ���⼭���� "Ư�� ��ü�� ������ �����ϸ� �ش� ��ü�� ���� ������ ������" ����

//    public Chase(PathFinder pathFinder)
//    {

//    }

//    public override NodeState Evaluate() 
//    {

//        return NodeState.SUCCESS;
//    }
//}