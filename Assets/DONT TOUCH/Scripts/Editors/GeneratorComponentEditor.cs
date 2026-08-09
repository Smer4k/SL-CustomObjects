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
            DrawDefaultInspector();
            var generator = (GeneratorComponent)target;
            GUILayout.Label(
                $"<color=gray>Dropdown Speed: {generator.TotalActivationTime / generator.TotalDeactivationTime}</color>",
                SchematicManager.UnityRichTextStyle);
        }
    }
}