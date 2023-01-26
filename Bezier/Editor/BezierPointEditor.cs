using UnityEngine;
using UnityEditor;
using SanyoniLib.Bezier;

namespace SanyoniLib.Bezier.Editor
{
    [CustomEditor(typeof(BezierPoint))]
    public class BezierPointEditor : UnityEditor.Editor
    {
        private BezierPoint Point;

        private SerializedProperty PreviewPointColorProp;
        private SerializedProperty PreviewHandleColorProp;
        private SerializedProperty PreviewHandleLineColorProp;
        private SerializedProperty TypeProp;
        private SerializedProperty HandleBeforeProp;
        private SerializedProperty HandleAfterProp;

        void OnEnable()
        {
            Point = (BezierPoint)target;

            PreviewPointColorProp = serializedObject.FindProperty("PreviewPointColor");
            PreviewHandleColorProp = serializedObject.FindProperty("PreviewHandleColor");
            PreviewHandleLineColorProp = serializedObject.FindProperty("PreviewHandleLineColor");
            TypeProp = serializedObject.FindProperty("m_Type");
            HandleBeforeProp = serializedObject.FindProperty("m_HandleBefore");
            HandleAfterProp = serializedObject.FindProperty("m_HandleAfter");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BezierPoint.EType _originType = (BezierPoint.EType)TypeProp.enumValueIndex;

            EditorGUILayout.PropertyField(PreviewPointColorProp);
            EditorGUILayout.PropertyField(PreviewHandleColorProp);
            EditorGUILayout.PropertyField(PreviewHandleLineColorProp);
            EditorGUILayout.PropertyField(TypeProp);
            EditorGUILayout.PropertyField(HandleBeforeProp);
            EditorGUILayout.PropertyField(HandleAfterProp);

            if (GUI.changed)
            {
                BezierPoint.EType _nextType = (BezierPoint.EType)TypeProp.enumValueIndex;

                if (_originType != _nextType
                    && _nextType == BezierPoint.EType.Line)
                {
                    Point.HandleBefore.localPosition = Vector3.zero;
                    Point.HandleAfter.localPosition = Vector3.zero;
                    Point.GetBezier().TryConstraintPoints();
                }

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        void OnSceneGUI()
        {
            Bezier _bezier = Point.GetBezier();

            if (_bezier == null || _bezier == false)
                return;

            BezierEditor.DrawSceneBezierPoints(_bezier);
        }

        public static void DrawScenePoint(BezierPoint _point)
        {
            Color _originColor = Handles.color;
            Handles.color = _point.GetPreviewPointColor();

            // Point 게임 오브젝트의 이름을 표시합니다.
            Handles.Label(
                _point.transform.position + new Vector3(0, HandleUtility.GetHandleSize(_point.transform.position) * 0.4f, 0),
                _point.gameObject.name);

            // FreeMoveHandle을 표시하고, 드래그했을 때 드래그한 위치로 값을 새로 씁니다.
            EditorGUI.BeginChangeCheck();
            Vector3 _newPosition = Handles.FreeMoveHandle(
                _point.transform.position,
                Quaternion.identity,
                HandleUtility.GetHandleSize(_point.transform.position) * 0.2f,
                Vector3.zero,
                Handles.CircleHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_point.transform, "Move Bezier Point");
                _point.transform.position = _newPosition;
                _point.GetBezier().TryConstraintPoints();
            }

            /* Handle을 그립니다. */

            if (_point.GetBezier().GetPointBefore(_point) != null
                && _point.Type == BezierPoint.EType.Curve)
            {
                // Handle Before을 표시하고, 드래그했을 때 드래그한 위치로 값을 새로 씁니다.
                Handles.color = _point.GetPreviewHandleColor();

                EditorGUI.BeginChangeCheck();
                Vector3 _newPosHandleBefore = Handles.FreeMoveHandle(
                    _point.HandleBefore.position,
                    Quaternion.identity,
                    HandleUtility.GetHandleSize(_point.transform.position) * 0.1f,
                    Vector3.zero,
                    Handles.CircleHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(_point.HandleBefore.transform, "Move Bezier's BeforeHandle");
                    _point.HandleBefore.position = _newPosHandleBefore;
                    _point.GetBezier().TryConstraintPoints();
                }

                Handles.color = _point.GetPreviewHandleLineColor();
                Handles.DrawDottedLine(_point.transform.position, _newPosHandleBefore, .2f);
                Handles.Label(_newPosHandleBefore + new Vector3(0, HandleUtility.GetHandleSize(_point.transform.position) * 0.4f, 0), "b");
            }

            if (_point.GetBezier().GetPointAfter(_point) != null
                && _point.Type == BezierPoint.EType.Curve)
            {
                // Handle After을 표시하고, 드래그했을 때 드래그한 위치로 값을 새로 씁니다.
                Handles.color = _point.GetPreviewHandleColor();

                EditorGUI.BeginChangeCheck();
                Vector3 _newPosHandleAfter = Handles.FreeMoveHandle(
                    _point.HandleAfter.position,
                    Quaternion.identity,
                    HandleUtility.GetHandleSize(_point.transform.position) * 0.1f,
                    Vector3.zero,
                    Handles.CircleHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(_point.HandleAfter.transform, "Move Bezier's AfterHandle");
                    _point.HandleAfter.position = _newPosHandleAfter;
                    _point.GetBezier().TryConstraintPoints();
                }

                Handles.color = _point.GetPreviewHandleLineColor();
                Handles.DrawDottedLine(_point.transform.position, _newPosHandleAfter, .2f);
                Handles.Label(_newPosHandleAfter + new Vector3(0, HandleUtility.GetHandleSize(_point.transform.position) * 0.4f, 0), "a");
            }

            Handles.color = _originColor;
        }
    }
}