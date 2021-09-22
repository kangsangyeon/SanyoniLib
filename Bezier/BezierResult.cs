using UnityEngine;
using System;

namespace SanyoniLib.Bezier
{
    [Serializable]
    public struct BezierResult
    {
        [SerializeField] private Vector3 Direction;

        [SerializeField] private Vector3 Point;

        [SerializeField] private float Delta;

        [SerializeField] private float DistanceFromOrigin;

        [SerializeField] private Vector3 OriginPoint;

        private Bezier Bezier;

        public BezierResult(Bezier _bezier, Vector3 _direction, Vector3 _point, float _delta, Vector3 _originPoint)
        {
            this.Bezier = _bezier;
            this.Direction = _direction;
            this.Point = _point;
            this.Delta = _delta;
            this.OriginPoint = _originPoint;

            this.DistanceFromOrigin = (_originPoint - _point).magnitude;
        }

        public Vector3 GetDirection() => Direction;

        public Vector3 GetPoint() => Point;

        public float GetDelta() => Delta;

        public Vector3 GetOriginPoint() => OriginPoint;

        public float GetDistanceFromOrigin() => DistanceFromOrigin;
    
        public Bezier GetBezier() => Bezier;
    }    
}
