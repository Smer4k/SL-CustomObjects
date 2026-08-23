using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using DONT_TOUCH.Enums;
using DONT_TOUCH.Scripts;
using DONT_TOUCH.Scripts.BlockComponents;
using DONT_TOUCH.Scripts.BlockSerialization;
using DONT_TOUCH.Scripts.Extensions;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0618

[ExecuteInEditMode]
public class Schematic : SchematicBlock
{
    public override BlockType BlockType => BlockType.Schematic;
    [SerializeField] private SchematicClusterOptimizerSettings clusterOptimizer = new();

    public SchematicClusterOptimizerSettings ClusterOptimizer => clusterOptimizer;
    public int OptimizeClusters() => SchematicClusterOptimizer.Optimize(this);

    public void CompileSchematic()
    {
        SetupOutput(out string schematicDirectoryPath);
        CheckEmptyObjects();
        
        var rootObjectId = transform.GetId();
        BlockList.RootObjectId = rootObjectId;
        BlockList.Blocks.Clear();
        RigidbodyDictionary.Clear();
        Teleports.Clear();

        if (TryGetComponent(out Rigidbody rigidbody))
            RigidbodyDictionary.Add(rootObjectId, new SerializableRigidbody(rigidbody));

        foreach (SchematicBlock block in GetComponentsInChildren<SchematicBlock>())
        {
            if (block.CompareTag("EditorOnly") || block == this)
                continue;

            SchematicBlockData data = new();
            block.Compile(data);
            
            foreach (var blockData in BlockList.Blocks)
            {
                if (!Config.SafeBackwardCompatibility)
                    break;
                if (!block.RequiredUniqName)
                    continue;
                if (blockData.Name != block.name) continue;
                string errorMsg = $"Multiple blocks found with the name '{blockData.Name}'! Rename them so that each has a unique name.";
                EditorUtility.DisplayDialog(
                    "Schematic Compilation Error",
                    errorMsg,
                    "OK"
                );
                Debug.LogError(errorMsg);
                return;
            }

            if (block.TryGetComponent(out Animator animator) && animator.runtimeAnimatorController != null)
            {
                RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
                data.AnimatorName = runtimeAnimatorController.name;
#if UNITY_2021
                BuildPipeline.BuildAssetBundle(runtimeAnimatorController,
                    runtimeAnimatorController.animationClips,
                    Path.Combine(schematicDirectoryPath, runtimeAnimatorController.name),
                    AssetBundleBuildOptions, EditorUserBuildSettings.activeBuildTarget);
#endif
            }

            if (block.TryGetComponent(out rigidbody))
                RigidbodyDictionary.Add(block.transform.GetId(), new SerializableRigidbody(rigidbody));

            BlockList.Blocks.Add(data);
        }

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}.json"),
            JsonConvert.SerializeObject(BlockList, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (RigidbodyDictionary.Count > 0)
            File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}-Rigidbodies.json"),
                JsonConvert.SerializeObject(RigidbodyDictionary, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (Teleports.Count > 0)
            File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}-Teleports.json"),
                JsonConvert.SerializeObject(Teleports, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (Config.ZipCompiledSchematics)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(schematicDirectoryPath, $"{schematicDirectoryPath}.zip",
                System.IO.Compression.CompressionLevel.Optimal, true);
            Directory.Delete(schematicDirectoryPath, true);
        }

        Debug.Log($"<color=#00FF00><b>{name}</b> has been successfully compiled!</color>");
    }

    public void Update()
    {
        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
            Debug.LogError("<color=red>Do not change the scale of the root object!</color>");
        }

        if (name.Contains(" "))
        {
            name = name.Replace(" ", string.Empty);
            Debug.LogError("<color=red>Schematic name cannot contain spaces!</color>");
        }
    }

    // This is only used in nested schematics (schematics inside other schematics)
    public override void Compile(SchematicBlockData block)
    {
        return;
        // block.Rotation = transform.localEulerAngles;
        //
        // block.BlockType = BlockType.Schematic;
        // block.Properties = new Dictionary<string, object>
        // {
        //     { "SchematicName", name }
        // };

        // return false;
    }

    private void CheckEmptyObjects()
    {
        foreach (var target in GetComponentsInChildren<Transform>())
        {
            if (target.TryGetComponent<SchematicBlock>(out _) || target.TryGetComponent<IgnoreObject>(out _))
            {
                continue;
            }

            if (target.TryGetComponent<Light>(out _))
            {
                target.gameObject.AddComponent<LightComponent>();
                continue;
            }

            if (target.TryGetComponent<MeshRenderer>(out var meshRenderer) && target.TryGetComponent<MeshFilter>(out var meshFilter))
            {
                var primitiveComponent = target.gameObject.AddComponent<PrimitiveComponent>();
                if (target.TryGetComponent(out Collider col))
                {
                    GameObject.DestroyImmediate(col);
                }
                else
                {
                    primitiveComponent.Collidable = false;
                }
                target.gameObject.tag = meshFilter.sharedMesh.name.ToLower() switch
                {
                    "cube" => "Cube",
                    "sphere" => "Sphere",
                    "capsule" => "Capsule",
                    "cylinder" => "Cylinder",
                    "plane" => "Plane",
                    "quad" => "Quad",
                    _ => target.gameObject.tag
                };
                primitiveComponent.Color = meshRenderer.sharedMaterial.color;
                continue;
            }

            target.gameObject.AddComponent<EmptyComponent>();
        }
    }

    private void SetupOutput(out string schematicDirectoryPath)
    {
        string parentDirectoryPath = Directory.Exists(Config.ExportPath) ? Config.ExportPath : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");
        schematicDirectoryPath = Path.Combine(parentDirectoryPath, name);

        if (!Directory.Exists(parentDirectoryPath))
            Directory.CreateDirectory(parentDirectoryPath);

        if (Directory.Exists(schematicDirectoryPath))
            DeleteDirectory(schematicDirectoryPath);

        if (File.Exists($"{schematicDirectoryPath}.zip"))
            File.Delete($"{schematicDirectoryPath}.zip");

        Directory.CreateDirectory(schematicDirectoryPath);
    }

    private static void DeleteDirectory(string path)
    {
        string[] files = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(path, false);
    }

    internal readonly SchematicObjectDataList BlockList = new();
    
#if UNITY_6000_5_OR_NEWER
    internal readonly Dictionary<long, SerializableRigidbody> RigidbodyDictionary = new();
#else 
    internal readonly Dictionary<int, SerializableRigidbody> RigidbodyDictionary = new();
#endif
    
    internal readonly List<SerializableTeleport> Teleports = new();

    private static BuildAssetBundleOptions AssetBundleBuildOptions => BuildAssetBundleOptions.ChunkBasedCompression |
                                                                      BuildAssetBundleOptions.ForceRebuildAssetBundle |
                                                                      BuildAssetBundleOptions.StrictMode;

    private static readonly Config Config = SchematicManager.Config;
}