using BaseModel;

namespace TWDModel
{
	public class UnEquipBounsCommand : ModelCommand
	{
		public UnEquipBounsCommand()
		{
		}

		public UnEquipBounsCommand(SurvivorModel survivor)
			: base(survivor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			TWDModelResult result = TWDModelResult.Error;
			if (model.UsingBounsModel != null)
			{
				result = model.UnequipBouns(model.UsingBounsModel);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
