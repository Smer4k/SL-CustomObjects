using System.Collections.Generic;
using DONT_TOUCH.Enums;
using UnityEngine;

namespace DONT_TOUCH.Scripts
{
    public static class PrimitiveMeshGetter
    {
        private static readonly Dictionary<PrimitiveType, Mesh> PrimitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

        public static Mesh GetPrimitiveMesh(PrimitiveType type)
        {
            if (!PrimitiveMeshes.ContainsKey(type))
            {
                CreatePrimitiveMesh(type);
            }
            return PrimitiveMeshes[type];
        }

        public static Mesh GetPrimitiveMesh(ColliderShape type)
        {
            PrimitiveType primitiveType = type switch
            {
                ColliderShape.Sphere => PrimitiveType.Sphere,
                ColliderShape.Box => PrimitiveType.Cube,
                ColliderShape.Capsule => PrimitiveType.Capsule,
                _ => PrimitiveType.Sphere
            };
            return CreatePrimitiveMesh(primitiveType);
        }

        private static Mesh CreatePrimitiveMesh(PrimitiveType type)
        {
            GameObject tempGameObject = GameObject.CreatePrimitive(type);
            Mesh mesh = tempGameObject.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(tempGameObject);

            PrimitiveMeshes[type] = mesh;
            return mesh;
        }
    }
}