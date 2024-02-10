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
                if (tilePieces[i].IsBlock == false) // �ϳ��� false�� ���
                {
                    return false;
                }
            }

            return true;
        }

        // Trasform���� Ÿ��, ���� ���� ������Ʈ�� �����صα�

        public bool IsWall { get { return CheckAllPieceIsBlock() || _canPass == false; } }
        public bool CanPlant { get { return CheckAllPieceIsBlock() == false && _isFill == false; } } // Ÿ���� IsBlock�̰ų� _canPlant�� false�� ���

        bool _canPass = true; // �� �����ٴϴ� ����
        public bool CanPass { set { _canPass = value; } }

        bool _isFill; // ����ִٸ� �ǹ��̳� ������ �ø� �� ����
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


        // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
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

    //  GridStorage�� �̸� �ٲ�����
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

        public Grid[,] ReturnGridArray() { return _gridArray; } // �̰� index ��� �׸���
        public RectInt ReturnGridRect() { return new RectInt(_bottomLeft.x, _bottomLeft.y, _width - 1, _height - 1); } // �̰� position

        // Start is called before the first frame update
        public void Initialize()
        {
            pieces = GetComponentsInChildren<TilePiece>();
            // ���� ������ ���� ���� �Ʒ�, ������ �� ������Ʈ ���ϱ�
            // Ÿ�� ���� ����, ���� ���� ���ϱ�
            // �� �����͸� ���� ��ȯ���Ѽ� �����ֱ�

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

            _width = (_topRight.x - _bottomLeft.x) + 1; // �� ĭ�� ���� ���� ���̰� 1�� --> �ǹ��� �߽��̶� �� ���� ���Ե��� ���� �׷��� 1�� ���������
            _height = (_topRight.y - _bottomLeft.y) + 1;

            //localBottomLeft = new Vector2Int(0, 0);
            //localTopRight = new Vector2Int(width - 1, height - 1);

            // piece�� Node �����ͷ� ��ȯ������� ��
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
                    // ���� ����Ʈ�� �߰��ϴ� ���
                    _gridArray[tmpXIndex, tmpZIndex].TilePieces.Add(pieces[i]);
                }
            }
        }
    }
}