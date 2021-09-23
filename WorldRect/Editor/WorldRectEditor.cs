using System;
using UnityEditor;
using UnityEngine;

namespace SanyoniLib.WorldRect
{
    [CustomEditor(typeof(WorldRect))]
    public class WorldRectEditor : Editor
    {
        private WorldRect m_Rect;

        private SerializedProperty m_MaxPointProp;
        private SerializedProperty m_MinPointProp;
        private SerializedProperty m_PreviewSegmentColorProp;

        private void OnEnable()
        {
            m_Rect = (WorldRect)target;

            m_MaxPointProp = serializedObject.FindProperty("m_MaxPoint");
            m_MinPointProp = serializedObject.FindProperty("m_MinPoint");
            m_PreviewSegmentColorProp = serializedObject.FindProperty("m_PreviewSegmentColor");

            bool _bModified = false;

            if (m_MaxPointProp.objectReferenceValue == null)
            {
                GameObject _newMaxPointGO = new GameObject("Max Point");
                _newMaxPointGO.AddComponent<WorldRectPoint>();
                _newMaxPointGO.transform.parent = m_Rect.transform;
                _newMaxPointGO.transform.position = m_Rect.transform.position + new Vector3(50f, 50f, 0f);

                m_MaxPointProp.objectReferenceValue = _newMaxPointGO;
                _bModified = true;
            }

            if (m_MinPointProp.objectReferenceValue == null)
            {
                GameObject _newMinPointGO = new GameObject("Min Point");
                _newMinPointGO.AddComponent<WorldRectPoint>();
                _newMinPointGO.transform.parent = m_Rect.transform;
                _newMinPointGO.transform.position = m_Rect.transform.position + new Vector3(-50f, -50f, 0f);

                m_MinPointProp.objectReferenceValue = _newMinPointGO;
                _bModified = true;
            }

            if (_bModified)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_MaxPointProp);
            EditorGUILayout.PropertyField(m_MinPointProp);
            EditorGUILayout.PropertyField(m_PreviewSegmentColorProp);

            EditorGUILayout.LabelField(String.Format("Size: " + m_Rect.GetSize()));
            EditorGUILayout.LabelField(String.Format("Max: " + m_Rect.GetMaxPoint()));
            EditorGUILayout.LabelField(String.Format("Min: " + m_Rect.GetMinPoint()));
            EditorGUILayout.LabelField(String.Format("Center: " + m_Rect.GetCenterPoint()));

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void OnSceneGUI()
        {
            if (m_Rect == null)
                return;

            // Rect 미리보기 선을 표시합니다.
            DrawSceneSegments(m_Rect);

            // Rect Point를 표시합니다.
            DrawScenePoints(m_Rect);
        }

        public static void DrawSceneSegments(WorldRect _rect)
        {
            Color _originColor = Handles.color;
            Handles.color = _rect.GetPreviewSegmentColor();

            Vector3 _leftBottomBackwardPoint = _rect.GetLeftBottomBackwardPoint();
            Vector3 _leftBottomForwardPoint = _rect.GetLeftBottomForwardPoint();
            Vector3 _leftTopBackwardPoint = _rect.GetLeftTopBackwardPoint();
            Vector3 _leftTopForwardPoint = _rect.GetLeftTopForwardPoint();
            Vector3 _rightBottomBackwardPoint = _rect.GetRightBottomBackwardPoint();
            Vector3 _rightBottomForwardPoint = _rect.GetRightBottomForwardPoint();
            Vector3 _rightTopBackwardPoint = _rect.GetRightTopBackwardPoint();
            Vector3 _rightTopForwardPoint = _rect.GetRightTopForwardPoint();

            // 왼쪽 면을 그립니다.
            Handles.DrawLine(_leftBottomBackwardPoint, _leftBottomForwardPoint);
            Handles.DrawLine(_leftBottomBackwardPoint, _leftTopBackwardPoint);
            Handles.DrawLine(_leftBottomForwardPoint, _leftTopForwardPoint);
            Handles.DrawLine(_leftTopBackwardPoint, _leftTopForwardPoint);

            // 오른쪽 면을 그립니다.
            Handles.DrawLine(_rightBottomBackwardPoint, _rightBottomForwardPoint);
            Handles.DrawLine(_rightBottomBackwardPoint, _rightTopBackwardPoint);
            Handles.DrawLine(_rightBottomForwardPoint, _rightTopForwardPoint);
            Handles.DrawLine(_rightTopBackwardPoint, _rightTopForwardPoint);

            // 정면을 그립니다.
            Handles.DrawLine(_leftBottomBackwardPoint, _rightBottomBackwardPoint);
            Handles.DrawLine(_leftTopBackwardPoint, _rightTopBackwardPoint);

            // 후면을 그립니다.
            Handles.DrawLine(_leftBottomForwardPoint, _rightBottomForwardPoint);
            Handles.DrawLine(_leftTopForwardPoint, _rightTopForwardPoint);

            Handles.color = _originColor;
        }

        public static void DrawScenePoints(WorldRect _rect)
        {
            WorldRectPointEditor.DrawScenePoint(_rect.GetMinRectPoint());
            WorldRectPointEditor.DrawScenePoint(_rect.GetMaxRectPoint());
        }
    }
}