using BaseModel;

namespace TWDModel
{
	public class ToggleFavouriteForEquipment : ModelCommand
	{
		public ToggleFavouriteForEquipment()
		{
		}

		public ToggleFavouriteForEquipment(EquipmentItemModel equipmentModel)
			: base(equipmentModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager.GetModel(base.ModelId) is EquipmentItemModel equipmentItemModel))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			equipmentItemModel.IsFavourite = !equipmentItemModel.IsFavourite;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
