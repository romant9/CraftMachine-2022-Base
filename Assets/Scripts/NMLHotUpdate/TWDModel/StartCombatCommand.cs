using BaseModel;

namespace TWDModel
{
	public class StartCombatCommand : ModelCommand
	{
		public string MissionNameEnglish { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				combatModel.MissionNameEnglish = MissionNameEnglish;
				result = combatModel.StartCombat();
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
