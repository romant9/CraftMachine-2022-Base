using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ClientNodeBase : NodeBaseWrapper, INodeBase
{
	[HideInInspector]
	public int guidHash = Guid.NewGuid().GetHashCode();

	[HideInInspector]
	public List<NodeConnection> nodeConnections = new List<NodeConnection>();

	[HideInInspector]
	public List<NodeConnection> nodeDataConnections = new List<NodeConnection>();

	private Dictionary<string, List<NodeDelegate_Void>> runtimeDelegateMap = new Dictionary<string, List<NodeDelegate_Void>>();

	private Dictionary<string, List<RuntimeExportPinInfo>> runtimePropertyInfoMap = new Dictionary<string, List<RuntimeExportPinInfo>>();

	public int GuidHash
	{
		get
		{
			return guidHash;
		}
		set
		{
			guidHash = value;
		}
	}

	public List<NodeConnection> NodeConnections => nodeConnections;

	public List<NodeConnection> NodeDataConnections => nodeDataConnections;

	public Dictionary<string, List<NodeDelegate_Void>> RuntimeDelegateMap => runtimeDelegateMap;

	public Dictionary<string, List<RuntimeExportPinInfo>> RuntimePropertyInfoMap => runtimePropertyInfoMap;

	public bool IsClientOnly => true;

	public override INodeBase NodeBase
	{
		get
		{
			return this;
		}
		set
		{
		}
	}

	public void SetupConnections(NodeGraphWrapper nodeGraphWrapper)
	{
		for (int i = 0; i < nodeConnections.Count; i++)
		{
			NodeConnection nodeConnection = nodeConnections[i];
			INodeBase node = nodeGraphWrapper.GetNode(nodeConnection.TargetGuidHash);
			if (node != null)
			{
				INodeBaseHelpers.CreateConnectionDelegate(this, nodeConnection, node);
			}
		}
		for (int j = 0; j < nodeDataConnections.Count; j++)
		{
			NodeConnection nodeConnection2 = nodeDataConnections[j];
			INodeBase node2 = nodeGraphWrapper.GetNode(nodeConnection2.TargetGuidHash);
			if (node2 != null)
			{
				INodeBaseHelpers.CreateDataConnection(this, nodeConnection2, node2);
			}
		}
	}

	public void Fire(string id)
	{
		INodeBaseHelpers.Fire(this, id);
	}

	public object Import(string id)
	{
		return INodeBaseHelpers.Import(this, id);
	}

	public List<object> ImportValues(string id)
	{
		return INodeBaseHelpers.ImportValues(this, id);
	}

	public void Export(string id, object value)
	{
		INodeBaseHelpers.Export(this, id, value);
	}

	public void ExportValues(string id, object value)
	{
		INodeBaseHelpers.ExportValues(this, id, value);
	}
}
