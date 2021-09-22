using UnityEngine;

namespace SanyoniLib.UnityEngineHelper
{
    public class MathHelper
    {
        public static float GetRotationZWithDirection2D(in Vector2 _direction)
        {
            return Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        }
    }
}