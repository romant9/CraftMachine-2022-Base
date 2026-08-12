using BaseModel;

namespace TWDModel
{
	public class CutVegetationCommand : ConsumeCurrencyCommand
	{
		public CutVegetationCommand()
		{
		}

		public CutVegetationCommand(VegetationModel vegetation)
			: base(vegetation)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = manager.GetModel<VegetationModel>(base.ModelId).StartCut();
			return new NGModelCommandRespond(this, result);
		}
	}
}
