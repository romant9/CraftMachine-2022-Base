using BaseModel;

namespace TWDModel
{
	public class SetSurvivorToCombatCommand : ModelCommand
	{
		public int OldSurvivorId = -1;

		public SurvivorContainerModel.SurvivorType SurvivorType { get; set; }

		public SetSurvivorToCombatCommand()
		{
		}

		public SetSurvivorToCombatCommand(SurvivorModel newSurvivor, SurvivorModel oldSurvivor = null)
			: base(newSurvivor)
		{
			if (oldSurvivor != null)
			{
				OldSurvivorId = oldSurvivor.ModelId;
			}
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			SurvivorModel survivorModel = null;
			if (OldSurvivorId != -1)
			{
				survivorModel = manager.GetModel<SurvivorModel>(OldSurvivorId);
			}
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			if (SurvivorType == SurvivorContainerModel.SurvivorType.Combat || SurvivorType == SurvivorContainerModel.SurvivorType.CombatOutpost || SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival || SurvivorType == SurvivorContainerModel.SurvivorType.CombatGuildBattle)
			{
				tWDModelResult = playerModel.SurvivorContainer.AddSurvivorToCombat(model, survivorModel, SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival);
				if (tWDModelResult == TWDModelResult.OK)
				{
					if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival)
					{
						playerModel.SurvivorContainer.StoreCombatTeam(SurvivorContainerModel.SurvivorType.CombatSurvival);
					}
					else
					{
						playerModel.SurvivorContainer.StoreCombatTeam(SurvivorContainerModel.SurvivorType.Combat);
					}
				}
			}
			else if (SurvivorType == SurvivorContainerModel.SurvivorType.Outpost)
			{
				tWDModelResult = playerModel.SurvivorContainer.AddSurvivorToOutpostDefense(model, survivorModel);
				if (tWDModelResult == TWDModelResult.OK)
				{
					tWDModelManager.Metrics.AddSwitch().AddOldSurvivor(survivorModel).AddNewSurvivor(model)
						.Send();
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
