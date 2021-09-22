using System;
using UnityEngine;

namespace SanyoniLib.Bezier
{
    [Serializable]
    public class BezierPoint : MonoBehaviour
    {
        [SerializeField] private Color PreviewColor = Color.green;

        private Bezier m_Bezier;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Color GetPreviewColor() => PreviewColor;

        public Bezier GetBezier()
        {
            if (m_Bezier)
                return m_Bezier;

            m_Bezier = transform.GetComponentInParent<Bezier>();
            return m_Bezier;
        }
    }
}