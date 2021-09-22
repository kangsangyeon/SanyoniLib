using UnityEngine;

namespace SanyoniLib.UI
{

    public class UITransformTracker : MonoBehaviour
    {
        public Vector2 m_Offset = new Vector2(0f, 120f);
        public float m_LerpSpeed = .5f;
        public Transform m_Target;

        private Vector3 m_TargetPosition;

        private void LateUpdate()
        {
            if (m_Target != null)
                m_TargetPosition = m_Target.position;

            Vector3 desiredPosition = Camera.main.WorldToScreenPoint(m_TargetPosition) + new Vector3(m_Offset.x, m_Offset.y);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, m_LerpSpeed);
        }

    }

}
