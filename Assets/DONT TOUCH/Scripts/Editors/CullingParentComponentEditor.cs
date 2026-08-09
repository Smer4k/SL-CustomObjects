using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomEditor(typeof(CullingParentComponent))]
    [CanEditMultipleObjects]
    public sealed class CullingParentComponentEditor : Editor
    {
        private BoxBoundsHandle _boxHandle;
        
        private void OnEnable()
        {
            _boxHandle = new BoxBoundsHandle();
        }
        
        public override void OnInspectorGUI()
        {
            var cullingParent = (CullingParentComponent)target;
            DrawDefaultInspector();

            GUILayout.Label(
                $"<color=white>Number of blocks: <b>{cullingParent.GetComponentsInChildren<SchematicBlock>().Length - 1}</b></color>",
                SchematicManager.UnityRichTextStyle);
        }
        
        private void OnSceneGUI()
        {
            if (Tools.current == Tool.Scale)
                Tools.current = Tool.Move;

            var cullingParent = (CullingParentComponent)target;
            if (cullingParent == null)
                return;
            
            var t = cullingParent.transform;

            PrimitiveDrawer.LockScale(t);
            
            var matrix = Matrix4x4.TRS(t.position, Quaternion.identity, Vector3.one);
            
            using (new Handles.DrawingScope(matrix))
            {
                PrimitiveDrawer.DrawBox(_boxHandle, cullingParent, ref cullingParent.BoundsSize);
            }
        }
    }
}