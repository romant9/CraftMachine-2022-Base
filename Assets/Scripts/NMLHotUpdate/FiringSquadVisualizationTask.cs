using TWDModel;

public class FiringSquadVisualizationTask : GenericAbilityVisualizationTask
{
	public FiringSquadVisualizationTask(FiringSquadAction action)
		: base(action)
	{
		traitIdentifier = "Ui_Icon_Trait_LeaderBuffFiringSquad";
	}
}
