using TWDModel;
using UnityEngine;

public class NodeBaseWrapper : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private int nodeGuidHash;

	private INodeBase runtimeNodeBase;

	public int NodeGuidHash => nodeGuidHash;

	public virtual string NodeBaseMemberName => "NodeBaseInternal";

	public virtual INodeBase NodeBase
	{
		get
		{
			if (runtimeNodeBase == null)
			{
				return GetType().GetField(NodeBaseMemberName).GetValue(this) as INodeBase;
			}
			return runtimeNodeBase;
		}
		set
		{
			runtimeNodeBase = value;
		}
	}

	public virtual void OnNodeBind()
	{
	}

	public void BindRuntimeNode(NodeGraph nodeGraph)
	{
		for (int i = 0; i < nodeGraph.Nodes.Count; i++)
		{
			INodeBase nodeBase = nodeGraph.Nodes[i];
			if (nodeBase.GuidHash == NodeGuidHash)
			{
				NodeBase = nodeBase;
				OnNodeBind();
				break;
			}
		}
	}
}
