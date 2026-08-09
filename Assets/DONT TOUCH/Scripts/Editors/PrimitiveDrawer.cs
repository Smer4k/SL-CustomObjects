#if UNITY_EDITOR
using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    public static class PrimitiveDrawer
    {
        public static void DrawBox(BoxBoundsHandle boundsHandle, MonoBehaviour target, ref Vector3 center, ref Vector3 boxSize)
        {
            Handles.color = Color.cyan;
            
            boundsHandle.center = center;
            boundsHandle.size = boxSize;

            EditorGUI.BeginChangeCheck();
            boundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Resize Culling Zone Box");
                boxSize = boundsHandle.size;
                center = boundsHandle.center;
                EditorUtility.SetDirty(target);
            }
        }
        
        public static void DrawBox(BoxBoundsHandle boundsHandle, MonoBehaviour target, ref Vector3 boxSize)
        {
            Handles.color = Color.cyan;
            
            boundsHandle.center = Vector3.zero;
            boundsHandle.size = boxSize;

            EditorGUI.BeginChangeCheck();
            boundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Resize Culling Zone Box");
                boxSize = boundsHandle.size;
                EditorUtility.SetDirty(target);
            }
        }

        public static void DrawSphere(MonoBehaviour target, ref float sphereRadius)
        {
            float radius = Mathf.Max(0.001f, sphereRadius);

            EditorGUI.BeginChangeCheck();
            Handles.color = Color.cyan;
            float newRadius = Handles.RadiusHandle(Quaternion.identity, Vector3.zero, radius);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Resize Culling Zone Sphere");
                sphereRadius = Mathf.Max(0.001f, newRadius);
                EditorUtility.SetDirty(target);
            }
        }

        public static void DrawCapsule(MonoBehaviour target, ref float capsuleRadius, ref float capsuleHeight)
        {
            float radius = Mathf.Max(0.01f, capsuleRadius);
            float height = Mathf.Max(radius * 2f, capsuleHeight);

            float cylinderHalf = Mathf.Max(0f, height * 0.5f - radius);

            Vector3 top = Vector3.up * cylinderHalf;
            Vector3 bottom = Vector3.down * cylinderHalf;
            Handles.color = Color.red;
            
            #region Radius

            EditorGUI.BeginChangeCheck();

            float newRadius = Handles.ScaleSlider(
                radius,
                Vector3.right * radius,
                Vector3.right,
                Quaternion.identity,
                HandleUtility.GetHandleSize(Vector3.right * radius) * 0.7f,
                0);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Capsule Radius");

                capsuleRadius = Mathf.Max(0.01f, newRadius);
                capsuleHeight = Mathf.Max(capsuleHeight, capsuleRadius * 2f);

                EditorUtility.SetDirty(target);

                radius = capsuleRadius;
                height = capsuleHeight;
                cylinderHalf = Mathf.Max(0f, height * .5f - radius);

                top = Vector3.up * cylinderHalf;
                bottom = Vector3.down * cylinderHalf;
            }

            #endregion

            #region Height
            
            EditorGUI.BeginChangeCheck();

            Vector3 newTop = Handles.Slider(
                top + Vector3.up * radius,
                Vector3.up,
                HandleUtility.GetHandleSize(top) * .08f,
                Handles.CubeHandleCap,
                0);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Capsule Height");

                capsuleHeight = Mathf.Max(
                    radius * 2f,
                    (newTop.y + radius) * 2f);

                EditorUtility.SetDirty(target);

                height = capsuleHeight;
                cylinderHalf = Mathf.Max(0f, height * .5f - radius);

                top = Vector3.up * cylinderHalf;
                bottom = Vector3.down * cylinderHalf;
            }

            EditorGUI.BeginChangeCheck();

            Vector3 newBottom = Handles.Slider(
                bottom - Vector3.up * radius,
                Vector3.down,
                HandleUtility.GetHandleSize(bottom) * .08f,
                Handles.CubeHandleCap,
                0);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Capsule Height");

                capsuleHeight = Mathf.Max(
                    radius * 2f,
                    (-newBottom.y + radius) * 2f);

                EditorUtility.SetDirty(target);

                height = capsuleHeight;
                cylinderHalf = Mathf.Max(0f, height * .5f - radius);

                top = Vector3.up * cylinderHalf;
                bottom = Vector3.down * cylinderHalf;
            }

            #endregion

            Handles.color = Color.cyan;

            // Верхняя окружность
            Handles.DrawWireDisc(top, Vector3.up, radius);

            // Нижняя окружность
            Handles.DrawWireDisc(bottom, Vector3.up, radius);

            // Вертикали
            Handles.DrawLine(top + Vector3.right * radius, bottom + Vector3.right * radius);
            Handles.DrawLine(top - Vector3.right * radius, bottom - Vector3.right * radius);
            Handles.DrawLine(top + Vector3.forward * radius, bottom + Vector3.forward * radius);
            Handles.DrawLine(top - Vector3.forward * radius, bottom - Vector3.forward * radius);

            // Задняя дуга
            Handles.DrawWireArc(top, Vector3.right, Vector3.back, 180, radius);
            Handles.DrawWireArc(bottom, Vector3.right, Vector3.forward, 180, radius);

            // Вторая боковая дуга
            Handles.DrawWireArc(top, Vector3.forward, Vector3.right, 180, radius);
            Handles.DrawWireArc(bottom, Vector3.forward, Vector3.left, 180, radius);
        }

        public static void LockScale(Transform target)
        {
            if (target == null)
                return;

            if (target.localScale == Vector3.one)
                return;

            Undo.RecordObject(target, "Lock Culling Zone Scale");
            target.localScale = Vector3.one;
            EditorUtility.SetDirty(target);
        }

        public static Vector3 ClampPositive(Vector3 v)
        {
            return new Vector3(
                Mathf.Max(0.001f, Mathf.Abs(v.x)),
                Mathf.Max(0.001f, Mathf.Abs(v.y)),
                Mathf.Max(0.001f, Mathf.Abs(v.z))
            );
        }
    }
}
#endif