using UnityEngine;
using UnityEditor;

namespace SanyoniLib.Bezier
{
    [CustomEditor(typeof(BezierPoint))]
    public class BezierPointEditor : Editor
    {
        private BezierPoint Point;

        private SerializedProperty PreviewColorProp;

        void OnEnable()
        {
            Point = (BezierPoint)target;

            PreviewColorProp = serializedObject.FindProperty("PreviewColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(PreviewColorProp);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        void OnSceneGUI()
        {
            Bezier _bezier = Point.GetBezier();

            if (_bezier == null || _bezier == false)
                return;

            BezierEditor.DrawSceneBezier(_bezier);
            BezierEditor.DrawSceneBezierPoints(_bezier);
        }

        public static void DrawScenePoint(BezierPoint _point)
        {
            Color _originColor = Handles.color;
            Handles.color = _point.GetPreviewColor();

            // Point 게임 오브젝트의 이름을 표시합니다.
            Handles.Label(
                _point.Position + new Vector3(0, HandleUtility.GetHandleSize(_point.Position) * 0.4f, 0),
                _point.gameObject.name);

            // FreeMoveHandle을 표시하고, 드래그했을 때 드래그한 위치로 값을 새로 씁니다.
            Vector3 _newPosition = Handles.FreeMoveHandle(
                _point.transform.position,
                Quaternion.identity,
                HandleUtility.GetHandleSize(_point.transform.position) * 0.2f,
                Vector3.zero,
                Handles.CircleHandleCap);
            _point.Position = _newPosition;

            Handles.color = _originColor;
        }
    }
}