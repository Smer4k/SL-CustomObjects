using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomEditor(typeof(LightComponent))]
    [CanEditMultipleObjects]
    public class LightComponentEditor : Editor
    {
        private SerializedProperty _flicker;
        private SerializedProperty _cycle;

        private SerializedProperty _randomInRange;

        private SerializedProperty _timeToOn;
        private SerializedProperty _timeToOff;

        private SerializedProperty _maxOn;
        private SerializedProperty _minOn;
        private SerializedProperty _maxOff;
        private SerializedProperty _minOff;

        private void OnEnable()
        {
            _flicker = serializedObject.FindProperty("Flicker");
            _cycle = serializedObject.FindProperty("Cycle");

            _randomInRange = serializedObject.FindProperty("RandomInRange");
            _timeToOn = serializedObject.FindProperty("TimeToOn");
            _timeToOff = serializedObject.FindProperty("TimeToOff");

            _maxOn = serializedObject.FindProperty("MaxOn");
            _minOn = serializedObject.FindProperty("MinOn");

            _maxOff = serializedObject.FindProperty("MaxOff");
            _minOff = serializedObject.FindProperty("MinOff");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(
                serializedObject,
                nameof(LightComponent.RandomInRange),
                nameof(LightComponent.TimeToOn),
                nameof(LightComponent.TimeToOff),
                nameof(LightComponent.MaxOn),
                nameof(LightComponent.MinOn),
                nameof(LightComponent.MaxOff),
                nameof(LightComponent.MinOff),
                nameof(ActionEventHostBlockBase.ActionEvents)
            );

            if (!_flicker.boolValue || !_cycle.boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                GUILayout.Space(6f);
                GUILayout.BeginHorizontal();
                ActionEventEditorWindow.DrawOpenButton(
                    (MonoBehaviour)target,
                    "Open Actions Editor",
                    "Edit Actions",
                    GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
                ActionInfoWindow.DrawOpenButton("Action Info", GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
                GUILayout.EndHorizontal();
                return;
            }

            EditorGUILayout.PropertyField(_randomInRange);

            if (_randomInRange.boolValue)
            {
                EditorGUILayout.PropertyField(_maxOn);
                EditorGUILayout.PropertyField(_minOn);

                EditorGUILayout.PropertyField(_maxOff);
                EditorGUILayout.PropertyField(_minOff);
            }
            else
            {
                EditorGUILayout.PropertyField(_timeToOn);
                EditorGUILayout.PropertyField(_timeToOff);
            }
            
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            ActionEventEditorWindow.DrawOpenButton(
                (MonoBehaviour)target,
                "Open Actions Editor",
                "Light Actions",
                GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
            ActionInfoWindow.DrawOpenButton("Action Info", GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
            GUILayout.EndHorizontal();
        }
    }
}