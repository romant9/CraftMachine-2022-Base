using TWDModel;

[GraphItNode(NodeType.Action)]
public class QuickTipClientNode : ClientNodeBase
{
	[GraphItVariable("Tip ID")]
	public string TipID;

	[GraphItInput("Activate", "")]
	public void Activate()
	{
		VisualizationQueue.Instance.Add(new QuickTipVisualizationTask(TipID));
	}
}
