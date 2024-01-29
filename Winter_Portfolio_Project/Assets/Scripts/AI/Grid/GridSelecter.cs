using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.DRAWING;
using System;

namespace WPP.GRID
{
    public struct OffsetFromCenter
    {
        int _top, _down, _left, _right;

        public int Top { get { return _top; } }
        public int Down { get { return _down; } }
        public int Left { get { return _left; } }
        public int Right { get { return _right; } }

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
        public Func<Grid[,]> OnReturnGridRequested;
        public Func<RectInt> OnReturnGridRectRequested;

        RectInt _rectSize;
        Vector2Int _centerOffset;

        [SerializeField] Vector2Int minV2;
        [SerializeField] Vector2Int maxV2;

        Vector3 _spawnPoint;

        private void Start()
        {
            GridStorage gridStorage = GetComponent<GridStorage>();
            if (gridStorage == null) return;
            
            OnReturnGridRequested = gridStorage.ReturnGridArray;
            OnReturnGridRectRequested = gridStorage.ReturnGridRect;

            //RectInt rect = OnReturnGridRectRequested();

            Initialize();
            DrawArea(_rectSize);
        }

        public Vector3 ReturnSpawnPoint() { return new Vector3(_spawnPoint.x, 0, _spawnPoint.z); }

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
                gridWorldPos.min.x + offset.Left, 
                gridWorldPos.min.y + offset.Down, 
                gridWorldPos.width - offset.Left - offset.Right,
                gridWorldPos.height - offset.Top - offset.Down);
        }

        Vector3 ReturnClampedPos(Vector3 pos)
        {
            RectInt clampedRange = ReturnClampedRange();
            float clampedX = Mathf.Clamp(pos.x, clampedRange.min.x, clampedRange.max.x);
            float clampedZ = Mathf.Clamp(pos.z, clampedRange.min.y, clampedRange.max.y);
            return new Vector3(clampedX, 1.1f, clampedZ);
        }

        Vector2Int ReturnIndexOfGrid(Vector2 pos)
        {
            RectInt gridWorldPos = OnReturnGridRectRequested();
            return new Vector2Int((int)pos.x - gridWorldPos.min.x, (int)pos.y - gridWorldPos.min.y);
        }

        Vector2Int ReturnIndexOfGrid(Vector3 pos)
        {
            RectInt gridWorldPos = OnReturnGridRectRequested();
            return new Vector2Int((int)pos.x - gridWorldPos.min.x, (int)pos.z - gridWorldPos.min.y);
        }

        private void Update()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 clampedPos = ReturnClampedPos(hit.transform.position);
                //_spawnRect.Move(new Vector3(clampedPos.x - 0.5f, 1.1f, clampedPos.z - 0.5f), new Vector3(-_centerOffset.x, 0, -_centerOffset.y));

                Vector2Int indexOfGrid = ReturnIndexOfGrid(clampedPos);

                int leftX = _centerOffset.x;
                int downY = _centerOffset.y;

                int rightX = _rectSize.width - leftX - 1;
                int topY = _rectSize.height - downY - 1;

                int minXIndex = indexOfGrid.x - leftX;
                int minYIndex = indexOfGrid.y - downY;
                minV2 = new Vector2Int(minXIndex, minYIndex);

                int maxXIndex = indexOfGrid.x + rightX;
                int maxYIndex = indexOfGrid.y + topY;
                maxV2 = new Vector2Int(maxXIndex, maxYIndex);

                // �׸����� �ִ� �ּ� �� �ε���
                RectInt clampedRange = ReturnClampedRange();

                Vector2Int downLeft = ReturnIndexOfGrid(clampedRange.min);
                Vector2Int topRight = ReturnIndexOfGrid(clampedRange.max);

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
                        for (int z = i - downY; z <= i + topY; z++)
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

                    if(thisRectIsBlock == false)
                    {
                        points.Add(new Vector3(clampedPos.x, 1.1f, i - 20));

                        //Vector3 changedPos = new Vector3(clampedPos.x - 0.5f, 1.1f, i - 20 - 0.5f);
                        //_spawnRect.Move(changedPos, new Vector3(-_centerOffset.x, 0, -_centerOffset.y));
                    }
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
                        if(tmpDistance < storedDistance)
                        {
                            storedDistance = tmpDistance;
                            selectedIndex = i;
                        }
                    }
                }

                _spawnPoint = points[selectedIndex];

                Vector3 changedPos = points[selectedIndex] + new Vector3(-0.5f, 0, -0.5f);
                _spawnRect.Move(changedPos, new Vector3(-_centerOffset.x, 0, -_centerOffset.y));
            }
        }

        // ���� ���� �׸��� ����� �޾Ƽ� ���콺�� �̵��� �� �ִ� �ּ�, �ִ� ��ǥ�� ���غ���
        // ���� ������Ʈ�� ���콺 �̵� �Է��� �޾ƺ��� �ɷ� �����ϰ� �̵��� int �����θ� �ϴ� �ɷ� ����

        // ���콺 ��ǥ�� �ݿø� �ؼ� int�� �ޱ�
        // ���⿡ ������ �ɱ� ������Ʈ�� ������ �Ѱܼ� �ʱ�ȭ���ְ� �� ���� �������� �ٴ� �׸��带 �׷�����

        // ��������� �ؾ��� �κ��� ��ġ ���� �Ѱ��ְ� �ٴ� �׸��� �׷��ִ� ����� �߰��ϸ� ��
    }
}