using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SanyoniLib.UnityEngineExtensions
{

    public static class CameraExtensions 
    {
        public static bool IsVisible2D(this UnityEngine.Camera me, GameObject gameObject, bool simpleCheck = true)
        {
            if(simpleCheck == true)
            {
                Vector3 screenPoint = me.WorldToViewportPoint(gameObject.transform.position);
                if (screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) return true;
                else return false;
            }
            else
            { 
                //TODO: 오브젝트의 바운더리 철저하게 검사할 것
                return false;
            }
            

        }

    }

}