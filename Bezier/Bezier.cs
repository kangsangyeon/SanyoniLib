using System;
using System.Collections.Generic;
using System.Linq;
using SanyoniLib.SystemHelper;
using UnityEngine;

namespace SanyoniLib.Bezier
{
    [Serializable]
    public class Bezier : MonoBehaviour
    {
        [SerializeField] private BezierPoint[] Points = new BezierPoint[0];

        [SerializeField] private Color PreviewSegmentColor = Color.red;

        [SerializeField] private int PreviewSegmentCount = 50;

        [SerializeField] private float PreviewSegmentWidth = 3f;

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

        public bool IsValid()
        {
            return Points.Length >= 2;
        }

        public void RemovePointAt(int _index)
        {
            ArrayHelper.RemoveAt(ref Points, _index);
        }

        public Vector3[] GetPoints()
        {
            return Points.Select(p => p.Position).ToArray();
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
                PreviewSegmentPoints[i] = BezierTool.GetBezierLocationSimple(GetPoints(), _delta);
            }

            return PreviewSegmentPoints;
        }

        public void SetPoints(in BezierPoint[] _newPoints)
        {
            Points = _newPoints;
        }

        public BezierResult GetLocation(Vector3 _currentPosition, float _delta)
        {
            Vector3[] _points = GetPoints();

            if (_points.Length == 0)
                return new BezierResult(this, Vector3.zero, Vector3.zero, _delta, _currentPosition);
            else if (_points.Length == 1)
                return new BezierResult(this, Vector3.zero, _points[0], _delta, _currentPosition);

            List<Vector3> _newPointList = new List<Vector3>(_points);
            Vector3 _direction = Vector3.zero;

            do
            {
                List<Vector3> _newPoints = new List<Vector3>(_newPointList.Count);

                for (int i = 0; i < _newPointList.Count - 1; i++)
                {
                    _direction = _newPointList[i + 1] - _newPointList[i];
                    Vector3 _newPoint = _direction * _delta + _newPointList[i];

                    _newPoints.Add(_newPoint);
                }

                _newPointList = _newPoints;
            } while (_newPointList.Count > 1);

            return new BezierResult(this, _direction.normalized, _newPointList[0], _delta, _currentPosition);
        }
    }
}