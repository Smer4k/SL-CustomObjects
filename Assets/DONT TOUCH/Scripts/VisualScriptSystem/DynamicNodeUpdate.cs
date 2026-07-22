using System;
using System.Collections.Generic;
using System.Linq;
using DONT_TOUCH.Scripts.VisualScriptSystem.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace DONT_TOUCH.Scripts.VisualScriptSystem
{
    [InitializeOnLoad]
    public static class DynamicNodeUpdate
    {
        public static Graph Graph = null; // както это работать
        private static double _previousUpdateTime = 0f;

        static DynamicNodeUpdate()
        {
            EditorApplication.update += CheckPorts;
        }

        private static void CheckPorts()
        {
            if (Graph is null) return;
            if (EditorApplication.timeSinceStartup - _previousUpdateTime < 0.5) return;
            _previousUpdateTime = EditorApplication.timeSinceStartup;
            foreach (INode node in Graph.GetNodes().ToArray())
            {
                if (node is not IDynamicNode dynamicNode)
                {
                    continue;
                }

                Dictionary<string, HashSet<IPort>> snapshot = PortSnapshot(node);
                Type dynamicType = typeof(object);
                foreach (string portName in snapshot.Keys)
                {
                    if (!portName.StartsWith(Constants.DYNAMIC_PORT_PREFIX)) continue;
                    IPort connectedPort = snapshot[portName].FirstOrDefault();
                    if (connectedPort is null) continue;
                    INode connectedNode = connectedPort.GetNode();
                    IPort port = node.GetPortByName(portName);
                    if (port is null || !port.IsConnected) continue;
                    if (connectedPort.DataType == typeof(object) || connectedPort.DataType is null) continue;
                    dynamicType = connectedPort.DataType;
                    break;
                }

                if (dynamicType == dynamicNode.PortType) continue;
                RecreateNode(dynamicNode, dynamicType, snapshot);
            }
        }

        private static void RecreateNode(IDynamicNode node, Type newType, Dictionary<string, HashSet<IPort>> snapshot)
        {
            try
            {
                Graph.UndoBeginRecordGraph("update node type");
                IDynamicNode newNode = (IDynamicNode)Activator.CreateInstance(node.GetType());
                newNode.PortType = newType;
                var pos = node.Position;
                newNode.Position = pos;
                Graph.RemoveNode(node);
                Graph.AddNode((Node)newNode);
                Reconnect(newNode, snapshot);
            }
            finally
            {
                Graph.UndoEndRecordGraph();
            }
        }

        private static Dictionary<string, HashSet<IPort>> PortSnapshot(INode node)
        {
            Dictionary<string, HashSet<IPort>> connectedPorts = new();
            List<IPort> ports = new List<IPort>();
            foreach (IPort port in node.GetInputPorts().Concat(node.GetOutputPorts()))
            {
                port.GetConnectedPorts(ports);
                connectedPorts.Add(port.Name, ports.ToHashSet());
                ports.Clear();
            }

            return connectedPorts;
        }

        private static void Reconnect(INode node, Dictionary<string, HashSet<IPort>> connectedPorts)
        {
            foreach (string portName in connectedPorts.Keys)
            {
                IPort
                    port = node.GetPortByName(
                        portName); // GetPortByName - возвращает порт вне зависимости от направления (Вход/Выход)
                if (port is null) continue;

                foreach (IPort connected in connectedPorts[portName])
                {
                    if (connected is not null && ValidatePortTypes(connected, port)) GraphConnect(connected, port);
                }
            }
        }

        private static bool ValidatePortTypes(IPort first, IPort second)
        {
            return DirectionlessPortFunction(first, second, ValidatePortTypesDirectional);
        }

        private static bool GraphConnect(IPort first, IPort second)
        {
            return DirectionlessPortFunction(first, second, Graph.Connect);
        }

        private static bool ValidatePortTypesDirectional(IPort from, IPort to)
        {
            if (from.DataType == null && to.DataType is not null) return false;
            if (to.DataType == typeof(object)) return true;
            return from.DataType == to.DataType;
        }

        private static T DirectionlessPortFunction<T>(IPort first, IPort second, Func<IPort, IPort, T> func)
        {
            if (first.Direction == second.Direction) return default(T);
            if (first.Direction == PortDirection.Input) return func(second, first);
            return func(first, second);
        }

        public static IPort GetPortByName(this INode node, string name)
        {
            return node.GetInputPortByName(name) ?? node.GetOutputPortByName(name);
        }
    }
}