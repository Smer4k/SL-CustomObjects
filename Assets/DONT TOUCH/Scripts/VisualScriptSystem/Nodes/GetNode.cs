using System;
using System.Collections.Generic;
using DONT_TOUCH.Scripts.VisualScriptSystem.Enums;
using DONT_TOUCH.Scripts.VisualScriptSystem.NodeSerialization;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.VisualScriptSystem.Nodes
{
    public abstract class GetNode : BaseNode
    {
        public override void Compile(SerializableNode node, Dictionary<INode, int> nodeToRuntimeIndex)
        {
            base.Compile(node, nodeToRuntimeIndex);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            DefaultColor = NodeColors.GetColor;
        }

        protected void AddOutputValuePort(IPortDefinitionContext ctx, Type typePort)
        {
            ctx.AddOutputPort(Constants.OUT_VALUE_PORT_NAME)
                .WithDataType(typePort)
                .WithDisplayName("Value")
                .Build();
        }
    }
}