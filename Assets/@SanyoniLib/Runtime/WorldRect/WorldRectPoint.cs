using UnityEngine;

namespace SanyoniLib.WorldRect
{
    [System.Serializable]
    public class WorldRectPoint : MonoBehaviour
    {
        [SerializeField] private Color m_PreviewColor = Color.green;

        private WorldRect m_Rect;

        public Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                GetRect().ConstrainMinAndMax();
            }
        }

        public Color GetPreviewColor() => m_PreviewColor;

        public WorldRect GetRect()
        {
            if (m_Rect)
                return m_Rect;

            m_Rect = transform.GetComponentInParent<WorldRect>();
            return m_Rect;
        }
    }
}