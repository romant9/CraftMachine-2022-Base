using BaseModel;

namespace TWDModel
{
	public class SetSurvivorToCombatTeamSlotCommand : ModelCommand
	{
		public int SetToSlotIndex = -1;

		public int FromSlotIndex = -1;

		public SurvivorContainerModel.SurvivorType SurvivorType { get; set; }

		public SetSurvivorToCombatTeamSlotCommand()
		{
		}

		public SetSurvivorToCombatTeamSlotCommand(SurvivorModel newSurvivor, int toTeamSlotIndex, int fromSlotIndex)
			: base(newSurvivor)
		{
			SetToSlotIndex = toTeamSlotIndex;
			FromSlotIndex = fromSlotIndex;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			if (SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle)
			{
				tWDModelResult = playerModel.SurvivorContainer.AddSurvivorToCombatTeamSlot(model, SetToSlotIndex, FromSlotIndex);
				if (tWDModelResult == TWDModelResult.OK)
				{
					StoreCombatTeams(playerModel);
				}
			}
			else if (SurvivorType == SurvivorContainerModel.SurvivorType.Outpost)
			{
				tWDModelResult = playerModel.SurvivorContainer.AddSurvivorToOutpostDefenseSlot(model, SetToSlotIndex, FromSlotIndex);
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}

		private void StoreCombatTeams(PlayerModel player)
		{
			if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival)
			{
				player.SurvivorContainer.StoreCombatTeam(SurvivorContainerModel.SurvivorType.CombatSurvival);
			}
			else
			{
				player.SurvivorContainer.StoreCombatTeam(SurvivorContainerModel.SurvivorType.Combat);
			}
		}
	}
}
