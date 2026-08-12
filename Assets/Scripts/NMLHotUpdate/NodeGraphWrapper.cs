using System.Collections.Generic;
using TWDModel;
using UnityEngine;

[ExecuteInEditMode]
public class NodeGraphWrapper : MonoBehaviour
{
	[HideInInspector]
	public List<NodeBaseWrapper> Nodes = new List<NodeBaseWrapper>();

	[SerializeField]
	[HideInInspector]
	private int guidHash;

	private NodeGraph NodeGraph;

	public int GuidHash => guidHash;

	public void BindToModels(NodeGraph nodeGraph)
	{
		NodeGraph = nodeGraph;
		for (int i = 0; i < Nodes.Count; i++)
		{
			if (NodeGraph != null)
			{
				Nodes[i].BindRuntimeNode(NodeGraph);
			}
			if (Nodes[i].NodeBase.IsClientOnly)
			{
				(Nodes[i].NodeBase as NodeBaseWrapper).OnNodeBind();
			}
		}
		List<INodeBase> list = new List<INodeBase>();
		for (int j = 0; j < Nodes.Count; j++)
		{
			if (Nodes[j].NodeBase.IsClientOnly)
			{
				list.Add(Nodes[j].NodeBase);
			}
		}
		for (int k = 0; k < Nodes.Count; k++)
		{
			INodeBaseHelpers.SetupRuntimeConnections(Nodes[k].NodeBase, list);
		}
	}

	public INodeBase GetNode(int guidHash)
	{
		for (int i = 0; i < Nodes.Count; i++)
		{
			if (Nodes[i].NodeBase.GuidHash == guidHash && Nodes[i].NodeBase.IsClientOnly)
			{
				return Nodes[i].NodeBase;
			}
		}
		return null;
	}
}
