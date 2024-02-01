using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.DRAWING;
using System;

namespace WPP.AI.GRID
{
    [Serializable]
    public struct OffsetRect
    {
        public int _top, _down, _left, _right;
        const float _defaultLengthFromCenter = 0.5f;

        public OffsetRect(int top, int down, int left, int right)
        {
            _top = top;
            _down = down;
            _left = left;
            _right = right;
        }

        public DrawingData ReturnRectDrawingData()
        {
            Vector3[] verties = new Vector3[] {
                new Vector3(-_left - _defaultLengthFromCenter, 0, -_down - _defaultLengthFromCenter),
                new Vector3(-_left - _defaultLengthFromCenter, 0, _top + _defaultLengthFromCenter),
                new Vector3(_right + _defaultLengthFromCenter, 0, _top + _defaultLengthFromCenter),
                new Vector3(_right + _defaultLengthFromCenter, 0, -_down - _defaultLengthFromCenter)
            };
            int[] orders = new int[] { 0, 1, 2, 0, 2, 3 };

            return new DrawingData(verties, orders);
        }

        int ReturnCenter(int length) { return Mathf.FloorToInt((float)length / 2); }

        public Vector2Int ReturnCenterOffset()
        {
            int width = _left + _right + (int)_defaultLengthFromCenter * 2;
            int height = _top + _down + (int)_defaultLengthFromCenter * 2;

            return new Vector2Int(ReturnCenter(width), ReturnCenter(height));
        }
    }

    public class GridSelectComponent : MonoBehaviour
    {
        [SerializeField] SpawnAreaDrawer _spawnRect;
        Func<Grid[,]> OnReturnGridRequested;
        Func<RectInt> OnReturnGridRectRequested;
        Func<Vector3, Vector2Int> OnConvertV3ToIndexRequested;
        Func<Vector2, Vector2Int> OnConvertV2ToIndexRequested;

        OffsetRect _rectSize;
        Vector2Int _centerOffset;

        [SerializeField] Vector2Int minV2;
        [SerializeField] Vector2Int maxV2;

        Vector3 _spawnPoint;
        int _layer;
        float _maxDistance = 50;

        public void Initialize(GridStorage gridStorage)
        {
            _layer = LayerMask.GetMask("Tile");

            OnReturnGridRequested = gridStorage.ReturnGridArray;
            OnReturnGridRectRequested = gridStorage.ReturnGridRect;

            OnConvertV3ToIndexRequested = gridStorage.ConvertPositionToIndex;
            OnConvertV2ToIndexRequested = gridStorage.ConvertPositionToIndex;
        }

        public Vector3 ReturnSpawnPoint() { return new Vector3(_spawnPoint.x, 1, _spawnPoint.z); }

        public void EraseArea()
        {
            _spawnRect.Erase();
        }

        void DrawArea()
        {
            DrawingData data = _rectSize.ReturnRectDrawingData();
            _spawnRect.Initialize(data);
            _spawnRect.Draw();
        }

        public void ResetRect(OffsetRect offsetFromCenter)
        {
            _rectSize = offsetFromCenter;
            _centerOffset = offsetFromCenter.ReturnCenterOffset();
            DrawArea();
        }

        RectInt ReturnClampedRange()
        {
            RectInt gridWorldPos = OnReturnGridRectRequested();

            return new RectInt(
                gridWorldPos.min.x + _rectSize._left, 
                gridWorldPos.min.y + _rectSize._down, 
                gridWorldPos.width - _rectSize._left - _rectSize._right,
                gridWorldPos.height - _rectSize._top - _rectSize._down);
        }

        Vector3 ReturnClampedPos(Vector3 pos)
        {
            RectInt clampedRange = ReturnClampedRange();
            float clampedX = Mathf.Clamp(pos.x, clampedRange.min.x, clampedRange.max.x);
            float clampedZ = Mathf.Clamp(pos.z, clampedRange.min.y, clampedRange.max.y);
            return new Vector3(clampedX, 1.1f, clampedZ);
        }

        public void SelectGrid()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RectInt gridWorldPos = OnReturnGridRectRequested();

            if (Physics.Raycast(ray, out hit, _maxDistance, _layer))
            {
                Vector3 clampedPos = ReturnClampedPos(hit.transform.position);
                Vector2Int indexOfGrid = OnConvertV3ToIndexRequested(clampedPos);

                int minXIndex = indexOfGrid.x - _rectSize._left;
                int minYIndex = indexOfGrid.y - _rectSize._down;
                minV2 = new Vector2Int(minXIndex, minYIndex);

                int maxXIndex = indexOfGrid.x + _rectSize._right;
                int maxYIndex = indexOfGrid.y + _rectSize._top;
                maxV2 = new Vector2Int(maxXIndex, maxYIndex);

                // 그리드의 최대 최소 값 인덱스
                RectInt clampedRange = ReturnClampedRange();

                Vector2Int downLeft = OnConvertV2ToIndexRequested(clampedRange.min);
                Vector2Int topRight = OnConvertV2ToIndexRequested(clampedRange.max);

                // 우선 가장 먼저 마우스 포인터 주변 인덱스를 조사해서 그 부분이 Filled 되어있는지 판단해보기
                // 그렇다면 아래 코드 실행해주기

                List<Vector3> points = new List<Vector3>();

                Grid[,] grids = OnReturnGridRequested();
                for (int i = downLeft.y; i <= topRight.y; i++)
                {
                    bool thisRectIsBlock = false;
                    for (int j = minXIndex; j <= maxXIndex; j++)
                    {
                        bool exitThisLine = false;
                        for (int z = i - _rectSize._down; z <= i + _rectSize._top; z++)
                        {
                            if (grids[j, z].CanPlant == false)
                            {
                                exitThisLine = true;
                                break;
                            }
                        }

                        if (exitThisLine)
                        {
                            thisRectIsBlock = true;
                            break;
                        }
                    }

                    if (thisRectIsBlock == false) points.Add(new Vector3(clampedPos.x, 1.1f, i + gridWorldPos.yMin));
                }

                float storedDistance = 0;
                int selectedIndex = -1;

                // 마우스와 가장 가까운 지점을 반환해서 그 위치로 spawnRect를 위치시킴
                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 0)
                    {
                        selectedIndex = i;
                        storedDistance = Vector3.Distance(clampedPos, points[i]);
                    }
                    else
                    {
                        float tmpDistance = Vector3.Distance(clampedPos, points[i]);
                        if (tmpDistance < storedDistance)
                        {
                            storedDistance = tmpDistance;
                            selectedIndex = i;
                        }
                    }
                }

                _spawnPoint = points[selectedIndex];
                _spawnRect.Move(_spawnPoint);
            }
        }

        //private void Update()
        //{
 
        //}

        // 가장 먼저 그리드 사이즈를 받아서 마우스가 이동할 수 있는 최소, 최대 좌표를 구해보자
        // 먼저 오브젝트로 마우스 이동 입력을 받아보는 걸로 시작하고 이동은 int 범위로만 하는 걸로 하자

        // 마우스 좌표를 반올림 해서 int로 받기
        // 여기에 범위를 심길 오브젝트의 범위를 넘겨서 초기화해주고 그 값을 바탕으로 바닥 그리드를 그려주자

        // 결과적으로 해야할 부분은 위치 정보 넘겨주고 바닥 그리드 그려주는 기능을 추가하면 됨
    }
}