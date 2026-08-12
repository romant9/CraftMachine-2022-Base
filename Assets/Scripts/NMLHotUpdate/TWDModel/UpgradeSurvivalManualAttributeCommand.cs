using BaseModel;

namespace TWDModel
{
	public class UpgradeSurvivalManualAttributeCommand : ConsumeCurrencyCommand
	{
		public int SurvivalManualDefinitionId { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.gameEconomyData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			TWDModelResult result = tWDModelManager.Player.SurvivalManualManager.UpgradeSurvivalManualAttributeLeve();
			return new NGModelCommandRespond(this, result);
		}
	}
}
