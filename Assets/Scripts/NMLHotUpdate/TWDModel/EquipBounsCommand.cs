using BaseModel;

namespace TWDModel
{
	public class EquipBounsCommand : ModelCommand
	{
		public int BounsId { get; protected set; }

		public EquipBounsCommand()
		{
		}

		public EquipBounsCommand(SurvivorModel survivorModel, BounsModel bounsModel)
			: base(survivorModel)
		{
			BounsId = bounsModel.ModelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			BounsModel model2 = manager.GetModel<BounsModel>(BounsId);
			TWDModelResult result = TWDModelResult.Error;
			if (model2.Owner == model.ActorDefinitionID || model2.LevelDefinition != null)
			{
				result = model.EquipBouns(model2);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
