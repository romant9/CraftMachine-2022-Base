using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class SetCombatSurvivorsCommand : ModelCommand
	{
		public SurvivorContainerModel.SurvivorType SurvivorType;

		public int[] SurvivorIds;

		public SetCombatSurvivorsCommand()
		{
		}

		public SetCombatSurvivorsCommand(SurvivorContainerModel.SurvivorType survivorType, SurvivorModel[] survivors)
		{
			SurvivorType = survivorType;
			SurvivorIds = survivors.Select((SurvivorModel survivor) => survivor?.ModelId ?? 0).ToArray();
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager tWDModelManager)
			{
				SurvivorContainerModel survivorContainer = tWDModelManager.Player.SurvivorContainer;
				while (survivorContainer.CombatSurvivors.Count > 0)
				{
					SurvivorModel survivor = survivorContainer.CombatSurvivors[0];
					survivorContainer.RemoveSurvivorFromCombat(survivor);
				}
				int[] survivorIds = SurvivorIds;
				foreach (int num in survivorIds)
				{
					if (num > 0)
					{
						SurvivorModel model = tWDModelManager.GetModel<SurvivorModel>(num);
						survivorContainer.AddSurvivorToCombat(model, null, SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival);
					}
				}
				if (SurvivorType == SurvivorContainerModel.SurvivorType.CombatSurvival)
				{
					survivorContainer.StoreCombatTeam(SurvivorContainerModel.SurvivorType.CombatSurvival);
				}
				else
				{
					survivorContainer.StoreCombatTeam(SurvivorContainerModel.SurvivorType.Combat);
				}
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
