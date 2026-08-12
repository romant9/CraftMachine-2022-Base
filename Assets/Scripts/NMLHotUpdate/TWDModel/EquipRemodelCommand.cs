using BaseModel;

namespace TWDModel
{
	public class EquipRemodelCommand : ModelCommand
	{
		public string TraitId { get; set; }

		public bool Exchange { get; set; }

		public EquipRemodelCommand()
		{
		}

		public EquipRemodelCommand(EquipmentItemModel equipmentItemModel, string traitId, bool exchange)
			: base(equipmentItemModel)
		{
			TraitId = traitId;
			Exchange = exchange;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel playerModel = manager.GetPlayer() as PlayerModel;
			EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(base.ModelId);
			TWDModelResult result = TWDModelResult.Error;
			if (model.IsValid() && playerModel != null)
			{
				result = model.EquipmentRemodel(TraitId, Exchange);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
