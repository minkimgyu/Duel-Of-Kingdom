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
        // 여기서 원형, 사각형 모양 지정해보기
        public void Initialize(DrawingData drawingData)
        {
            _vertices = drawingData.Vertices;
            _orders = drawingData.Orders;
        }

        void ResetVertices()
        {
            int _steps = 50;
            float radius = 1;
            List<Vector3> vertices = new List<Vector3>();

            for (int currentStep = 0; currentStep <= _steps; currentStep++)
            {
                float circumferenceProgress = (float)currentStep / _steps;
                float currentRadian = circumferenceProgress * 2 * Mathf.PI;

                float xScaled = Mathf.Cos(currentRadian);
                float yScaled = Mathf.Sin(currentRadian);

                float x = xScaled * radius;
                float y = yScaled * radius;

                vertices.Add(new Vector3(x, 0, y));
            }


        }

        void DrawPoint()
        {

        }

        public void Initialize()
        {
            //_vertices = drawingData.Vertices;
            //_orders = drawingData.Orders;
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
        public void Erase()
        {
            _meshFilter.mesh.Clear(); // mesh 초기화
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