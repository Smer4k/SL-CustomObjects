using System;
using System.Collections.Generic;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts.Extensions;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public abstract class SchematicBlock : MonoBehaviour
{
    public virtual bool RequiredUniqName { get; } = false;
    public abstract BlockType BlockType { get; }

    [Tooltip("Object movement smoothing"), Range(0, 255)]
    public byte MovementSmoothing = 60;

    public VisualScriptRuntimeGraph ScriptGraph;

    public static T Create<T>(string prefabPath) where T : Object
    {
        T prefab = AssetDatabase.LoadAssetAtPath<T>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"{prefabPath} not found.");
            return null;
        }

        T instance = Instantiate(prefab);
        instance.name = instance.name.Replace("(Clone)", string.Empty);

        return instance;
    }

    public virtual void Compile(SchematicBlockData block)
    {
        block.BlockType = BlockType;

        Transform t = transform;
        block.Name = t.name;
        block.ObjectId = t.GetId();
        block.ParentId = t.parent.GetId();

        t.GetLocalPositionAndRotation(out Vector3 localPosition, out Quaternion localRotation);
        block.Position = localPosition;
        block.Rotation = localRotation.eulerAngles;
        block.Scale = t.localScale;

        if (block.Properties != null)
        {
            block.Properties.Add("Static", gameObject.isStatic);
            block.Properties.Add("MovementSmoothing", MovementSmoothing);
        }
        else
        {
            block.Properties = new Dictionary<string, object>()
            {
                { "Static", gameObject.isStatic },
                { "MovementSmoothing", MovementSmoothing }
            };
        }
    }

    public virtual void Decompile(ref GameObject gameObject, SchematicBlockData block, Transform parent)
    {
        Transform t = gameObject.transform;
        t.name = block.Name;

        t.SetParent(parent);
        t.localPosition = block.Position;
        t.localEulerAngles = block.Rotation != null ? block.Rotation : Vector3.zero;
        t.localScale = block.Scale != null ? block.Scale == Vector3.zero ? Vector3.one : block.Scale : Vector3.one;
        if (block.Properties != null)
        {
            gameObject.isStatic = block.Properties.TryGetValue("Static", out object isStatic) &&
                                  Convert.ToBoolean(isStatic);
            if (block.Properties.TryGetValue("MovementSmoothing", out object movementSmoothing))
            {
                MovementSmoothing = Convert.ToByte(movementSmoothing);
            }
        }
    }
    
    [ContextMenu("Center Pivot To Children")]
    public void CenterPivotToChildren()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("Нет Renderer-компонентов у детей.");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        Vector3 center = bounds.center;

        Transform[] children = new Transform[transform.childCount];
        Vector3[] worldPositions = new Vector3[transform.childCount];
        Quaternion[] worldRotations = new Quaternion[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
            worldPositions[i] = children[i].position;
            worldRotations[i] = children[i].rotation;
        }

        transform.position = center;

        for (int i = 0; i < children.Length; i++)
        {
            children[i].position = worldPositions[i];
            children[i].rotation = worldRotations[i];
        }
    }
}