using UnityEngine;
using System;

namespace SanyoniLib.Bezier
{
    [Serializable]
    public struct BezierResult
    {
        [SerializeField] private Vector3 Position;
        [SerializeField] private Vector3 Direction;
        [SerializeField] private float Delta;

        private Bezier Bezier;

        public BezierResult(Bezier _bezier, Vector3 position, Vector3 _direction, float _delta)
        {
            this.Bezier = _bezier;
            this.Position = position;
            this.Direction = _direction;
            this.Delta = _delta;
        }

        public Vector3 GetPosition() => Position;
        public Vector3 GetDirection() => Direction;
        public float GetDelta() => Delta;
        public Bezier GetBezier() => Bezier;
    }
}