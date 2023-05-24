using System;
using UnityEngine;
using UnityEditor;

namespace SanyoniLib.Bezier.Editor
{
    [CustomEditor(typeof(Bezier))]
    public class BezierEditor : UnityEditor.Editor
    {
        private Bezier Bezier;

        private SerializedProperty PointsProp;
        private SerializedProperty bConstraintPointsYProp;
        private SerializedProperty PreviewSegmentColorProp;
        private SerializedProperty PreviewSegmentCountProp;
        private SerializedProperty PreviewSegmentWidthProp;

        private static bool bShowPoints = true;

        private void OnEnable()
        {
            Bezier = (Bezier)target;

            PointsProp = serializedObject.FindProperty("Points");
            bConstraintPointsYProp = serializedObject.FindProperty("bConstraintPointsY");
            PreviewSegmentColorProp = serializedObject.FindProperty("PreviewSegmentColor");
            PreviewSegmentCountProp = serializedObject.FindProperty("PreviewSegmentCount");
            PreviewSegmentWidthProp = serializedObject.FindProperty("PreviewSegmentWidth");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // EditorGUILayout.PropertyField(PointsProp);
            EditorGUILayout.PropertyField(bConstraintPointsYProp);
            EditorGUILayout.PropertyField(PreviewSegmentColorProp);
            EditorGUILayout.PropertyField(PreviewSegmentCountProp);
            EditorGUILayout.PropertyField(PreviewSegmentWidthProp);

            bShowPoints = EditorGUILayout.Foldout(bShowPoints, "Bezier Points");
            if (bShowPoints)
            {
                DrawInspectorPointArray();
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void OnSceneGUI()
        {
            if (Bezier == null || Bezier.IsValid() == false)
                return;
            
            // Bezier Point를 표시합니다.
            DrawSceneBezierPoints(Bezier);
        }

        public static void DrawSceneBezierPoints(Bezier _bezier)
        {
            foreach (var _point in _bezier.GetBezierPoints())
                BezierPointEditor.DrawScenePoint(_point);
        }

        private void DrawInspectorPointArray()
        {
            BezierPoint[] _bezierPoints = Bezier.GetBezierPoints();
            for (int _i = 0; _i < _bezierPoints.Length; _i++)
            {
                if (_bezierPoints[_i] != null)
                    DrawInspectorPoint(_bezierPoints[_i], _i);
            }

            DrawInspectorAddPointButton();
        }

        private void DrawInspectorPoint(BezierPoint _point, int _index)
        {
            SerializedObject _serObj = new SerializedObject(_point);
            SerializedProperty _previewColorProp = _serObj.FindProperty("PreviewColor");

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                Undo.SetCurrentGroupName("Remove Bezier Point");

                // 배열에서 index에 위치한 Bezier Point를 삭제합니다.
                Undo.RegisterCompleteObjectUndo(Bezier, "Bezier");
                Bezier.RemovePointAt(_index);
                EditorUtility.SetDirty(Bezier);

                // Bezier Point 게임 오브젝트를 삭제합니다.
                Undo.DestroyObjectImmediate(_point.gameObject);

                return;
            }

            EditorGUILayout.ObjectField(_point.gameObject, typeof(GameObject), true);

            if (_index != 0 && GUILayout.Button("▲", GUILayout.Width(20)))
            {
                Undo.SetCurrentGroupName("Swap Bezier Point Order");

                UnityEngine.Object _other = PointsProp.GetArrayElementAtIndex(_index - 1).objectReferenceValue;
                PointsProp.GetArrayElementAtIndex(_index - 1).objectReferenceValue = _point;
                PointsProp.GetArrayElementAtIndex(_index).objectReferenceValue = _other;
            }

            if (_index != PointsProp.arraySize - 1 && GUILayout.Button("▼", GUILayout.Width(20)))
            {
                Undo.SetCurrentGroupName("Swap Bezier Point Order");

                UnityEngine.Object _other = PointsProp.GetArrayElementAtIndex(_index + 1).objectReferenceValue;
                PointsProp.GetArrayElementAtIndex(_index + 1).objectReferenceValue = _point;
                PointsProp.GetArrayElementAtIndex(_index).objectReferenceValue = _other;
            }

            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                _serObj.ApplyModifiedProperties();
                EditorUtility.SetDirty(_serObj.targetObject);
            }
        } // end DrawInspectorPoint()

        private void DrawInspectorAddPointButton()
        {
            if (GUILayout.Button("Add Point"))
            {
                Undo.RegisterSceneUndo("Add Point");

                GameObject pointObject = new GameObject("Point " + PointsProp.arraySize);
                pointObject.transform.parent = Bezier.transform;
                pointObject.transform.position = Vector3.zero;

                BezierPoint newPoint = pointObject.AddComponent<BezierPoint>();
                newPoint.Initialize(Bezier);

                PointsProp.InsertArrayElementAtIndex(PointsProp.arraySize);
                PointsProp.GetArrayElementAtIndex(PointsProp.arraySize - 1).objectReferenceValue = newPoint;
            }
        }
    }
}