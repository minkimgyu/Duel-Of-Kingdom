using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Grid
{
    public Grid(TilePiece tilePiece, int localX, int localZ) 
    { 
        x = localX;
        z = localZ;

        tilePieces = new List<TilePiece>();
        tilePieces.Add(tilePiece);
    }

    bool CheckAllPieceIsBlock()
    {
        for (int i = 0; i < tilePieces.Count; i++)
        {
            if(tilePieces[i].IsBlock == false) // �ϳ��� false�� ���
            {
                return false;
            }
        }

        return true;
    }
    
    // Trasform���� Ÿ��, ���� ���� ������Ʈ�� �����صα�

    public bool IsWall { get { return CheckAllPieceIsBlock(); } }
    public Grid ParentNode;

    [SerializeField] List<TilePiece> tilePieces;
    public List<TilePiece> TilePieces { get { return tilePieces; } }


    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
    public int x, z, G, H;
    public int F { get { return G + H; } }
}

public class GridManager : MonoBehaviour
{
    [SerializeField] TilePiece[] pieces;

    //[SerializeField] int topRightTileIndex;
    //[SerializeField] int bottomLeftTileIndex;

    [SerializeField] Vector2Int bottomLeft, topRight;

    [SerializeField] int width;
    [SerializeField] int height;

    [SerializeField] Grid[,] gridArray;

    public PathFindingParameter ReturnPathFindingValue() { return new PathFindingParameter(gridArray, bottomLeft, topRight); }

    // Start is called before the first frame update
    void Awake()
    {
        pieces = GetComponentsInChildren<TilePiece>();
        // ���� ������ ���� ���� �Ʒ�, ������ �� ������Ʈ ���ϱ�
        // Ÿ�� ���� ����, ���� ���� ���ϱ�
        // �� �����͸� ���� ��ȯ���Ѽ� �����ֱ�

        for (int i = 0; i < pieces.Length; i++)
        {
            bool nowResetTopRight = false;

            if (pieces[i].transform.position.x > topRight.x && pieces[i].transform.position.z >= topRight.y || pieces[i].transform.position.x >= topRight.x && pieces[i].transform.position.z > topRight.y)
            {
                topRight.x = (int)pieces[i].transform.position.x;
                topRight.y = (int)pieces[i].transform.position.z;
                //topRightTileIndex = i;
                nowResetTopRight = true;
            }

            if (nowResetTopRight == true) continue;

            if (pieces[i].transform.position.x < bottomLeft.x && pieces[i].transform.position.z <= bottomLeft.y || pieces[i].transform.position.x <= bottomLeft.x && pieces[i].transform.position.z < bottomLeft.y)
            {
                bottomLeft.x = (int)pieces[i].transform.position.x;
                bottomLeft.y = (int)pieces[i].transform.position.z;
                //bottomLeftTileIndex = i;
            }
        }

        width = (topRight.x - bottomLeft.x) + 1; // �� ĭ�� ���� ���� ���̰� 1�� --> �ǹ��� �߽��̶� �� ���� ���Ե��� ���� �׷��� 1�� ���������
        height = (topRight.y - bottomLeft.y) + 1;

        //localBottomLeft = new Vector2Int(0, 0);
        //localTopRight = new Vector2Int(width - 1, height - 1);

        // piece�� Node �����ͷ� ��ȯ������� ��
        gridArray = new Grid[width, height];

        for (int i = 0; i < pieces.Length; i++)
        {
            int tmpXIndex = (int)(pieces[i].transform.position.x - bottomLeft.x);
            int tmpZIndex = (int)(pieces[i].transform.position.z - bottomLeft.y);

            if (gridArray[tmpXIndex, tmpZIndex] == null)
            {
                gridArray[tmpXIndex, tmpZIndex] = new Grid(pieces[i], tmpXIndex, tmpZIndex); // localPos
            }
            else
            {
                // ���� ����Ʈ�� �߰��ϴ� ���
                gridArray[tmpXIndex, tmpZIndex].TilePieces.Add(pieces[i]);
            }
        }
    }
}
