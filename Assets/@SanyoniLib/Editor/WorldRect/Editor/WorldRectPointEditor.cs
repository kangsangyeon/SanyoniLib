using UnityEditor;
using UnityEngine;

namespace SanyoniLib.WorldRect
{
    [CustomEditor(typeof(WorldRectPoint))]
    public class WorldRectPointEditor : Editor
    {
        private WorldRectPoint m_Point;

        private SerializedProperty m_PreviewColorProp;

        void OnEnable()
        {
            m_Point = (WorldRectPoint)target;

            m_PreviewColorProp = serializedObject.FindProperty("m_PreviewColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_PreviewColorProp);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        void OnSceneGUI()
        {
            WorldRect _rect = m_Point.GetRect();

            if (_rect == null)
                return;

            WorldRectEditor.DrawSceneSegments(_rect);
            WorldRectEditor.DrawScenePoints(_rect);
        }

        public static void DrawScenePoint(WorldRectPoint _point)
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
                HandleUtility.GetHandleSize(_point.transform.position) * 0.1f,
                Vector3.zero,
                Handles.CircleHandleCap);

            Undo.RecordObject(_point.transform, "Move World Rect Point");
            _point.Position = _newPosition;

            Handles.color = _originColor;
        }
    }
}