using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using SanyoniLib.SystemHelper;

namespace SanyoniLib.Bezier
{
    [Serializable]
    public class Bezier : MonoBehaviour
    {
        [SerializeField] private BezierPoint[] Points = new BezierPoint[0];
        [SerializeField] private bool bConstraintPointsY = false;
        [SerializeField] private Color PreviewSegmentColor = Color.red;
        [SerializeField] private int PreviewSegmentCount = 50;
        [SerializeField] private float PreviewSegmentWidth = 3f;

        public static Bezier CreateBezier(
            Vector3[] _points,
            int _previewSegmentCount = 50,
            float _previewSegmentWidth = 3f)
        {
            GameObject _newBezierGO = new GameObject("Generated Bezier_" + Guid.NewGuid().ToString());
            Bezier _bezier = _newBezierGO.AddComponent<Bezier>();

            BezierPoint[] _bezierPoints = new BezierPoint[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                GameObject _newPointGO = new GameObject("Point " + (i + 1));
                _newPointGO.transform.parent = _newBezierGO.transform;

                BezierPoint _newPoint = _newPointGO.AddComponent<BezierPoint>();
                _newPoint.transform.position = _points[i];

                _bezierPoints[i] = _newPoint;
            }

            _bezier.Initialize(_bezierPoints, Color.red, _previewSegmentCount, _previewSegmentWidth);
            return _bezier;
        }

        public void Initialize(
            int _previewSegmentCount = 50,
            float _previewSegmentWidth = 3f)
        {
            Initialize(new BezierPoint[0], Color.red, _previewSegmentCount, _previewSegmentWidth);
        }

        public void Initialize(
            BezierPoint[] _points,
            Color _previewSegmentColor,
            int _previewSegmentCount = 50,
            float _previewSegmentWidth = 3f)
        {
            Points = _points;
            PreviewSegmentColor = _previewSegmentColor;
            PreviewSegmentCount = _previewSegmentCount;
            PreviewSegmentWidth = _previewSegmentWidth;
        }

        public void TryConstraintPoints()
        {
            if (bConstraintPointsY)
            {
                float _y = transform.position.y;

                foreach (var _p in Points)
                {
                    _p.transform.position = new Vector3(_p.transform.position.x, _y, _p.transform.position.z);
                    _p.HandleBefore.position = new Vector3(_p.HandleBefore.position.x, _y, _p.HandleBefore.position.z);
                    _p.HandleAfter.position = new Vector3(_p.HandleAfter.position.x, _y, _p.HandleAfter.position.z);
                }
            }
        }

        public bool IsValid()
        {
            return Points.Length >= 2;
        }

        public void RemovePointAt(int _index)
        {
            ArrayHelper.RemoveAt(ref Points, _index);
        }

        public List<Vector3> GetPointsAndHandlesPositions()
        {
            if (Points.Length == 0)
            {
                return new List<Vector3>();
            }
            else if (Points.Length == 1)
            {
                var a = new List<Vector3>();
                a.Add(Points[0].transform.position);
                return a;
            }

            // position list�� �ʱ�ȭ�մϴ�.
            // start point�� before handle��, end point�� after handle�� ������ ���� �����Ƿ�,
            // position ũ��� (point�� �� ���� * 3 - 2) �Դϴ�.
            int _posCount = Points.Length * 3 - 2;
            List<Vector3> _positionList = new List<Vector3>();
            for (int i = 0; i < _posCount; ++i) _positionList.Add(Vector3.zero);


            _positionList[0] = Points[0].transform.position;
            _positionList[1] = Points[0].HandleAfter.position;

            for (int i = 1; i <= Points.Length - 2; ++i)
            {
                _positionList[-1 + i * 3] = Points[i].HandleBefore.position;
                _positionList[-1 + i * 3 + 1] = Points[i].transform.position;
                _positionList[-1 + i * 3 + 2] = Points[i].HandleAfter.position;
            }

            _positionList[_posCount - 2] = Points[Points.Length - 1].HandleBefore.position;
            _positionList[_posCount - 1] = Points[Points.Length - 1].transform.position;

            return _positionList;
        }

        public BezierPoint[] GetBezierPoints() => Points;

        public Color GetPreviewSegmentColor() => PreviewSegmentColor;

        public float GetPreviewSegmentWidth() => PreviewSegmentWidth;

        public Vector3[] GetPreviewSegmentPoints()
        {
            Vector3[] PreviewSegmentPoints = new Vector3[PreviewSegmentCount + 1];
            for (int i = 0; i <= PreviewSegmentCount; i++)
            {
                float _delta = i / (float)PreviewSegmentCount;
                PreviewSegmentPoints[i] = GetPoint(_delta).GetPosition();
            }

            return PreviewSegmentPoints;
        }

        public BezierPoint GetPointBefore(BezierPoint _point)
        {
            Assert.IsTrue(Points.Contains(_point));
            int _index = Array.IndexOf(Points, _point);
            if (_index == 0)
                return null;
            return Points[_index - 1];
        }

        public BezierPoint GetPointAfter(BezierPoint _point)
        {
            Assert.IsTrue(Points.Contains(_point));
            int _index = Array.IndexOf(Points, _point);
            if (_index == Points.Length - 1)
                return null;
            return Points[_index + 1];
        }

        public void SetPoints(in BezierPoint[] _newPoints)
        {
            Points = _newPoints;
        }

        public BezierResult GetPoint(float _delta)
        {
            List<Vector3> _points = GetPointsAndHandlesPositions();

            if (_points.Count == 0)
                return new BezierResult(this, Vector3.zero, Vector3.zero, 0);
            else if (_points.Count == 1)
                return new BezierResult(this, _points[0], Vector3.zero, _delta);

            Vector3 _direction = Vector3.zero;

            while (_points.Count > 1)
            {
                List<Vector3> _newPoints = new List<Vector3>(_points.Count);

                for (int i = 0; i < _points.Count - 1; i++)
                {
                    _direction = _points[i + 1] - _points[i];
                    Vector3 _newPoint = _points[i] + _direction * _delta;

                    _newPoints.Add(_newPoint);
                }

                _points = _newPoints;
            }

            return new BezierResult(this, _points[0], _direction.normalized, _delta);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Bezier 미리보기 선을 표시합니다.

            Color _originColor = Handles.color;
            Handles.color = GetPreviewSegmentColor();

            var _previewSegmentPoints = GetPreviewSegmentPoints();
            for (int i = 0; i < _previewSegmentPoints.Length - 1; i++)
            {
                Vector3 _start = _previewSegmentPoints[i];
                Vector3 _end = _previewSegmentPoints[i + 1];
                Handles.DrawLine(_start, _end);
            }

            Handles.color = _originColor;
        }

        private void OnValidate()
        {
            TryConstraintPoints();
        }
#endif
    }
}