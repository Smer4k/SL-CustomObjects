using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts;
using DONT_TOUCH.Scripts.BlockComponents;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[System.Serializable]
public class SchematicClusterOptimizerSettings
{
    public List<BlockType> ExcludedBlockTypes = new()
    {
        BlockType.Schematic, BlockType.CullingParent, BlockType.Door, BlockType.Interactable, BlockType.Trigger,
        BlockType.PlayerSpawnPoint, BlockType.Pickup,
        BlockType.Waypoint, BlockType.PlayerBlocker, BlockType.Light, BlockType.CullingZone
    };

    [Min(0f)] public float MaxDistanceForPrimitiveCluster = 2.5f;

    [Min(1)] public int MaxPrimitivesPerCluster = 100;

    [Min(0f)] public float MinimumSizeBeforeBeingBigPrimitive = 10f;
}

public static class SchematicClusterOptimizer
{
    private const string CullingParentPrefabPath = "Assets/Resources/Blocks/CullingParent.prefab";

    public static int Optimize(Schematic schematic)
    {
        if (schematic == null)
            return 0;
        
        SchematicClusterOptimizerSettings settings = schematic.ClusterOptimizer;
        if (settings == null)
            return 0;
        
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Optimize schematic clusters");

        List<ClusterCandidate> candidates = GatherCandidates(schematic.transform, settings);
        if (candidates.Count == 0)
            return 0;

        List<ClusterGroup> groups = BuildGroups(candidates, settings);
        if (groups.Count == 0)
            return 0;
        
        int createdClusters = 0;

        try
        {
            foreach (ClusterGroup group in groups)
            {
                if (group.Members.Count == 0)
                    continue;

                // Step 1: Record members prior to ANY actions
                List<Transform> memberTransforms = new();
                foreach (ClusterCandidate candidate in group.Members)
                {
                    memberTransforms.Add(candidate.Transform);
                }

                // Step 2: Create cluster and apply transform
                CullingParentComponent cluster = SchematicBlock.Create<CullingParentComponent>(CullingParentPrefabPath);
                if (cluster == null)
                    continue;

                GameObject clusterObject = cluster.gameObject;

                // Register object creation directly
                Undo.RegisterCreatedObjectUndo(clusterObject, "Optimize schematic clusters");

                // Safety: Set the parent correctly and let Unity's Undo record this explicitly
                Undo.SetTransformParent(clusterObject.transform, schematic.transform, "Optimize schematic clusters");
                clusterObject.name = $"CullingParent_{createdClusters + 1}";

                // Apply cluster transform based on member bounds
                ApplyClusterTransform(clusterObject.transform, schematic.transform, group.Members);

                // Step 3: Now parent objects into cluster 
                foreach (Transform memberTransform in memberTransforms)
                {
                    Undo.SetTransformParent(memberTransform, clusterObject.transform, "Optimize schematic clusters");
                }

                createdClusters++;
            }
        }
        finally
        {
            Undo.CollapseUndoOperations(undoGroup);
            EditorUtility.SetDirty(schematic);
            EditorSceneManager.MarkSceneDirty(schematic.gameObject.scene);
        }

        return createdClusters;
    }

    private static List<ClusterCandidate> GatherCandidates(Transform schematicRoot,
        SchematicClusterOptimizerSettings settings)
    {
        List<ClusterCandidate> candidates = new();

        void Recurse(Transform node)
        {
            if (node == null || node == schematicRoot || node.TryGetComponent<CullingZoneComponent>(out _))
                return;

            if (TryGetBlockType(node, out BlockType blockType))
            {
                // If this block type is explicitly excluded, skip its entire subtree.
                if (settings.ExcludedBlockTypes.Contains(blockType))
                    return;

                // Empty nodes are just containers: don't add them, but recurse into children.
                if (blockType == BlockType.Empty)
                {
                    foreach (Transform child in node)
                        Recurse(child);

                    return;
                }

                // Otherwise this node represents a block (primitive or other) — add as candidate.
                if (IsBigObject(node, settings.MinimumSizeBeforeBeingBigPrimitive))
                    return;

                if (TryGetHierarchyBounds(node, out Bounds b))
                    candidates.Add(new ClusterCandidate(node, b));

                return; // don't traverse deeper — the block owns its subtree.
            }

            // No SchematicBlock on this transform. If it has renderers/colliders, treat it as candidate,
            // otherwise descend to children and let them be considered.
            bool hasRenderable = node.GetComponentInChildren<Renderer>(true) != null ||
                                 node.GetComponentInChildren<Collider>(true) != null;
            if (hasRenderable)
            {
                if (IsBigObject(node, settings.MinimumSizeBeforeBeingBigPrimitive))
                    return;

                if (TryGetHierarchyBounds(node, out Bounds b))
                    candidates.Add(new ClusterCandidate(node, b));

                return;
            }

            foreach (Transform child in node)
                Recurse(child);
        }

        foreach (Transform child in schematicRoot)
            Recurse(child);

        return candidates;
    }

    private static List<ClusterGroup> BuildGroups(IReadOnlyList<ClusterCandidate> candidates,
        SchematicClusterOptimizerSettings settings)
    {
        List<ClusterGroup> groups = new();
        int maxMembers = Mathf.Max(1, settings.MaxPrimitivesPerCluster);

        foreach (ClusterCandidate candidate in candidates)
        {
            ClusterGroup bestGroup = null;
            float bestDistance = float.MaxValue;

            foreach (ClusterGroup group in groups)
            {
                if (group.MemberCount >= maxMembers)
                    continue;

                float distance = DistanceToBounds(group.Bounds, candidate.Bounds.center);
                if (distance > settings.MaxDistanceForPrimitiveCluster)
                    continue;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestGroup = group;
                }
            }

            if (bestGroup == null)
            {
                bestGroup = new ClusterGroup(candidate.Bounds);
                groups.Add(bestGroup);
            }

            bestGroup.Add(candidate);
        }

        return groups;
    }

    private static void ApplyClusterTransform(Transform clusterTransform, Transform schematicRoot,
        List<ClusterCandidate> members)
    {
        if (members.Count == 0)
            return;

        // Recalculate bounds of all members now that they are children of cluster
        bool hasBounds = false;
        Bounds clusterLocalBounds = default;

        foreach (ClusterCandidate member in members)
        {
            // Get renderer or collider bounds in world space
            Renderer renderer = member.Transform.GetComponentInChildren<Renderer>(true);
            if (renderer != null)
            {
                Bounds rendererBounds = renderer.bounds;
                if (!hasBounds)
                {
                    clusterLocalBounds = rendererBounds;
                    hasBounds = true;
                }
                else
                {
                    clusterLocalBounds.Encapsulate(rendererBounds);
                }
            }

            Collider collider = member.Transform.GetComponentInChildren<Collider>(true);
            if (collider != null)
            {
                Bounds colliderBounds = collider.bounds;
                if (!hasBounds)
                {
                    clusterLocalBounds = colliderBounds;
                    hasBounds = true;
                }
                else
                {
                    clusterLocalBounds.Encapsulate(colliderBounds);
                }
            }

            if (TryGetLightBounds(member.Transform, out Bounds lightBounds))
            {
                if (!hasBounds)
                {
                    clusterLocalBounds = lightBounds;
                    hasBounds = true;
                }
                else
                {
                    clusterLocalBounds.Encapsulate(lightBounds);
                }
            }
        }

        if (!hasBounds)
            return;

        ApplyBounds(clusterTransform, schematicRoot, clusterLocalBounds);
    }

    private static void ApplyBounds(Transform clusterTransform, Transform schematicRoot, Bounds clusterLocalBounds)
    {
        // Add padding to ensure all objects fit with safety margin
        Vector3 padding = new(
            Mathf.Max(clusterLocalBounds.size.x * 0.1f, 0.2f),
            Mathf.Max(clusterLocalBounds.size.y * 0.1f, 0.2f),
            Mathf.Max(clusterLocalBounds.size.z * 0.1f, 0.2f));

        Vector3 worldSize = clusterLocalBounds.size + padding * 2f;
        worldSize = new(
            Mathf.Max(worldSize.x, 0.001f),
            Mathf.Max(worldSize.y, 0.001f),
            Mathf.Max(worldSize.z, 0.001f));

        // Account for schematic's scale
        Vector3 schematicScale = schematicRoot.lossyScale;
        Vector3 boundsSize = new(
            Mathf.Approximately(schematicScale.x, 0f) ? worldSize.x : worldSize.x / Mathf.Abs(schematicScale.x),
            Mathf.Approximately(schematicScale.y, 0f) ? worldSize.y : worldSize.y / Mathf.Abs(schematicScale.y),
            Mathf.Approximately(schematicScale.z, 0f) ? worldSize.z : worldSize.z / Mathf.Abs(schematicScale.z));

        Vector3 localBoundsCenter = clusterLocalBounds.center;
        Vector3 boundsPosition = schematicRoot.InverseTransformPoint(localBoundsCenter);

        clusterTransform.localPosition = boundsPosition;
        clusterTransform.localRotation = Quaternion.identity;
        clusterTransform.localScale = Vector3.one;

        // Set BoundsSize and BoundsPosition on CullingParentComponent
        if (clusterTransform.TryGetComponent(out CullingParentComponent cullingParent))
        {
            cullingParent.BoundsSize = boundsSize;
        }
    }

    private static bool TryGetLightBounds(Transform transform, out Bounds bounds)
    {
        LightComponent lightComponent = transform.GetComponentInChildren<LightComponent>(true);
        if (lightComponent != null && lightComponent.TryGetComponent(out Light light))
        {
            Vector3 lightPosition = lightComponent.transform.position;

            if (light.type == LightType.Point || light.type == LightType.Spot)
            {
                float diameter = Mathf.Max(light.range * 2f, 0.001f);
                bounds = new Bounds(lightPosition, Vector3.one * diameter);
                return true;
            }

            bounds = new Bounds(lightPosition, Vector3.one * 0.5f);
            return true;
        }

        bounds = default;
        return false;
    }

    private static bool TryGetBlockType(Transform transform, out BlockType blockType)
    {
        if (transform.TryGetComponent(out SchematicBlock block))
        {
            blockType = block.BlockType;
            return true;
        }

        blockType = default;
        return false;
    }

    private static bool IsBigObject(Transform transform, float minimumSizeBeforeBeingBigPrimitive)
    {
        if (minimumSizeBeforeBeingBigPrimitive <= 0f)
            return false;

        Vector3 localScale = transform.localScale;
        float size = Mathf.Abs(localScale.x) + Mathf.Abs(localScale.y) + Mathf.Abs(localScale.z);
        return size >= minimumSizeBeforeBeingBigPrimitive;
    }

    private static bool TryGetHierarchyBounds(Transform root, out Bounds bounds)
    {
        bool hasBounds = false;
        bounds = default;

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null)
                continue;

            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        foreach (Collider collider in root.GetComponentsInChildren<Collider>(true))
        {
            if (collider == null)
                continue;

            if (!hasBounds)
            {
                bounds = collider.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(collider.bounds);
            }
        }

        if (!hasBounds)
        {
            Vector3 fallbackSize = root.lossyScale;
            bounds = new Bounds(root.position, new Vector3(
                Mathf.Max(Mathf.Abs(fallbackSize.x), 0.001f),
                Mathf.Max(Mathf.Abs(fallbackSize.y), 0.001f),
                Mathf.Max(Mathf.Abs(fallbackSize.z), 0.001f)));
        }

        return true;
    }

    private static float DistanceToBounds(Bounds bounds, Vector3 point)
    {
        Vector3 closestPoint = bounds.ClosestPoint(point);
        return Vector3.Distance(closestPoint, point);
    }

    private sealed class ClusterCandidate
    {
        public ClusterCandidate(Transform transform, Bounds bounds)
        {
            Transform = transform;
            Bounds = bounds;
        }

        public Transform Transform { get; }
        public Bounds Bounds { get; }
    }

    private sealed class ClusterGroup
    {
        public ClusterGroup(Bounds initialBounds)
        {
            Bounds = initialBounds;
        }

        public List<ClusterCandidate> Members { get; } = new();
        public Bounds Bounds { get; private set; }
        public int MemberCount => Members.Count;

        public void Add(ClusterCandidate candidate)
        {
            Members.Add(candidate);
            Bounds.Encapsulate(candidate.Bounds);
        }
    }
}