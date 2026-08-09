using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomEditor(typeof(EmptyComponent))]
    [CanEditMultipleObjects]
    public sealed class EmptyComponentEditor : Editor
    {
        private SerializedProperty _damageable;
        private SerializedProperty _health;
        private SerializedProperty _weapons;
        private SerializedProperty _roles;
        private SerializedProperty _explosionTypes;
        
        private void OnEnable()
        {
            _damageable = serializedObject.FindProperty(nameof(EmptyComponent.Damageable));
            _health = serializedObject.FindProperty(nameof(EmptyComponent.Health));
            _weapons = serializedObject.FindProperty(nameof(EmptyComponent.Weapons));
            _explosionTypes = serializedObject.FindProperty(nameof(EmptyComponent.ExplosionTypes));
            _roles = serializedObject.FindProperty(nameof(EmptyComponent.Roles));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(
                serializedObject,
                nameof(InteractableComponent.ActionEvents),
                nameof(EmptyComponent.Health),
                nameof(EmptyComponent.Weapons),
                nameof(EmptyComponent.ExplosionTypes),
                nameof(EmptyComponent.Roles));

            if (!_damageable.boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.PropertyField(_health);
            EditorGUILayout.PropertyField(_explosionTypes);
            EditorGUILayout.PropertyField(_weapons);
            EditorGUILayout.PropertyField(_roles);
            serializedObject.ApplyModifiedProperties();
            
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            ActionEventEditorWindow.DrawOpenButton(
                (MonoBehaviour)target,
                "Open Actions Editor",
                "Damageable object Actions",
                GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
            ActionInfoWindow.DrawOpenButton("Action Info", GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
            GUILayout.EndHorizontal();
        }
    }
}