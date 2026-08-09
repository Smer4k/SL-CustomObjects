#if UNITY_EDITOR
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomEditor(typeof(CullingZoneComponent))]
    [CanEditMultipleObjects]
    public sealed class CullingZoneEditor : Editor
    {
        private SerializedProperty _boxSize;
        private SerializedProperty _center;
        private SerializedProperty _sphereRadius;
        private SerializedProperty _capsuleRadius;
        private SerializedProperty _capsuleHeight;
        private SerializedProperty _type;

        private BoxBoundsHandle _boxHandle;

        private void OnEnable()
        {
            _boxHandle = new BoxBoundsHandle();
            _boxSize = serializedObject.FindProperty(nameof(CullingZoneComponent.BoxSize));
            _center = serializedObject.FindProperty(nameof(CullingZoneComponent.Center));
            _sphereRadius = serializedObject.FindProperty(nameof(CullingZoneComponent.SphereRadius));
            _capsuleRadius = serializedObject.FindProperty(nameof(CullingZoneComponent.CapsuleRadius));
            _capsuleHeight = serializedObject.FindProperty(nameof(CullingZoneComponent.CapsuleHeight));
            _type = serializedObject.FindProperty(nameof(CullingZoneComponent.Type));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(
                serializedObject,
                nameof(CullingZoneComponent.BoxSize),
                nameof(CullingZoneComponent.Center),
                nameof(CullingZoneComponent.SphereRadius),
                nameof(CullingZoneComponent.CapsuleHeight),
                nameof(CullingZoneComponent.CapsuleRadius));

            GUILayout.Space(15f);
            
            if (_type.intValue == (int)ColliderShape.Box)
            {
                EditorGUILayout.PropertyField(_boxSize);
                EditorGUILayout.PropertyField(_center);
            }
            else if (_type.enumValueFlag == (int)ColliderShape.Sphere)
            {
                EditorGUILayout.PropertyField(_sphereRadius);
            }
            else
            {
                EditorGUILayout.PropertyField(_capsuleRadius);
                EditorGUILayout.PropertyField(_capsuleHeight);
            }

            serializedObject.ApplyModifiedProperties();

            var count = 0;
            var countZones = 0;
            foreach (var o in targets)
            {
                if (o is CullingZoneComponent cullingZone)
                {
                    count += cullingZone.GetComponentsInChildren<SchematicBlock>().Length - 1;
                    countZones++;
                }
            }
            GUILayout.Space(5f);
            if (countZones > 1)
            {
                GUILayout.Label(
                    $"<color=white>Selected zones: <b>{countZones}</b></color>",
                    SchematicManager.UnityRichTextStyle);  
            }
            
            GUILayout.Label(
                $"<color=white>Number of blocks: <b>{count}</b></color>",
                SchematicManager.UnityRichTextStyle);
        }

        private void OnSceneGUI()
        {
            if (Tools.current == Tool.Scale)
                Tools.current = Tool.Move;

            var zone = (CullingZoneComponent)target;
            if (zone == null)
                return;
            
            var t = zone.transform;

            PrimitiveDrawer.LockScale(t);

            using (new Handles.DrawingScope(t.localToWorldMatrix))
            {
                switch (zone.Type)
                {
                    case ColliderShape.Box:
                        PrimitiveDrawer.DrawBox(_boxHandle, zone, ref zone.Center, ref zone.BoxSize);
                        break;

                    case ColliderShape.Sphere:
                        PrimitiveDrawer.DrawSphere(zone, ref zone.SphereRadius);
                        break;

                    case ColliderShape.Capsule:
                        PrimitiveDrawer.DrawCapsule(zone, ref zone.CapsuleRadius, ref zone.CapsuleHeight);
                        break;
                }
            }
        }
    }
}
#endif