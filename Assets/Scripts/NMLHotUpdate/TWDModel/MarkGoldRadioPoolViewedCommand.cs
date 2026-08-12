using BaseModel;

namespace TWDModel
{
	public class MarkGoldRadioPoolViewedCommand : ModelCommand
	{
		public string Identifier { get; set; }

		public MarkGoldRadioPoolViewedCommand()
		{
		}

		public MarkGoldRadioPoolViewedCommand(string identifier)
		{
			Identifier = identifier;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				EquipPrizeWheelModel equipPrizeWheelModel = tWDModelManager.Player.EquipPrizeWheelModel;
				if (equipPrizeWheelModel == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				equipPrizeWheelModel.MarkGoldRadioPoolAsViewed(Identifier);
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
