using TWDModel;

public class BeatEmUpVisualizationTask : GenericAbilityVisualizationTask
{
	public BeatEmUpVisualizationTask(BeatEmUpAction action)
		: base(action)
	{
		traitIdentifier = "Ui_Icon_Trait_LeaderBuffBeatEmUp";
	}
}
