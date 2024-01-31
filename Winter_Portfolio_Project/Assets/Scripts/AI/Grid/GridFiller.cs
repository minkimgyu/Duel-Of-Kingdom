using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPP.DRAWING;

namespace WPP.GRID
{
    // ��� �����̺�� ��ӽ��Ѽ� Grid ��������ִ� ������� ���
    // ������ �ɱ� �� �ִ� ��ġ�� ��ȯ���ִ� Ŭ������ �ϳ� �� ������
    public class GridFiller : MonoBehaviour
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
            new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 17), new Vector3(14, 0, 17), new Vector3(14, 0, 0) },
            new int[] { 0, 1, 2, 0, 2, 3}
        );

        DrawingData rAllDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 12), new Vector3(14, 0, 12), new Vector3(14, 0, 0) },
            new int[] { 0, 1, 2, 0, 2, 3 }
        );

        DrawingData rLeftDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 12), new Vector3(7, 0, 12), new Vector3(7, 0, 17), new Vector3(14, 0, 17), new Vector3(14, 0, 12), new Vector3(14, 0, 0) },
            new int[] { 0, 1, 6, 1, 5, 6, 5, 2, 3, 5, 3, 4 }
        );

        DrawingData rRightDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 12), new Vector3(0, 0, 17), new Vector3(7, 0, 17), new Vector3(7, 0, 12), new Vector3(14, 0, 12), new Vector3(14, 0, 0) },
            new int[] { 0, 1, 6, 1, 5, 6, 1, 3, 4, 1, 2, 3 }
        );



        DrawingData cNoDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(0, 0, 33), new Vector3(14, 0, 33), new Vector3(14, 0, 16), new Vector3(0, 0, 16) },
            new int[] { 0, 1, 3, 3, 1, 2 }
        );

        DrawingData cAllDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(0, 0, 33), new Vector3(14, 0, 33), new Vector3(14, 0, 21), new Vector3(0, 0, 21) },
            new int[] { 0, 1, 3, 3, 1, 2 }
        );

        DrawingData cLeftDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(0, 0, 33), new Vector3(14, 0, 33), new Vector3(14, 0, 21), new Vector3(14, 0, 16), new Vector3(7, 0, 16), new Vector3(7, 0, 21), new Vector3(0, 0, 21) },
            new int[] { 0, 1, 6, 6, 1, 2, 5, 2, 4, 4, 2, 3 }
        );

        DrawingData cRightDestroyDrawingData = new DrawingData(
            new Vector3[] { new Vector3(0, 0, 33), new Vector3(14, 0, 33), new Vector3(14, 0, 21), new Vector3(7, 0, 21), new Vector3(7, 0, 16), new Vector3(0, 0, 16), new Vector3(0, 0, 21) },
            new int[] { 0, 1, 6, 6, 1, 2, 5, 6, 3, 5, 3, 4 }
        );


        AreaData cAreaData = new AreaData(new Vector2Int(13, 32), new Vector2Int(0, 16), true);

        AreaData cLeftAreaData = new AreaData(new Vector2Int(6, 20), new Vector2Int(0, 16), false);
        AreaData cRightAreaData = new AreaData(new Vector2Int(13, 20), new Vector2Int(7, 16), false);

        AreaData rAreaData = new AreaData(new Vector2Int(13, 16), new Vector2Int(0, 0), true);

        AreaData rLeftAreaData = new AreaData(new Vector2Int(6, 15), new Vector2Int(0, 12), false);
        AreaData rRightAreaData = new AreaData(new Vector2Int(13, 15), new Vector2Int(7, 12), false);

        Func<Grid[,]> OnReturnGridRequested;
        Func<Vector3, Vector2Int> OnConvertV3ToIndexRequested;

        [SerializeField] SpawnAreaDrawer _spawnRect;
        [SerializeField] TowerCondition _storedTowerCondition;

        private void Awake()
        {
            GridStorage gridStorage = GetComponent<GridStorage>();
            if (gridStorage == null) return;

            OnReturnGridRequested = gridStorage.ReturnGridArray;
            OnConvertV3ToIndexRequested = gridStorage.ConvertPositionToIndex;
        }

        // ��� ������ ������Ʈ �� �ɰ� ���ƹ�����
        public void OnLandFormationAssigned(LandFormation landFormation)
        {
            ResetGrid(OnReturnGridRequested(), landFormation);
        }

        // Ÿ���� �ı��� ��, ��� ���� ���� �� �ִ� ������ �ٲٱ�
        public void OnTowerConditionChanged(LandFormation landFormation, TowerCondition towerCondition)
        {
            ResetGrid(OnReturnGridRequested(), landFormation, towerCondition);
            // ���⼭ _spawnRect �̰� �ʱ�ȭ ������
            // ���Ŀ� ������ �� �ʱ�ȭ�� �����͸� �ҷ��ͼ� ����ϸ� �� ��
        }

        // position���� ó���� �� �ְ� �ڵ带 ¥����
        // gridStorage�� ���ؼ� ��ġ�� �׸��� ��ǥ�� ��ȯ�ؼ� ������Ѻ���

        public void OnBuildingPlanted(Vector3 pos, OffsetFromCenter offset)
        {
            Vector2Int index = OnConvertV3ToIndexRequested(pos);

            Grid[,] grids = OnReturnGridRequested();
            ResetPass(grids, index, false);

            AreaData data = ReturnFillData(index, offset, true);
            ResetArea(OnReturnGridRequested(), data); // filldata�� �ٽ� �������� �� ��?
        }

        public void OnBuildingReleased(Vector3 pos, OffsetFromCenter offset)
        {
            Vector2Int index = OnConvertV3ToIndexRequested(pos);

            Grid[,] grids = OnReturnGridRequested();
            ResetPass(grids, index, true);

            AreaData data = ReturnFillData(index, offset, false);
            ResetArea(grids, data); // filldata�� �ٽ� �������� �� ��?
        }

        AreaData ReturnFillData(Vector2Int index, OffsetFromCenter rect, bool nowFill)
        {
            Vector2Int topRightIndex = new Vector2Int(index.x + rect._right, index.y + rect._top);
            Vector2Int bottomLeftIndex = new Vector2Int(index.x - rect._left, index.y - rect._down);
            AreaData data = new AreaData(topRightIndex, bottomLeftIndex, nowFill);
            return data;
        }

        void ResetPass(Grid[,] grids, Vector2Int index, bool canPass)
        {
            grids[index.x, index.y].CanPass = canPass;
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
                _spawnRect.Initialize(rNoDestroyDrawingData);
            }
            else if (myLandFormation == LandFormation.R)
            {
                ResetArea(grids, cAreaData);
                _spawnRect.Initialize(cNoDestroyDrawingData);
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
                        _spawnRect.Initialize(rLeftDestroyDrawingData);
                    }
                    else if (_storedTowerCondition == TowerCondition.RightDestroy)
                    {
                        ResetArea(grids, rRightAreaData);
                        _spawnRect.Initialize(rRightDestroyDrawingData);
                    }
                }
                else if (myLandFormation == LandFormation.R)
                {
                    if (_storedTowerCondition == TowerCondition.LeftDestroy)
                    {
                        ResetArea(grids, cLeftAreaData);
                        _spawnRect.Initialize(cLeftDestroyDrawingData);
                    }
                    else if (_storedTowerCondition == TowerCondition.RightDestroy)
                    {
                        ResetArea(grids, cRightAreaData);
                        _spawnRect.Initialize(cRightDestroyDrawingData);
                    }
                }
            }
            else if(_storedTowerCondition == TowerCondition.LeftDestroy && myTowerCondition == TowerCondition.RightDestroy)
            {
                _storedTowerCondition = TowerCondition.AllDestroy;

                if (myLandFormation == LandFormation.C)
                {
                    ResetArea(grids, rRightAreaData);
                    _spawnRect.Initialize(rAllDestroyDrawingData);
                }
                else if (myLandFormation == LandFormation.R)
                {
                    ResetArea(grids, cRightAreaData);
                    _spawnRect.Initialize(cAllDestroyDrawingData);
                }
            }
            else if (_storedTowerCondition == TowerCondition.RightDestroy && myTowerCondition == TowerCondition.LeftDestroy)
            {
                _storedTowerCondition = TowerCondition.AllDestroy;

                if (myLandFormation == LandFormation.C)
                {
                    ResetArea(grids, rLeftAreaData);
                    _spawnRect.Initialize(rAllDestroyDrawingData);
                }
                else if (myLandFormation == LandFormation.R)
                {
                    ResetArea(grids, cLeftAreaData);
                    _spawnRect.Initialize(cAllDestroyDrawingData);
                }
            }
        }
    }
}
