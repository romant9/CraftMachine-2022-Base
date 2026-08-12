using BaseModel;
using TWDModel;
using UnityEngine;

[GraphItNode(NodeType.Action)]
public class QuickTipNodeWrapper : NodeBaseWrapper
{
	[HideInInspector]
	public QuickTipNode NodeBaseInternal = new QuickTipNode();

	private QuickTipNode QuickTipNodeRef => NodeBase as QuickTipNode;

	public override void OnNodeBind()
	{
		QuickTipNodeRef.Changed += OnChanged;
	}

	private void OnChanged(ModelObject m, string changed, object args)
	{
		if (changed == "Show")
		{
			VisualizationQueue.Instance.Add(new QuickTipVisualizationTask(args as string));
		}
	}
}
