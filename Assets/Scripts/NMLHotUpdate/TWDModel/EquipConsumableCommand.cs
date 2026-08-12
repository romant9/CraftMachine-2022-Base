using BaseModel;

namespace TWDModel
{
	public class EquipConsumableCommand : ModelCommand
	{
		public int EquipmentItemId { get; protected set; }

		public EquipConsumableCommand()
		{
		}

		public EquipConsumableCommand(ActorModel actor, EquipmentItemModel consumable)
			: base(actor)
		{
			EquipmentItemId = consumable.ModelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			EquipmentItemModel model2 = manager.GetModel<EquipmentItemModel>(EquipmentItemId);
			if (model == null || tWDModelManager == null || model2 == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			EquipmentModel.ConsumableType consumableType = ConsumableUtils.IdToConsumableType(model2.Definition.ID);
			if (consumableType == EquipmentModel.ConsumableType.Unknown)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (model2.Definition.Category != EquipmentCategory.Utility || tWDModelManager.CombatModel.IsConsumableInCooldown(consumableType))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (consumableType == EquipmentModel.ConsumableType.MedKit && (model.IsDead || model.IsRaider))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			bool flag = model.Equip(model2) == TWDModelResult.OK;
			if (flag)
			{
				flag = model.EquipConsumableEquipment();
			}
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
