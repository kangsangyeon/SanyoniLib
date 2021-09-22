using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace SanyoniLib.Bezier
{
    public static class BezierTool
    {
        public static Vector3 GetBezierLocationSimple(in Vector3[] _points, in float _delta)
        {
            if (_points.Length == 0)
                return Vector3.zero;
            else if (_points.Length == 1)
                return _points[0];

            List<Vector3> _newPointList = new List<Vector3>(_points);
            do
            {
                List<Vector3> _newPoints = new List<Vector3>(_newPointList.Count);

                for (int i = 0; i < _newPointList.Count - 1; i++)
                {
                    Vector3 _direction = _newPointList[i + 1] - _newPointList[i];
                    Vector3 _newPoint = _direction * _delta + _newPointList[i];

                    _newPoints.Add(_newPoint);
                }

                _newPointList = _newPoints;
            } while (_newPointList.Count > 1);

            return _newPointList[0];
        }
    }
}