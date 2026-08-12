using BaseModel;

namespace TWDModel
{
	public class UpgradedModelViewedCommand : ModelCommand
	{
		public UpgradedModelViewedCommand()
		{
		}

		public UpgradedModelViewedCommand(ModelUpgraderBuildingModel building)
			: base(building)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ModelUpgraderBuildingModel model = manager.GetModel<ModelUpgraderBuildingModel>(base.ModelId);
			if (model != null)
			{
				model.MarkModelUpgradeAsSeen();
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
