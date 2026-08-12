using BaseModel;

namespace TWDModel
{
	public class SurvivalManualStorSkillCommand : ConsumeCurrencyCommand
	{
		public int SurvivalManualDefinitionId { get; set; }

		public SurvivalManualStorSkillCommand()
		{
		}

		public SurvivalManualStorSkillCommand(int id)
		{
			SurvivalManualDefinitionId = id;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			manager.GetModel<LootEntry>(base.ModelId);
			TWDModelResult result = ((PlayerModel)manager.GetPlayer()).SurvivalManualManager.UpgradeSurvivalManualStorySkill(SurvivalManualDefinitionId);
			return new NGModelCommandRespond(this, result);
		}
	}
}
