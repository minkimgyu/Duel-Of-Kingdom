using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPP.DRAWING
{
    public struct DrawingData
    {
        Vector3[] _vertices;
        public Vector3[] Vertices { get { return _vertices; } }

        int[] _orders;
        public int[] Orders { get { return _orders; } }

        public DrawingData(Vector3[] vertice, int[] order)
        {
            _vertices = vertice;
            _orders = order;
        }

        public void Draw(MeshFilter filter)
        {
            Mesh mesh = new Mesh();

            mesh.vertices = _vertices;
            mesh.triangles = _orders;
            filter.mesh = mesh;
        }
    }

    public class SpawnAreaDrawer : MonoBehaviour
    {
        LineRenderer _lineRenderer;
        MeshFilter _meshFilter;

        [SerializeField] float _lineWidth = 0.1f;
        Vector3[] _vertices;
        int[] _orders;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _lineRenderer = GetComponent<LineRenderer>();
        }

        // 여기서 변수 초기화 해주기
        public void Initialize(DrawingData drawingData)
        {
            _vertices = drawingData.Vertices;
            _orders = drawingData.Orders;
            Erase(); // 초기화와 함께 지워주기
        }

        public void Draw()
        {
            Mesh mesh = new Mesh();

            Vector3[] verticesToArr = _vertices;

            mesh.vertices = verticesToArr;
            mesh.triangles = _orders;
            _meshFilter.mesh = mesh;

            DrawLine(verticesToArr);
        }

        public void Move(Vector3 pos)
        {
            transform.position = pos;
        }

        public void Move(Vector3 pos, Vector3 offset)
        {
            transform.position = pos + offset;
        }

        // 모든 점, 선을 지워주기
        void Erase()
        {
            _meshFilter.mesh = new Mesh(); // mesh 초기화
            _lineRenderer.positionCount = 0;
        }

        void SetLinePosition(int index, Vector3 point)
        {
            _lineRenderer.SetPosition(index,
                   new Vector3(point.x, point.y, point.z));
        }

        void DrawLine(Vector3[] vertices)
        {
            _lineRenderer.startWidth =_lineRenderer.endWidth = _lineWidth;
            _lineRenderer.positionCount = vertices.Length + 1;

            for (int i = 0; i < vertices.Length; i++)
            {
                SetLinePosition(i, vertices[i]);
            }

            SetLinePosition(vertices.Length, vertices[0]);
        }
    }
}