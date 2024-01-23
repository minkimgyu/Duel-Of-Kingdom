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
            if(tilePieces[i].IsBlock == false) // 하나라도 false인 경우
            {
                return false;
            }
        }

        return true;
    }
    
    // Trasform으로 타워, 집과 같은 오브젝트를 저장해두기

    public bool IsWall { get { return CheckAllPieceIsBlock(); } }
    public Grid ParentNode;

    [SerializeField] List<TilePiece> tilePieces;
    public List<TilePiece> TilePieces { get { return tilePieces; } }


    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
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
        // 루프 돌려서 가장 왼쪽 아래, 오른쪽 위 오브젝트 구하기
        // 타일 가로 길이, 세로 길이 구하기
        // 이 데이터를 노드로 변환시켜서 보여주기

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

        width = (topRight.x - bottomLeft.x) + 1; // 한 칸의 가로 세로 길이가 1임 --> 피벗이 중심이라서 양 끝이 포함되지 않음 그래서 1을 더해줘야함
        height = (topRight.y - bottomLeft.y) + 1;

        //localBottomLeft = new Vector2Int(0, 0);
        //localTopRight = new Vector2Int(width - 1, height - 1);

        // piece를 Node 데이터로 변환해줘야할 듯
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
                // 여기 리스트에 추가하는 방식
                gridArray[tmpXIndex, tmpZIndex].TilePieces.Add(pieces[i]);
            }
        }
    }
}
