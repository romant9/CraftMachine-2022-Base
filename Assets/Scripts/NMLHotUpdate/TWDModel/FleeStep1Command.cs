using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class FleeStep1Command : ModelCommand
	{
		public List<int> modelIds { get; set; }

		public FleeStep1Command()
		{
			modelIds = new List<int>();
		}

		public FleeStep1Command(List<SurvivorModel> survivors)
		{
			modelIds = new List<int>();
			for (int i = 0; i < survivors.Count; i++)
			{
				modelIds.Add(survivors[i].ModelId);
			}
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			List<SurvivorModel> list = new List<SurvivorModel>();
			for (int i = 0; i < modelIds.Count; i++)
			{
				SurvivorModel model = manager.GetModel<SurvivorModel>(modelIds[i]);
				list.Add(model);
			}
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			IMapMissionModel attackTargetMissionModel = (manager as TWDModelManager).Player.GetAttackTargetMissionModel();
			bool flag = true;
			if (attackTargetMissionModel != null)
			{
				flag = attackTargetMissionModel.MaxTeamSize > 0;
			}
			if (list.Count > 0 || combatModel.HasPvPRules || !flag || combatModel.IsEndlessBattleMission)
			{
				TWDModelResult tWDModelResult = combatModel.FleeStep1(list);
				if (tWDModelResult == TWDModelResult.OK && combatModel.IsEndlessBattleMission)
				{
					EndlessModeCombatModel endlessModeCombatModel = combatModel.EndlessModeCombatModel;
					endlessModeCombatModel.EndlessModeManager?.HandlePostMissionLogic();
					endlessModeCombatModel?.SetSurvivorsSurvivedWaveCount();
				}
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
