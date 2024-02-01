using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.DRAWING;
using System;

namespace WPP.AI.GRID
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

    public class SelectComponent : MonoBehaviour
    {
        [SerializeField] SpawnAreaDrawer _spawnRect;
        Func<Grid[,]> OnReturnGridRequested;
        Func<RectInt> OnReturnGridRectRequested;
        Func<Vector3, Vector2Int> OnConvertV3ToIndexRequested;
        Func<Vector2, Vector2Int> OnConvertV2ToIndexRequested;

        OffsetFromCenter _rectSize;
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

            //DrawArea(_rectSize);
        }

        public Vector3 ReturnSpawnPoint() { return new Vector3(_spawnPoint.x, 1, _spawnPoint.z); }

        int ReturnCenterOffset(int length) { return Mathf.FloorToInt((float)length / 2); }

        void DrawArea()
        {
            Vector3[] verties = new Vector3[] { 
                new Vector3(-_rectSize._left, 0, -_rectSize._down),
                new Vector3(-_rectSize._left, 0, _rectSize._top),
                new Vector3(_rectSize._right, 0, _rectSize._top),
                new Vector3(_rectSize._right, 0, -_rectSize._down)
            };
            int[] orders = new int[] { 0, 1, 2, 0, 2, 3 };

            DrawingData data = new DrawingData(verties, orders);
            _spawnRect.Initialize(data);
            _spawnRect.Draw();
        }

        public void ResetRect(OffsetFromCenter offsetFromCenter)
        {
            int width = offsetFromCenter._left + offsetFromCenter._right + 1;
            int height = offsetFromCenter._top + offsetFromCenter._down + 1;

            _rectSize = offsetFromCenter;
            _centerOffset = new Vector2Int(ReturnCenterOffset(width), ReturnCenterOffset(height));
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

                // �׸����� �ִ� �ּ� �� �ε���
                RectInt clampedRange = ReturnClampedRange();

                Vector2Int downLeft = OnConvertV2ToIndexRequested(clampedRange.min);
                Vector2Int topRight = OnConvertV2ToIndexRequested(clampedRange.max);

                // �켱 ���� ���� ���콺 ������ �ֺ� �ε����� �����ؼ� �� �κ��� Filled �Ǿ��ִ��� �Ǵ��غ���
                // �׷��ٸ� �Ʒ� �ڵ� �������ֱ�

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

                // ���콺�� ���� ����� ������ ��ȯ�ؼ� �� ��ġ�� spawnRect�� ��ġ��Ŵ
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

        // ���� ���� �׸��� ����� �޾Ƽ� ���콺�� �̵��� �� �ִ� �ּ�, �ִ� ��ǥ�� ���غ���
        // ���� ������Ʈ�� ���콺 �̵� �Է��� �޾ƺ��� �ɷ� �����ϰ� �̵��� int �����θ� �ϴ� �ɷ� ����

        // ���콺 ��ǥ�� �ݿø� �ؼ� int�� �ޱ�
        // ���⿡ ������ �ɱ� ������Ʈ�� ������ �Ѱܼ� �ʱ�ȭ���ְ� �� ���� �������� �ٴ� �׸��带 �׷�����

        // ��������� �ؾ��� �κ��� ��ġ ���� �Ѱ��ְ� �ٴ� �׸��� �׷��ִ� ����� �߰��ϸ� ��
    }
}