using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPP.DRAWING;

namespace WPP.AI.GRID
{
    // ��� �����̺�� ��ӽ��Ѽ� Grid ��������ִ� ������� ���
    // ������ �ɱ� �� �ִ� ��ġ�� ��ȯ���ִ� Ŭ������ �ϳ� �� ������
    public class GridFillComponent : MonoBehaviour
    {
        // ����� �������� ��츦 ������ �����ؾ��� --> �׷��ϱ� 4���� ��������

        // ó���� ä�� ��ǥ�� ��� �ִ� �迭
        // ������ ��ǥ�� ��� �ִ� �迭

        // �̷��� �����س����� �����ϱ� ���� ��?

        // TopRight, BottomLeft �̷� ������ �� ��ǥ�� ���� �� ������ �׸��带 ä���ִ� ��� ������
        public struct AreaData
        {
            Vector2Int _topRight;
            public Vector2Int TopRight { get { return _topRight; } }

            Vector2Int _bottomLeft;
            public Vector2Int BottomLeft { get { return _bottomLeft; } }

            bool _nowFill;
            public bool NowFill { get { return _nowFill; } }

            public AreaData(Vector2Int topRight, Vector2Int bottomLeft, bool nowFill)
            {
                _topRight = topRight;
                _bottomLeft = bottomLeft;
                _nowFill = nowFill;
            }
        }

        DrawingData rNoDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(-2, 0, 0), new Vector3(-2, 0, 17), new Vector3(16, 0, 17), new Vector3(16, 0, 0) },
            new int[] { 0, 1, 2, 0, 2, 3}
        );

        DrawingData rAllDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(-2, 0, 0), new Vector3(-2, 0, 12), new Vector3(16, 0, 12), new Vector3(16, 0, 0) },
            new int[] { 0, 1, 2, 0, 2, 3 }
        );

        DrawingData rLeftDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(-2, 0, 0), new Vector3(-2, 0, 12), new Vector3(7, 0, 12), new Vector3(7, 0, 17), new Vector3(16, 0, 17), new Vector3(16, 0, 12), new Vector3(16, 0, 0) },
            new int[] { 0, 1, 6, 1, 5, 6, 5, 2, 3, 5, 3, 4 }
        );

        DrawingData rRightDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(-2, 0, 0), new Vector3(-2, 0, 12), new Vector3(-2, 0, 17), new Vector3(7, 0, 17), new Vector3(7, 0, 12), new Vector3(16, 0, 12), new Vector3(16, 0, 0) },
            new int[] { 0, 1, 6, 1, 5, 6, 1, 3, 4, 1, 2, 3 }
        );



        DrawingData cNoDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(-2, 0, 33), new Vector3(16, 0, 33), new Vector3(16, 0, 16), new Vector3(-2, 0, 16) },
            new int[] { 0, 1, 3, 3, 1, 2 }
        );

        DrawingData cAllDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(-2, 0, 33), new Vector3(16, 0, 33), new Vector3(16, 0, 21), new Vector3(-2, 0, 21) },
            new int[] { 0, 1, 3, 3, 1, 2 }
        );

        DrawingData cLeftDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(-2, 0, 33), new Vector3(16, 0, 33), new Vector3(16, 0, 21), new Vector3(16, 0, 16), new Vector3(7, 0, 16), new Vector3(7, 0, 21), new Vector3(-2, 0, 21) },
            new int[] { 0, 1, 6, 6, 1, 2, 5, 2, 4, 4, 2, 3 }
        );

        DrawingData cRightDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(-2, 0, 33), new Vector3(16, 0, 33), new Vector3(16, 0, 21), new Vector3(7, 0, 21), new Vector3(7, 0, 16), new Vector3(-2, 0, 16), new Vector3(-2, 0, 21) },
            new int[] { 0, 1, 6, 6, 1, 2, 5, 6, 3, 5, 3, 4 }
        );


        AreaData cAreaData = new AreaData(new Vector2Int(17, 32), new Vector2Int(0, 16), true);

        AreaData cLeftAreaData = new AreaData(new Vector2Int(8, 20), new Vector2Int(0, 16), false);
        AreaData cRightAreaData = new AreaData(new Vector2Int(17, 20), new Vector2Int(9, 16), false);

        AreaData rAreaData = new AreaData(new Vector2Int(17, 16), new Vector2Int(0, 0), true);

        AreaData rLeftAreaData = new AreaData(new Vector2Int(8, 15), new Vector2Int(0, 12), false);
        AreaData rRightAreaData = new AreaData(new Vector2Int(17, 15), new Vector2Int(9, 12), false);

        Func<Grid[,]> OnReturnGridRequested;
        Func<Vector3, Vector2Int> OnConvertV3ToIndexRequested;

        [SerializeField] AreaDrawer _spawnImpossibleRect;
        TowerCondition _storedTowerCondition;
        LandFormation _storedLandFormation;

        public void Initialize(GridStorage gridStorage)
        {
            OnReturnGridRequested = gridStorage.ReturnGridArray;
            OnConvertV3ToIndexRequested = gridStorage.ConvertPositionToIndex;
            _storedTowerCondition = TowerCondition.NoDestroy; // ó������ �̰ɷ� �ʱ�ȭ
        }

        // ��� ������ ������Ʈ �� �ɰ� ���ƹ�����
        public void OnLandFormationAssigned(LandFormation landFormation)
        {
            _storedLandFormation = landFormation;
            ResetGrid(OnReturnGridRequested(), landFormation);
        }

        // Ÿ���� �ı��� ��, ��� ���� ���� �� �ִ� ������ �ٲٱ�
        public void OnTowerConditionChange(TowerCondition towerCondition)
        {
            ResetGrid(OnReturnGridRequested(), _storedLandFormation, towerCondition);
            // ���⼭ _spawnRect �̰� �ʱ�ȭ ������
            // ���Ŀ� ������ �� �ʱ�ȭ�� �����͸� �ҷ��ͼ� ����ϸ� �� ��
        }

        // position���� ó���� �� �ְ� �ڵ带 ¥����
        // gridStorage�� ���ؼ� ��ġ�� �׸��� ��ǥ�� ��ȯ�ؼ� ������Ѻ���

        public void OnBuildingPlanted(Vector3 pos, OffsetRect offset, bool isMyBuilding)
        {
            Vector2Int index = OnConvertV3ToIndexRequested(pos);
           
            Grid[,] grids = OnReturnGridRequested();

            AreaData data = ReturnFillData(index, offset, true);
            ResetArea(OnReturnGridRequested(), data); // filldata�� �ٽ� �������� �� ��?

            if (isMyBuilding == false) return; // Client Id�� owership Id�� ���� ��쿡�� �Ʒ� ���� --> ������ ������Ų ������Ʈ�� ��쿡��
            ResetPass(grids, index, offset, false);
        }

        public void OnBuildingReleased(Vector3 pos, OffsetRect offset, bool isMyBuilding)
        {
            Vector2Int index = OnConvertV3ToIndexRequested(pos);

            Grid[,] grids = OnReturnGridRequested();

            AreaData data = ReturnFillData(index, offset, false);
            ResetArea(grids, data); // filldata�� �ٽ� �������� �� ��?

            if (isMyBuilding == false) return; // Client Id�� owership Id�� ���� ��쿡�� �Ʒ� ���� --> ������ ������Ų ������Ʈ�� ��쿡��
            ResetPass(grids, index, offset, true);
        }

        public void DrawSpawnImpossibleRect() => _spawnImpossibleRect.Draw();

        public void EraseSpawnImpossibleRect() => _spawnImpossibleRect.Erase();

        AreaData ReturnFillData(Vector2Int index, OffsetRect rect, bool nowFill)
        {
            Vector2Int topRightIndex = new Vector2Int(index.x + rect._right, index.y + rect._top);
            Vector2Int bottomLeftIndex = new Vector2Int(index.x - rect._left, index.y - rect._down);
            AreaData data = new AreaData(topRightIndex, bottomLeftIndex, nowFill);
            return data;
        }

        void ResetPass(Grid[,] grids, Vector2Int index, OffsetRect offset, bool canPass)
        {
            for (int i = index.x - offset._left; i <= index.x + offset._left; i++) 
                grids[i, index.y].CanPass = canPass;
        }

        void ResetArea(Grid[,] grids, AreaData data)
        {
            // ���� �׸����� ����� ���� �ʴ´ٸ� �� �̻� �����ϸ� �� �ɵ�?
            for (int x = data.BottomLeft.x; x <= data.TopRight.x; x++)
            {
                for (int y = data.BottomLeft.y; y <= data.TopRight.y; y++)
                {
                    grids[x, y].IsFill = data.NowFill;
                }
            }
        }

        // �̷��� �ָ� �ش� ���� �缳���ϴ� ����
        // ���� �ڵ忡 �߰��� ���� ���� ���� ǥ�õ� �ٽ� �׷��ֱ�
        void ResetGrid(Grid[,] grids, LandFormation myLandFormation)
        {
            _storedTowerCondition = TowerCondition.NoDestroy;

            if (myLandFormation == LandFormation.C) // �� ���� �������� �ݴ��� ������ �ʱ�ȭ �������
            {
                ResetArea(grids, rAreaData);
                _spawnImpossibleRect.Initialize(rNoDestroyDrawingData);
            }
            else if (myLandFormation == LandFormation.R)
            {
                ResetArea(grids, cAreaData);
                _spawnImpossibleRect.Initialize(cNoDestroyDrawingData);
            }
        }

        // ��� ������ �ٲ��� + ��� ���� �ٲ��� ����
        void ResetGrid(Grid[,] grids, LandFormation myLandFormation, TowerCondition myTowerCondition)
        {
            // ���� �׸����� ����� ���� �ʴ´ٸ� �� �̻� �����ϸ� �� �ɵ�?

            if(_storedTowerCondition == TowerCondition.NoDestroy)
            {
                _storedTowerCondition = myTowerCondition;

                if (myLandFormation == LandFormation.C)
                {
                    if (_storedTowerCondition == TowerCondition.LeftDestroy)
                    {
                        ResetArea(grids, rLeftAreaData);
                        _spawnImpossibleRect.Initialize(rLeftDestroyDrawingData);
                        return;
                    }
                    else if (_storedTowerCondition == TowerCondition.RightDestroy)
                    {
                        ResetArea(grids, rRightAreaData);
                        _spawnImpossibleRect.Initialize(rRightDestroyDrawingData);
                        return;
                    }
                }
                else if (myLandFormation == LandFormation.R)
                {
                    if (_storedTowerCondition == TowerCondition.LeftDestroy)
                    {
                        ResetArea(grids, cLeftAreaData);
                        _spawnImpossibleRect.Initialize(cLeftDestroyDrawingData);
                        return;
                    }
                    else if (_storedTowerCondition == TowerCondition.RightDestroy)
                    {
                        ResetArea(grids, cRightAreaData);
                        _spawnImpossibleRect.Initialize(cRightDestroyDrawingData);
                        return;
                    }
                }
            }
            else if(_storedTowerCondition == TowerCondition.LeftDestroy && myTowerCondition == TowerCondition.RightDestroy)
            {
                _storedTowerCondition = TowerCondition.AllDestroy;

                if (myLandFormation == LandFormation.C)
                {
                    ResetArea(grids, rRightAreaData);
                    _spawnImpossibleRect.Initialize(rAllDestroyDrawingData);
                    return;
                }
                else if (myLandFormation == LandFormation.R)
                {
                    ResetArea(grids, cRightAreaData);
                    _spawnImpossibleRect.Initialize(cAllDestroyDrawingData);
                    return;
                }
            }
            else if (_storedTowerCondition == TowerCondition.RightDestroy && myTowerCondition == TowerCondition.LeftDestroy)
            {
                _storedTowerCondition = TowerCondition.AllDestroy;

                if (myLandFormation == LandFormation.C)
                {
                    ResetArea(grids, rLeftAreaData);
                    _spawnImpossibleRect.Initialize(rAllDestroyDrawingData);
                    return;
                }
                else if (myLandFormation == LandFormation.R)
                {
                    ResetArea(grids, cLeftAreaData);
                    _spawnImpossibleRect.Initialize(cAllDestroyDrawingData);
                    return;
                }
            }
        }
    }
}
