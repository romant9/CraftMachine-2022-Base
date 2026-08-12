using BaseModel;

namespace TWDModel
{
	public class DisableSurvivalManualStorySkillCommand : ConsumeCurrencyCommand
	{
		public new int ModelId { get; set; }

		public UpgradeType Upgrade { get; set; }

		public DisableSurvivalManualStorySkillCommand()
		{
		}

		public DisableSurvivalManualStorySkillCommand(int id)
		{
			ModelId = id;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			manager.GetModel<SurvivalManualModel>(ModelId);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
