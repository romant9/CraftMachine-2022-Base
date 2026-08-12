using BaseModel;

namespace TWDModel
{
	public class RestoreSavedCombatTeamCommand : ModelCommand
	{
		public SurvivorContainerModel.SurvivorType Type;

		public RestoreSavedCombatTeamCommand()
		{
		}

		public RestoreSavedCombatTeamCommand(SurvivorContainerModel.SurvivorType type)
		{
			Type = type;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if ((manager.GetPlayer() as PlayerModel).SurvivorContainer.RestoreCombatTeam(Type))
			{
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
