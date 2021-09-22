using System;
using UnityEngine;

namespace SanyoniLib.Component
{
    public class FloatingMovement : MonoBehaviour
    {
        [Serializable]
        public enum MovementMethod
        {
            Sin,
            Cos
        }

        [SerializeField] private Vector3 LocalOffset = new Vector3(0f, 50f, 0f);

        [SerializeField] private MovementMethod Method = MovementMethod.Sin;

        [SerializeField] private float CycleSpeed = 1f;

        private Vector3 OriginalLocalPosition;

        private void Start()
        {
            OriginalLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            CycleSpeed = Mathf.Clamp(CycleSpeed, 0.1f, float.MaxValue);

            float _delta = Time.time * CycleSpeed;
            float _multiplier = (Method == MovementMethod.Sin ? Mathf.Sin(_delta)
                    : Method == MovementMethod.Cos ? Mathf.Cos(_delta)
                    : 0)
                * 0.5f + 0.5f;

            Vector3 _movement = LocalOffset * _multiplier;

            transform.localPosition = OriginalLocalPosition + _movement;
        }
    }
}