using System;
using System.Collections.Generic;
using System.Reflection;

namespace TWDModel
{
	public class INodeBaseHelpers
	{
		public static bool AddConnection(INodeBase originNode, string pinId, INodeBase connectToNode, string connectToPinId)
		{
			if (originNode.IsClientOnly && !connectToNode.IsClientOnly)
			{
				return false;
			}
			for (int i = 0; i < originNode.NodeConnections.Count; i++)
			{
				if (originNode.NodeConnections[i].InputPinId == pinId && originNode.NodeConnections[i].TargetGuidHash == connectToNode.GuidHash && originNode.NodeConnections[i].OutputPinId == connectToPinId)
				{
					return false;
				}
			}
			if (!IsConnectionValid(connectToNode.GetType(), connectToPinId))
			{
				return false;
			}
			originNode.NodeConnections.Add(new NodeConnection
			{
				TargetGuidHash = connectToNode.GuidHash,
				InputPinId = connectToPinId,
				OutputPinId = pinId
			});
			return true;
		}

		public static bool AddDataConnection(INodeBase originNode, string pinId, INodeBase connectToNode, string connectToPinId)
		{
			if (originNode.IsClientOnly && !connectToNode.IsClientOnly)
			{
				return false;
			}
			for (int i = 0; i < originNode.NodeDataConnections.Count; i++)
			{
				if (originNode.NodeDataConnections[i].InputPinId == pinId && originNode.NodeDataConnections[i].TargetGuidHash == connectToNode.GuidHash && originNode.NodeDataConnections[i].OutputPinId == connectToPinId)
				{
					return false;
				}
			}
			originNode.NodeDataConnections.Add(new NodeConnection
			{
				TargetGuidHash = connectToNode.GuidHash,
				InputPinId = connectToPinId,
				OutputPinId = pinId
			});
			return true;
		}

		public static void RemoveConnectionsTo(INodeBase source, INodeBase target)
		{
			List<NodeConnection> list = new List<NodeConnection>();
			for (int i = 0; i < source.NodeConnections.Count; i++)
			{
				if (source.NodeConnections[i].TargetGuidHash == target.GuidHash)
				{
					list.Add(source.NodeConnections[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				source.NodeConnections.Remove(list[j]);
			}
			list.Clear();
			for (int k = 0; k < source.NodeDataConnections.Count; k++)
			{
				if (source.NodeDataConnections[k].TargetGuidHash == target.GuidHash)
				{
					list.Add(source.NodeDataConnections[k]);
				}
			}
			for (int l = 0; l < list.Count; l++)
			{
				source.NodeDataConnections.Remove(list[l]);
			}
			list.Clear();
		}

		public static void CreateConnectionDelegate(INodeBase originNode, NodeConnection nodeConnection, INodeBase targetNodeBase)
		{
			if (!IsConnectionValid(targetNodeBase.GetType(), nodeConnection.InputPinId))
			{
				return;
			}
			MethodInfo methodInfo = GetMethodInfo(targetNodeBase.GetType(), nodeConnection.InputPinId);
			if (methodInfo != null)
			{
				NodeDelegate_Void item = Delegate.CreateDelegate(typeof(NodeDelegate_Void), targetNodeBase, methodInfo) as NodeDelegate_Void;
				List<NodeDelegate_Void> list = null;
				if (!originNode.RuntimeDelegateMap.ContainsKey(nodeConnection.OutputPinId))
				{
					list = new List<NodeDelegate_Void>();
					originNode.RuntimeDelegateMap.Add(nodeConnection.OutputPinId, list);
				}
				else
				{
					list = originNode.RuntimeDelegateMap[nodeConnection.OutputPinId];
				}
				list.Add(item);
			}
		}

		public static void CreateDataConnection(INodeBase originNode, NodeConnection nodeConnection, INodeBase targetNodeBase)
		{
			PropertyInfo propertyInfo = GetPropertyInfo(originNode.GetType(), nodeConnection.OutputPinId);
			if (propertyInfo != null)
			{
				RuntimeExportPinInfo item = new RuntimeExportPinInfo
				{
					PropertyInfo = propertyInfo,
					Source = originNode
				};
				if (!targetNodeBase.RuntimePropertyInfoMap.ContainsKey(nodeConnection.InputPinId))
				{
					targetNodeBase.RuntimePropertyInfoMap.Add(nodeConnection.InputPinId, new List<RuntimeExportPinInfo> { item });
				}
				else
				{
					targetNodeBase.RuntimePropertyInfoMap[nodeConnection.InputPinId].Add(item);
				}
			}
		}

		public static bool IsConnectionValid(Type targetType, string targetPinId)
		{
			MethodInfo methodInfo = GetMethodInfo(targetType, targetPinId);
			if (methodInfo != null)
			{
				return methodInfo.GetParameters().Length == 0;
			}
			return false;
		}

		public static MethodInfo GetMethodInfo(Type type, string pinId)
		{
			MethodInfo[] methods = type.GetMethods();
			foreach (MethodInfo methodInfo in methods)
			{
				GraphItInput graphItInput = (GraphItInput)Attribute.GetCustomAttribute(methodInfo, typeof(GraphItInput));
				if (graphItInput != null && graphItInput.Id == pinId)
				{
					return methodInfo;
				}
			}
			return null;
		}

		public static PropertyInfo GetPropertyInfo(Type type, string pinId)
		{
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				GraphItExportData graphItExportData = (GraphItExportData)Attribute.GetCustomAttribute(propertyInfo, typeof(GraphItExportData));
				if (graphItExportData != null && graphItExportData.Id == pinId)
				{
					return propertyInfo;
				}
			}
			return null;
		}

		public static PinExecutionResult Fire(INodeBase source, string id)
		{
			if (source.RuntimeDelegateMap.ContainsKey(id))
			{
				List<NodeDelegate_Void> list = source.RuntimeDelegateMap[id];
				for (int i = 0; i < list.Count; i++)
				{
					list[i]();
				}
				if (list.Count <= 0)
				{
					return PinExecutionResult.Success_NothingConnected;
				}
				return PinExecutionResult.Success;
			}
			return PinExecutionResult.Failed;
		}

		public static bool HasConnections(INodeBase source, string id)
		{
			if (source.RuntimeDelegateMap.ContainsKey(id))
			{
				return source.RuntimeDelegateMap[id].Count > 0;
			}
			return false;
		}

		public static List<object> ImportValues(INodeBase source, string id)
		{
			if (source.RuntimePropertyInfoMap.ContainsKey(id))
			{
				List<RuntimeExportPinInfo> list = source.RuntimePropertyInfoMap[id];
				List<object> list2 = new List<object>();
				for (int i = 0; i < list.Count; i++)
				{
					list2.Add(list[i].PropertyInfo.GetValue(list[i].Source, null));
				}
				return list2;
			}
			return null;
		}

		public static object Import(INodeBase source, string id)
		{
			if (source.RuntimePropertyInfoMap.ContainsKey(id))
			{
				List<RuntimeExportPinInfo> list = source.RuntimePropertyInfoMap[id];
				return list[0].PropertyInfo.GetValue(list[0].Source, null);
			}
			return null;
		}

		public static void ExportValues(INodeBase source, string id, object value)
		{
			if (source.RuntimePropertyInfoMap.ContainsKey(id))
			{
				List<RuntimeExportPinInfo> list = source.RuntimePropertyInfoMap[id];
				for (int i = 0; i < list.Count; i++)
				{
					list[i].PropertyInfo.SetValue(list[i].Source, value, null);
				}
			}
		}

		public static void Export(INodeBase source, string id, object value)
		{
			if (source.RuntimePropertyInfoMap.ContainsKey(id))
			{
				List<RuntimeExportPinInfo> list = source.RuntimePropertyInfoMap[id];
				list[0].PropertyInfo.SetValue(list[0].Source, value, null);
			}
		}

		public static void SetupRuntimeConnections(INodeBase nodeBase, List<INodeBase> nodes)
		{
			for (int i = 0; i < nodeBase.NodeConnections.Count; i++)
			{
				NodeConnection nodeConnection = nodeBase.NodeConnections[i];
				INodeBase nodeBase2 = null;
				for (int j = 0; j < nodes.Count; j++)
				{
					if (nodes[j].GuidHash == nodeConnection.TargetGuidHash)
					{
						nodeBase2 = nodes[j];
						break;
					}
				}
				if (nodeBase2 != null)
				{
					CreateConnectionDelegate(nodeBase, nodeConnection, nodeBase2);
				}
			}
			for (int k = 0; k < nodeBase.NodeDataConnections.Count; k++)
			{
				NodeConnection nodeConnection2 = nodeBase.NodeDataConnections[k];
				INodeBase nodeBase3 = null;
				for (int l = 0; l < nodes.Count; l++)
				{
					if (nodes[l].GuidHash == nodeConnection2.TargetGuidHash)
					{
						nodeBase3 = nodes[l];
						break;
					}
				}
				if (nodeBase3 != null)
				{
					CreateDataConnection(nodeBase, nodeConnection2, nodeBase3);
				}
			}
		}
	}
}
