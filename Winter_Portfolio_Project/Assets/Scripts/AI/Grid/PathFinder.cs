using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WPP.AI.GRID
{
    public struct PathFindingParameter
    {
        public PathFindingParameter(Grid[,] gridArray, Vector2Int bottomLeft, Vector2Int topRight)
        {
            _gridArray = gridArray;
            _bottomLeft = bottomLeft;
            _topRight = topRight;
        }

        Grid[,] _gridArray;
        public Grid[,] GridArray { get { return _gridArray; } }

        Vector2Int _bottomLeft, _topRight;
        public Vector2Int BottomLeft { get { return _bottomLeft; } }
        public Vector2Int TopRight { get { return _topRight; } }

        // ���� �迭�� ����, �ε��� 0�� �迭�� ����
        public Vector2Int LocalBottomLeft { get { return new Vector2Int(0, 0); } } // BottomLeft�� �������� ����
        public Vector2Int LocalTopRight { get { return new Vector2Int(_gridArray.GetLength(0) - 1, _gridArray.GetLength(1) - 1); } }
    }

    public class PathFinder : MonoBehaviour
    {
        Func<PathFindingParameter> OnPathFindingRequested;
        Func<Vector3, Vector2Int> OnConvertV3ToIndexRequested;

        [SerializeField] bool allowDiagonal, dontCrossCorner;

        // Start is called before the first frame update
        void Awake()
        {
            GridStorage gridStorage = GetComponent<GridStorage>();
            if (gridStorage == null) return;

            OnPathFindingRequested = gridStorage.ReturnPathFindingValue;
            OnConvertV3ToIndexRequested = gridStorage.ConvertPositionToIndex;
        }

        //public bool IsPathBlock(List<Vector3> path)
        //{
        //    if (path.Count == 0) return false;

        //    PathFindingParameter parameter = OnPathFindingRequested();
        //    for (int i = 0; i < path.Count; i++)
        //    {
        //        Vector2Int index = OnConvertV3ToIndexRequested(path[i]);
        //        if (parameter.GridArray[index.x, index.y].IsWall == true) return true;
        //    }

        //    return false;
        //}

        public bool IsPathBlock(List<Vector3> path, bool isMyEntity)
        {
            if (path.Count == 0) return false;

            PathFindingParameter parameter = OnPathFindingRequested();
            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int index = OnConvertV3ToIndexRequested(path[i]);

                if(isMyEntity)
                {
                    if (parameter.GridArray[index.x, index.y].IsMyWall == true) return true;
                }
                else
                {
                    if (parameter.GridArray[index.x, index.y].IsYourWall == true) return true;
                }
            }
            return false;
        }



        Vector3Int SwitchV3ToV3Int(Vector3 pos)
        {
            return new Vector3Int((int)MathF.Round(pos.x), (int)MathF.Round(pos.y), (int)MathF.Round(pos.z));
        }

        List<Vector3> SwitchLocalGridToWorldPos(List<Grid> finalGridList, Vector2Int bottomLeft, float startYPos)
        {
            List<Vector3> worldPos = new List<Vector3>();

            for (int i = 0; i < finalGridList.Count; i++)
            {
                worldPos.Add(new Vector3(finalGridList[i].x + bottomLeft.x, startYPos, finalGridList[i].z + bottomLeft.y));
            }

            return worldPos;
        }

        public List<Vector3> ReturnPath(Vector3 startPos, Vector3 targetPos, bool isMyEntity, bool ignoreWall)
        {
            // Vector3 startPos, Vector3 targetPos --> �̰Ÿ� Vector3Int�� �����ֱ�
            Vector3Int startIntPos = SwitchV3ToV3Int(startPos);
            Vector3Int targetIntPos = SwitchV3ToV3Int(targetPos);

            PathFindingParameter parameter = OnPathFindingRequested();
            return CalculatePath(startIntPos, targetIntPos, parameter, startPos.y, isMyEntity, ignoreWall);
        }

        #region NewCalculatePath

        List<Vector3> CalculatePath(Vector3 startIntPos, Vector3 targetIntPos, PathFindingParameter parameter, float startYPos, bool isMyEntity, bool ignoreWall)
        {
            List<Grid> FinalNodeList;
            List<Grid> OpenList, ClosedList;
            Grid StartNode, TargetNode, CurNode;

            int startX = (int)(startIntPos.x - parameter.BottomLeft.x);
            int startY = (int)(startIntPos.z - parameter.BottomLeft.y);

            int targetX = (int)(targetIntPos.x - parameter.BottomLeft.x);
            int targetY = (int)(targetIntPos.z - parameter.BottomLeft.y);

            if ((startX > parameter.LocalTopRight.x || startX < parameter.LocalBottomLeft.x) || (targetX > parameter.LocalTopRight.x || targetX < parameter.LocalBottomLeft.x)
                || (startY > parameter.LocalTopRight.y || startY < parameter.LocalBottomLeft.y) || (targetY > parameter.LocalTopRight.y || targetY < parameter.LocalBottomLeft.y))
            {
                Debug.Log("���� �ۿ� ���� ��ġ�� ������");
                return new List<Vector3>(); // �⺻�� ��ȯ
            }

            StartNode = parameter.GridArray[startX, startY];
            TargetNode = parameter.GridArray[targetX, targetY];


            OpenList = new List<Grid>() { StartNode };
            ClosedList = new List<Grid>();
            FinalNodeList = new List<Grid>();

            int tmpCount = 0;

            while (OpenList.Count > 0)
            {
                tmpCount++;
                //Debug.Log(tmpCount);

                // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
                CurNode = OpenList[0];
                for (int i = 1; i < OpenList.Count; i++)
                    if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

                OpenList.Remove(CurNode);
                ClosedList.Add(CurNode);

                // ������
                if (CurNode == TargetNode)
                {
                    Grid TargetCurNode = TargetNode;
                    while (TargetCurNode != StartNode)
                    {
                        FinalNodeList.Add(TargetCurNode);
                        TargetCurNode = TargetCurNode.ParentNode;
                    }
                    FinalNodeList.Add(StartNode);
                    FinalNodeList.Reverse();

                    // bottomLeft�� �̿��ؼ� 
                    //for (int i = 0; i < FinalNodeList.Count; i++) print(i + "��°�� " + FinalNodeList[i].x + ", " + FinalNodeList[i].z);
                    return SwitchLocalGridToWorldPos(FinalNodeList, parameter.BottomLeft, startYPos);
                }

                // �֢آע�
                if (allowDiagonal)
                {
                    OpenListAdd(parameter, CurNode.x + 1, CurNode.z + 1, OpenList, ClosedList, CurNode, TargetNode, isMyEntity, ignoreWall);
                    OpenListAdd(parameter, CurNode.x - 1, CurNode.z + 1, OpenList, ClosedList, CurNode, TargetNode, isMyEntity, ignoreWall);
                    OpenListAdd(parameter, CurNode.x - 1, CurNode.z - 1, OpenList, ClosedList, CurNode, TargetNode, isMyEntity, ignoreWall);
                    OpenListAdd(parameter, CurNode.x + 1, CurNode.z - 1, OpenList, ClosedList, CurNode, TargetNode, isMyEntity, ignoreWall);
                }

                // �� �� �� ��
                OpenListAdd(parameter, CurNode.x, CurNode.z + 1, OpenList, ClosedList, CurNode, TargetNode, isMyEntity, ignoreWall);
                OpenListAdd(parameter, CurNode.x + 1, CurNode.z, OpenList, ClosedList, CurNode, TargetNode, isMyEntity, ignoreWall);
                OpenListAdd(parameter, CurNode.x, CurNode.z - 1, OpenList, ClosedList, CurNode, TargetNode, isMyEntity, ignoreWall);
                OpenListAdd(parameter, CurNode.x - 1, CurNode.z, OpenList, ClosedList, CurNode, TargetNode, isMyEntity, ignoreWall);
            }

            return new List<Vector3>(); // �⺻�� ��ȯ
        }

        void OpenListAdd(PathFindingParameter parameter, int checkX, int checkZ, List<Grid> OpenList, List<Grid> ClosedList, Grid CurNode, Grid TargetNode, bool isMyEntity, bool ignoreWall)
        {
            // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
            if (checkX >= parameter.LocalBottomLeft.x && checkX < parameter.LocalTopRight.x + 1 && checkZ >= parameter.LocalBottomLeft.y && checkZ < parameter.LocalTopRight.y + 1 &&
                !ClosedList.Contains(parameter.GridArray[checkX - parameter.LocalBottomLeft.x, checkZ - parameter.LocalBottomLeft.y]))
            {
                if (ignoreWall == false) // ���� �������� �ʴ´ٸ� �Ʒ��� ������
                {
                    if(isMyEntity == true)
                    {
                        if (parameter.GridArray[checkX - parameter.LocalBottomLeft.x, checkZ - parameter.LocalBottomLeft.y].IsMyWall == true) return;

                        // �밢�� ����, �� ���̷� ��� �ȵ�
                        if (allowDiagonal) if (parameter.GridArray[CurNode.x - parameter.LocalBottomLeft.x, checkZ - parameter.LocalBottomLeft.y].IsMyWall && parameter.GridArray[checkX - parameter.LocalBottomLeft.x, CurNode.z - parameter.LocalBottomLeft.y].IsMyWall) return;

                        // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
                        if (dontCrossCorner) if (parameter.GridArray[CurNode.x - parameter.LocalBottomLeft.x, checkZ - parameter.LocalBottomLeft.y].IsMyWall || parameter.GridArray[checkX - parameter.LocalBottomLeft.x, CurNode.z - parameter.LocalBottomLeft.y].IsMyWall) return;
                    }
                    else
                    {
                        if (parameter.GridArray[checkX - parameter.LocalBottomLeft.x, checkZ - parameter.LocalBottomLeft.y].IsYourWall == true) return;

                        // �밢�� ����, �� ���̷� ��� �ȵ�
                        if (allowDiagonal) if (parameter.GridArray[CurNode.x - parameter.LocalBottomLeft.x, checkZ - parameter.LocalBottomLeft.y].IsYourWall && parameter.GridArray[checkX - parameter.LocalBottomLeft.x, CurNode.z - parameter.LocalBottomLeft.y].IsYourWall) return;

                        // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
                        if (dontCrossCorner) if (parameter.GridArray[CurNode.x - parameter.LocalBottomLeft.x, checkZ - parameter.LocalBottomLeft.y].IsYourWall || parameter.GridArray[checkX - parameter.LocalBottomLeft.x, CurNode.z - parameter.LocalBottomLeft.y].IsYourWall) return;
                    }
                }

                // �̿���忡 �ְ�, ������ 10, �밢���� 14���
                Grid NeighborNode = parameter.GridArray[checkX - parameter.LocalBottomLeft.x, checkZ - parameter.LocalBottomLeft.y];
                int MoveCost;
                // ���� ����� ��� �ٸ��� �ʱ�ȭ����
                if (NeighborNode.IsFastPath)
                    MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.z - checkZ == 0 ? 2 : 3);
                else
                    MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.z - checkZ == 0 ? 10 : 14);
                // ���⼭ Ÿ�� piece�� ���� ��찡 �´ٸ� movecost�� �Ϻ� �ٿ��ش�.

                // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
                if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.z - TargetNode.z)) * 10;
                    NeighborNode.ParentNode = CurNode;

                    OpenList.Add(NeighborNode);
                }
            }
        }

        #endregion CalculatePath
    }
}