using UnityEngine;

namespace SanyoniLib
{
    public class HueLoopingAnimation : MonoBehaviour
    {
        [SerializeField] private bool m_EnableLoop = true;

        [SerializeField] private float m_CycleTime = 2f;

        private float m_StartTime;

        private float m_RandomOffset;

        private void Start()
        {
            m_StartTime = Time.time;
            m_RandomOffset = Random.Range(0f, 360f) / 360f;
        }

        public Color GetColor(Color _originColor)
        {
            if (m_EnableLoop == false)
                return _originColor;

            float _originHue;
            float _originSaturation;
            float _originValue;
            Color.RGBToHSV(_originColor, out _originHue, out _originSaturation, out _originValue);

            float _delta = ((Time.time - m_StartTime + m_RandomOffset) % m_CycleTime) / m_CycleTime;

            return Color.HSVToRGB(_delta, _originSaturation, _originValue);
        }
    }
}