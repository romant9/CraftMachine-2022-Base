using BaseModel;

namespace TWDModel
{
	public class ExploreCommand : ConsumeCurrencyCommand
	{
		public bool IsDeadly { get; private set; }

		public ExploreCommand()
		{
		}

		public ExploreCommand(MapMissionGroupModel missionGroupModel, bool isDeadly)
			: base(missionGroupModel)
		{
			IsDeadly = isDeadly;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			MapMissionGroupModel model = (manager as TWDModelManager).GetModel<MapMissionGroupModel>(base.ModelId);
			if (model != null)
			{
				Cashier exploreMissionCashier = model.GetExploreMissionCashier(IsDeadly);
				if (exploreMissionCashier != null)
				{
					result = exploreMissionCashier.Pay();
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
