using BaseModel;

namespace TWDModel
{
	public class RemoveSurvivorFromCombatTeamCommand : ModelCommand
	{
		public SurvivorContainerModel.SurvivorType SurvivorType { get; set; }

		public RemoveSurvivorFromCombatTeamCommand()
		{
		}

		public RemoveSurvivorFromCombatTeamCommand(SurvivorModel survivor)
			: base(survivor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			TWDModelResult result = TWDModelResult.Error;
			if (SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle)
			{
				result = playerModel.SurvivorContainer.RemoveSurvivorFromCombat(model);
			}
			else if (SurvivorType == SurvivorContainerModel.SurvivorType.Outpost)
			{
				result = playerModel.SurvivorContainer.RemoveSurvivorFromOutpostDefense(model);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
