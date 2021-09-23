using System;
using UnityEngine;

namespace SanyoniLib.WorldRect
{
    [System.Serializable]
    public class WorldRect : MonoBehaviour
    {
        [SerializeField] private WorldRectPoint m_MinPoint;

        [SerializeField] private WorldRectPoint m_MaxPoint;

        [SerializeField] private Color m_PreviewSegmentColor = Color.red;

        public WorldRectPoint GetMinRectPoint() => m_MinPoint;

        public WorldRectPoint GetMaxRectPoint() => m_MaxPoint;

        public Color GetPreviewSegmentColor() => m_PreviewSegmentColor;

        public Vector3 GetSize()
        {
            Vector3 _difference = m_MaxPoint.Position - m_MinPoint.Position;

            return new Vector3(
                Math.Abs(_difference.x),
                Math.Abs(_difference.y),
                Math.Abs(_difference.z)
            );
        }

        public Vector3 GetMinPoint() => m_MinPoint.Position;

        public Vector3 GetMaxPoint() => m_MaxPoint.Position;

        public Vector3 GetCenterPoint()
        {
            return GetMinPoint() + GetSize() / 2;
        }

        public Vector3 GetLeftBottomBackwardPoint() => m_MinPoint.Position;

        public Vector3 GetLeftBottomForwardPoint() => new Vector3(
            m_MinPoint.Position.x, m_MinPoint.Position.y, m_MaxPoint.Position.z
        );

        public Vector3 GetLeftTopBackwardPoint() => new Vector3(
            m_MinPoint.Position.x, m_MaxPoint.Position.y, m_MinPoint.Position.z
        );

        public Vector3 GetLeftTopForwardPoint() => new Vector3(
            m_MinPoint.Position.x, m_MaxPoint.Position.y, m_MaxPoint.Position.z
        );

        public Vector3 GetRightBottomBackwardPoint() => new Vector3(
            m_MaxPoint.Position.x, m_MinPoint.Position.y, m_MinPoint.Position.z
        );

        public Vector3 GetRightBottomForwardPoint() => new Vector3(
            m_MaxPoint.Position.x, m_MinPoint.Position.y, m_MaxPoint.Position.z
        );

        public Vector3 GetRightTopBackwardPoint() => new Vector3(
            m_MaxPoint.Position.x, m_MaxPoint.Position.y, m_MinPoint.Position.z
        );

        public Vector3 GetRightTopForwardPoint() => m_MaxPoint.Position;

        public Vector3 GetRandomPoint()
        {
            Vector3 _size = GetSize();
            Vector3 _randomOffset = new Vector3(
                UnityEngine.Random.Range(0, _size.x),
                UnityEngine.Random.Range(0, _size.y),
                UnityEngine.Random.Range(0, _size.z));

            Vector3 _randomPoint = m_MinPoint.Position + _randomOffset;
            return _randomPoint;
        }
    }
}