using TWDModel;

[GraphItNode(NodeType.Event)]
public class SmartTutorialNode : ClientNodeBase
{
	[GraphItExportData("Instigator", "")]
	public ActorModel Instigator { get; set; }

	[GraphItOutput("Activate", "")]
	public void Activate(ActorModel instigator)
	{
		Instigator = instigator;
		Fire("Activate");
	}
}
