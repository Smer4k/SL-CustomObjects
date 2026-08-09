using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomEditor(typeof(TriggerComponent))]
    [CanEditMultipleObjects]
    public class TriggerComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(
                serializedObject,
                "m_Script",
                nameof(TriggerComponent.ActionEvents));
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            ActionEventEditorWindow.DrawOpenButton(
                (MonoBehaviour)target,
                "Open Actions Editor",
                "Trigger Actions",
                GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
            ActionInfoWindow.DrawOpenButton("Action Info", GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
            GUILayout.EndHorizontal();
        }
    }
}
