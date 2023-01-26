using System;
using UnityEngine;

namespace SanyoniLib.Bezier
{
    [Serializable]
    public class BezierPoint : MonoBehaviour
    {
        public enum EType
        {
            Curve,
            Line
        }

        [SerializeField] private Color PreviewPointColor = Color.green;
        [SerializeField] private Color PreviewHandleColor = Color.gray;
        [SerializeField] private Color PreviewHandleLineColor = Color.black;

        private Bezier m_Bezier;

        [SerializeField] private EType m_Type;

        [SerializeField] private Transform m_HandleBefore;
        [SerializeField] private Transform m_HandleAfter;

        public void OnEnable()
        {
            if (m_Bezier != null)
                Initialize(m_Bezier);
        }

        public void Initialize(Bezier _bezier)
        {
            if (m_HandleBefore == null)
            {
                GameObject _go = new GameObject("HandleBefore");
                _go.transform.position = transform.position + Vector3.left * .5f;
                _go.transform.parent = this.transform;

                m_HandleBefore = _go.transform;
            }

            if (m_HandleAfter == null)
            {
                GameObject _go = new GameObject("HandleAfter");
                _go.transform.position = transform.position + Vector3.right * .5f;
                _go.transform.parent = this.transform;

                m_HandleAfter = _go.transform;
            }
        }

        public EType Type
        {
            get => m_Type;
            set
            {
                m_Type = value;
                if (m_Type == EType.Line)
                {
                    m_HandleBefore.localPosition = Vector3.zero;
                    m_HandleAfter.localPosition = Vector3.zero;
                }
            }
        }

        public Transform HandleBefore
        {
            get => m_HandleBefore;
            set => m_HandleBefore = value;
        }

        public Transform HandleAfter
        {
            get => m_HandleAfter;
            set => m_HandleAfter = value;
        }

        public Color GetPreviewPointColor() => PreviewPointColor;
        public Color GetPreviewHandleColor() => PreviewHandleColor;
        public Color GetPreviewHandleLineColor() => PreviewHandleLineColor;

        public Bezier GetBezier()
        {
            if (m_Bezier)
                return m_Bezier;

            m_Bezier = transform.GetComponentInParent<Bezier>();
            return m_Bezier;
        }
    }
}