using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem
{
    [Serializable]
    [Graph(AssetExtension, GraphOptions.SupportsSubgraphs)]
    public class VisualScriptGraph : Graph
    {
        private const string KGraphName = "Visual Script Graph";
        internal const string AssetExtension = "vsg";
        
        [MenuItem("Assets/Create/Visual Script Graph")]
        public static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<VisualScriptGraph>(KGraphName);
        }

        /// <summary>
        /// Called when the graph changes.
        /// </summary>
        /// <param name="infos">The GraphLogger object to which errors and warnings are added.</param>
        /// <remarks>
        /// This method is triggered whenever the graph is modified. It calls `CheckGraphErrors` to validate the graph
        /// and report any issues.
        /// </remarks>
        public override void OnGraphChanged(GraphLogger infos)
        {
            base.OnGraphChanged(infos);
            if (DynamicNodeUpdate.Graph == null || DynamicNodeUpdate.Graph != this)
                DynamicNodeUpdate.Graph = this;
            CheckGraphErrors(infos);
        }

        /// <summary>
        /// Checks the graph for errors and warnings and adds them to the result object.
        /// </summary>
        /// <param name="infos">Object implementing <see cref="GraphLogger"/> interface and containing
        /// collected errors and warnings</param>
        /// <remarks>Errors and warnings are reported by adding them to the GraphLogger object,
        /// which is the default reporting mechanism for a Graph Toolkit tool. </remarks>
        private void CheckGraphErrors(GraphLogger infos)
        {
            var msg = new List<string>();
            foreach (var node in GetNodes())
            {
                if (node is BaseNode actionNode)
                {
                    actionNode.CheckErrors(msg);
                    foreach (var str in msg)
                    {
                        infos.LogError(str, node);
                    }
                    
                    msg.Clear();
                }
            }
        }
    }
}