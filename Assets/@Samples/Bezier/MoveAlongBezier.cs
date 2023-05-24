using System;
using SanyoniLib.Bezier;
using UnityEngine;

public class MoveAlongBezier : MonoBehaviour
{
    [SerializeField] private Bezier m_Bezier;
    [SerializeField] private float m_Duration = 4f;

    private void Update()
    {
        float _delta = (Time.time % m_Duration) / m_Duration;
        var _point = m_Bezier.GetPoint(_delta);
        transform.rotation = Quaternion.LookRotation(_point.GetDirection(), Vector3.forward);
        transform.position = _point.GetPosition();
    }
}