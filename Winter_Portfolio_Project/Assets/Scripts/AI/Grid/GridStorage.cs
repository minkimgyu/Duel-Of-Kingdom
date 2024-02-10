using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WPP.AI.GRID
{
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
                if (tilePieces[i].IsBlock == false) // 하나라도 false인 경우
                {
                    return false;
                }
            }

            return true;
        }

        // Trasform으로 타워, 집과 같은 오브젝트를 저장해두기

        public bool IsWall { get { return CheckAllPieceIsBlock() || _canPass == false; } }
        public bool CanPlant { get { return CheckAllPieceIsBlock() == false && _isFill == false; } } // 타일이 IsBlock이거나 _canPlant가 false인 경우

        bool _canPass = true; // 못 지나다니는 구역
        public bool CanPass { set { _canPass = value; } }

        bool _isFill; // 비어있다면 건물이나 유닛을 올릴 수 있음
        public bool IsFill { get { return _isFill; } set { _isFill = value; } }

        public bool IsFastPath 
        { 
            get 
            {
                for (int i = 0; i < tilePieces.Count; i++)
                    if (tilePieces[i].IsFastPath == true) return true;

                return false;
            } 
        }

        public Grid ParentNode;

        [SerializeField] List<TilePiece> tilePieces;
        public List<TilePiece> TilePieces { get { return tilePieces; } }


        // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
        public int x, z, G, H;
        public int F { get { return G + H; } }
    }

    public enum TowerCondition
    {
        NoDestroy,
        LeftDestroy,
        RightDestroy,
        AllDestroy,
    }

    public enum LandFormation
    {
        C,
        R
    }

    //  GridStorage로 이름 바꿔주자
    public class GridStorage : MonoBehaviour
    {
        [SerializeField] TilePiece[] pieces;
        [SerializeField] Vector2Int _bottomLeft, _topRight;

        [SerializeField] int _width;
        [SerializeField] int _height;

        Grid[,] _gridArray;

        public Vector2Int ConvertPositionToIndex(Vector2 pos)
        {
            return new Vector2Int(Mathf.RoundToInt(pos.x) - _bottomLeft.x, Mathf.RoundToInt(pos.y) - _bottomLeft.y);
        }

        public Vector2Int ConvertPositionToIndex(Vector3 pos) 
        {
            return new Vector2Int(Mathf.RoundToInt(pos.x) - _bottomLeft.x, Mathf.RoundToInt(pos.z) - _bottomLeft.y);
        }

        public PathFindingParameter ReturnPathFindingValue() { return new PathFindingParameter(_gridArray, _bottomLeft, _topRight); }

        public Grid[,] ReturnGridArray() { return _gridArray; } // 이건 index 기반 그리드
        public RectInt ReturnGridRect() { return new RectInt(_bottomLeft.x, _bottomLeft.y, _width - 1, _height - 1); } // 이건 position

        // Start is called before the first frame update
        public void Initialize()
        {
            pieces = GetComponentsInChildren<TilePiece>();
            // 루프 돌려서 가장 왼쪽 아래, 오른쪽 위 오브젝트 구하기
            // 타일 가로 길이, 세로 길이 구하기
            // 이 데이터를 노드로 변환시켜서 보여주기

            for (int i = 0; i < pieces.Length; i++)
            {
                bool nowResetTopRight = false;

                if (pieces[i].transform.position.x > _topRight.x && pieces[i].transform.position.z >= _topRight.y || pieces[i].transform.position.x >= _topRight.x && pieces[i].transform.position.z > _topRight.y)
                {
                    _topRight.x = (int)pieces[i].transform.position.x;
                    _topRight.y = (int)pieces[i].transform.position.z;
                    //topRightTileIndex = i;
                    nowResetTopRight = true;
                }

                if (nowResetTopRight == true) continue;

                if (pieces[i].transform.position.x < _bottomLeft.x && pieces[i].transform.position.z <= _bottomLeft.y || pieces[i].transform.position.x <= _bottomLeft.x && pieces[i].transform.position.z < _bottomLeft.y)
                {
                    _bottomLeft.x = (int)pieces[i].transform.position.x;
                    _bottomLeft.y = (int)pieces[i].transform.position.z;
                    //bottomLeftTileIndex = i;
                }
            }

            _width = (_topRight.x - _bottomLeft.x) + 1; // 한 칸의 가로 세로 길이가 1임 --> 피벗이 중심이라서 양 끝이 포함되지 않음 그래서 1을 더해줘야함
            _height = (_topRight.y - _bottomLeft.y) + 1;

            //localBottomLeft = new Vector2Int(0, 0);
            //localTopRight = new Vector2Int(width - 1, height - 1);

            // piece를 Node 데이터로 변환해줘야할 듯
            _gridArray = new Grid[_width, _height];

            for (int i = 0; i < pieces.Length; i++)
            {
                int tmpXIndex = (int)(pieces[i].transform.position.x - _bottomLeft.x);
                int tmpZIndex = (int)(pieces[i].transform.position.z - _bottomLeft.y);

                if (_gridArray[tmpXIndex, tmpZIndex] == null)
                {
                    _gridArray[tmpXIndex, tmpZIndex] = new Grid(pieces[i], tmpXIndex, tmpZIndex); // localPos
                }
                else
                {
                    // 여기 리스트에 추가하는 방식
                    _gridArray[tmpXIndex, tmpZIndex].TilePieces.Add(pieces[i]);
                }
            }
        }
    }
}