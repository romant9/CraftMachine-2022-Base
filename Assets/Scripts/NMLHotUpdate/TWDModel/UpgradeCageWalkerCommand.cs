using BaseModel;

namespace TWDModel
{
	public class UpgradeCageWalkerCommand : ConsumeCurrencyCommand
	{
		public bool Instant { get; set; }

		public UpgradeCageWalkerCommand()
		{
		}

		public UpgradeCageWalkerCommand(OutpostWalkerModel outpostWalkerModel)
			: base(outpostWalkerModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			OutpostWalkerModel outpostWalkerModel = (OutpostWalkerModel)manager.GetModel(base.ModelId);
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (Instant)
			{
				tWDModelResult = outpostWalkerModel.UpgradeInstant();
			}
			else
			{
				tWDModelResult = outpostWalkerModel.StartUpgrade(base.UseDiamondsAmount);
				if (tWDModelResult == TWDModelResult.OK)
				{
					if ((manager.GetPlayer() as PlayerModel).Camp.GetBuilding("Cage") is CageBuildingModel cageBuildingModel)
					{
						cageBuildingModel.SetUpgradingModel(outpostWalkerModel);
					}
					else
					{
						tWDModelManager.Debug.LogError("cage not found!");
						tWDModelResult = TWDModelResult.Error;
					}
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
