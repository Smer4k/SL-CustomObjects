using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomEditor(typeof(GeneratorComponent))]
    [CanEditMultipleObjects]
    public sealed class GeneratorComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(
                serializedObject,
                nameof(GeneratorComponent.RequiredPermissions));
            
            var permissionProp = serializedObject.FindProperty(nameof(GeneratorComponent.RequiredPermissions));
            permissionProp.intValue = EditorGUILayout.MaskField(
                permissionProp.displayName,
                permissionProp.intValue,
                System.Enum.GetNames(typeof(DoorPermissionFlags)));
            
            serializedObject.ApplyModifiedProperties();
            var generator = (GeneratorComponent)target;
            GUILayout.Label(
                $"<color=white>Dropdown Speed: {generator.TotalActivationTime / generator.TotalDeactivationTime}</color>",
                SchematicManager.UnityRichTextStyle);
        }
    }
}