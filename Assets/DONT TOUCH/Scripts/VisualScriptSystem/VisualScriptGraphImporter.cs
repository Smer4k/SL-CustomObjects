using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem
{
    [ScriptedImporter(1, VisualScriptGraph.AssetExtension)]
    public sealed class VisualScriptGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<VisualScriptGraph>(ctx.assetPath);
            if (graph == null)
            {
                Debug.LogError($"Failed to load Action Director graph asset: {ctx.assetPath}");
                return;
            }
            var runtimeAsset = ScriptableObject.CreateInstance<VisualScriptRuntimeGraph>();
            ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }
    }
}