using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomEditor(typeof(InteractableComponent))]
    public class InteractableComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(
                serializedObject,
                nameof(InteractableComponent.ActionEvents),
                nameof(InteractableComponent.Permissions));
            
            var permissionProp = serializedObject.FindProperty(nameof(InteractableComponent.Permissions));
            permissionProp.intValue = EditorGUILayout.MaskField(
                permissionProp.displayName,
                permissionProp.intValue,
                System.Enum.GetNames(typeof(DoorPermissionFlags)));
            
            serializedObject.ApplyModifiedProperties();

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            ActionEventEditorWindow.DrawOpenButton(
                (MonoBehaviour)target,
                "Open Actions Editor",
                "Interactable Actions",
                GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
            ActionInfoWindow.DrawOpenButton("Action Info", GUILayout.Height(EditorGUIUtility.singleLineHeight + 2f));
            GUILayout.EndHorizontal();
        }
    }
}
