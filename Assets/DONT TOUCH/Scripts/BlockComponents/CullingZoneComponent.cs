using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace DONT_TOUCH.Scripts.BlockComponents
{
    [ExecuteInEditMode]
    public sealed class CullingZoneComponent : SchematicBlock
    {
        public static readonly Color GizmoColor = new Color(0.2f, 0.6f, 1f, 0.6f);
        public override BlockType BlockType { get; } = BlockType.CullingZone;
        public ColliderShape Type = ColliderShape.Box;
        [Min(0), Tooltip("How many objects will be spawned per frame (0 = all objects instantly)")] public int ObjectPerSpawn = 0;
        [Tooltip("Which zones will be loaded if the player touches this zone")] public List<CullingZoneComponent> ConnectedZones = new();

        public Vector3 BoxSize = Vector3.one;
        public Vector3 Center = Vector3.zero;
        public float SphereRadius = 1f;
        public float CapsuleRadius = 0.5f;
        public float CapsuleHeight = 2f;
        
        public override void Compile(SchematicBlockData block)
        {
            var targetScale = BoxSize;
            if (Type == ColliderShape.Sphere)
            {
                targetScale = new Vector3(SphereRadius, SphereRadius, SphereRadius);
            } else if (Type == ColliderShape.Capsule)
            {
                targetScale = new Vector3(CapsuleRadius, CapsuleHeight, CapsuleRadius);
            }

            var targetCenter = Center;
            if (Type is ColliderShape.Capsule or ColliderShape.Sphere)
                targetCenter = Vector3.zero;

#if UNITY_6000_5_OR_NEWER
            var ids = new List<long>();
#else
            var ids = new List<int>();
#endif
            foreach (var cullingZone in ConnectedZones)
            {
                ids.Add(cullingZone.transform.GetId());
            }
            
            block.Properties = new Dictionary<string, object>()
            {
                { "ColliderShape", Type },
                { nameof(ObjectPerSpawn), ObjectPerSpawn },
                { "ColliderSize", new SerializableVector(targetScale)  },
                { "ColliderCenter", new SerializableVector(targetCenter) },
                { nameof(ConnectedZones), ids },
            };
            
            base.Compile(block);
        }

        public override void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
        {
            CullingZoneComponent cullingZone =
                Create<CullingZoneComponent>("Assets/Resources/Blocks/CullingZone.prefab");
            gameObject = cullingZone.gameObject;
            cullingZone.Type = (ColliderShape)Convert.ToInt32(block.Properties["ColliderShape"]);
            cullingZone.ObjectPerSpawn = Convert.ToInt32(block.Properties["ObjectPerSpawn"]);

            var colliderSize = Vector3.one;
            if (block.Properties.TryGetValue("ColliderSize", out object colliderSizeObj))
            {
                colliderSize = JsonConvert.DeserializeObject<SerializableVector>(colliderSizeObj.ToString());
            }
            if (block.Properties.TryGetValue("ColliderCenter", out object colliderCenterObj))
            {
                cullingZone.Center = JsonConvert.DeserializeObject<SerializableVector>(colliderCenterObj.ToString());
            }

            if (Type == ColliderShape.Capsule)
            {
                cullingZone.CapsuleHeight = colliderSize.y;
                cullingZone.CapsuleRadius = colliderSize.x;
            } else if (Type == ColliderShape.Sphere)
            {
                cullingZone.SphereRadius = colliderSize.x;
            }
            else
            {
                cullingZone.BoxSize = colliderSize;
            }
            
            base.Decompile(ref gameObject, block, parent);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = GizmoColor;
            switch (Type)
            {
                case ColliderShape.Box:
                    var oldMatrix = Gizmos.matrix;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(Center, BoxSize); // теперь координаты локальные
                    Gizmos.matrix = oldMatrix; 
                    break;
                case ColliderShape.Sphere:
                    Gizmos.DrawSphere(transform.position, SphereRadius);
                    break;
            }
        }
    }
}