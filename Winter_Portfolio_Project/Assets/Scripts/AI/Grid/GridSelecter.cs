using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.DRAWING;
using System;

namespace WPP.GRID
{
    [Serializable]
    public struct OffsetFromCenter
    {
        public int _top, _down, _left, _right;

        public OffsetFromCenter(int top, int down, int left, int right)
        {
            _top = top;
            _down = down;
            _left = left;
            _right = right;
        }
    }


    public class GridSelecter : MonoBehaviour
    {
        [SerializeField] SpawnAreaDrawer _spawnRect;
        Func<Grid[,]> OnReturnGridRequested;
        Func<RectInt> OnReturnGridRectRequested;
        Func<Vector3, Vector2Int> OnConvertV3ToIndexRequested;
        Func<Vector2, Vector2Int> OnConvertV2ToIndexRequested;

        RectInt _rectSize;
        Vector2Int _centerOffset;

        [SerializeField] Vector2Int minV2;
        [SerializeField] Vector2Int maxV2;

        Vector3 _spawnPoint;
        int _layer;
        float _maxDistance = 50;

        private void Start()
        {
            _layer = LayerMask.GetMask("Tile");

            GridStorage gridStorage = GetComponent<GridStorage>();
            if (gridStorage == null) return;
            
            OnReturnGridRequested = gridStorage.ReturnGridArray;
            OnReturnGridRectRequested = gridStorage.ReturnGridRect;

            OnConvertV3ToIndexRequested = gridStorage.ConvertPositionToIndex;
            OnConvertV2ToIndexRequested = gridStorage.ConvertPositionToIndex;

            Initialize();
            DrawArea(_rectSize);
        }

        public Vector3 ReturnSpawnPoint() { return new Vector3(_spawnPoint.x, 1, _spawnPoint.z); }

        int ReturnCenterOffset(int length) { return Mathf.FloorToInt((float)length / 2); }

        void DrawArea(RectInt rectInt)
        {
            Vector3[] verties = new Vector3[] { 
                new Vector3(0, 0, 0), 
                new Vector3(0, 0, rectInt.max.y), 
                new Vector3(rectInt.max.x, 0, rectInt.max.y), 
                new Vector3(rectInt.max.x, 0, 0) 
            };
            int[] orders = new int[] { 0, 1, 2, 0, 2, 3 };

            DrawingData data = new DrawingData(verties, orders);
            _spawnRect.Initialize(data);
            _spawnRect.Draw();
        }

        public void Initialize()
        {
            int width = 3;
            int height = 3;

            _rectSize = new RectInt(Vector2Int.zero, new Vector2Int(width, height));
            _centerOffset = new Vector2Int(ReturnCenterOffset(width), ReturnCenterOffset(height));
        }

        public void Initialize(RectInt rectInt)
        {
            _rectSize = rectInt;
            _centerOffset = new Vector2Int(ReturnCenterOffset(_rectSize.width), ReturnCenterOffset(_rectSize.height));
        }

        OffsetFromCenter ReturnSpawnRectOffset()
        {
            int leftX = _centerOffset.x;
            int downY = _centerOffset.y;

            int rightX = _rectSize.width - leftX - 1;
            int topY = _rectSize.height - downY - 1;

            return new OffsetFromCenter(topY, downY, leftX, rightX);
        }

        RectInt ReturnClampedRange()
        {
            OffsetFromCenter offset = ReturnSpawnRectOffset();
            RectInt gridWorldPos = OnReturnGridRectRequested();

            return new RectInt(
                gridWorldPos.min.x + offset._left, 
                gridWorldPos.min.y + offset._down, 
                gridWorldPos.width - offset._left - offset._right,
                gridWorldPos.height - offset._top - offset._down);
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

                OffsetFromCenter offset = ReturnSpawnRectOffset();

                int minXIndex = indexOfGrid.x - offset._left;
                int minYIndex = indexOfGrid.y - offset._down;
                minV2 = new Vector2Int(minXIndex, minYIndex);

                int maxXIndex = indexOfGrid.x + offset._right;
                int maxYIndex = indexOfGrid.y + offset._top;
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
                        for (int z = i - offset._down; z <= i + offset._top; z++)
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

                //Debug.Log(points[selectedIndex]);
                //Debug.Log(selectedIndex);
                _spawnPoint = points[selectedIndex];

                Vector3 changedPos = points[selectedIndex] + new Vector3(-0.5f, 0, -0.5f);
                _spawnRect.Move(changedPos, new Vector3(-_centerOffset.x, 0, -_centerOffset.y));
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