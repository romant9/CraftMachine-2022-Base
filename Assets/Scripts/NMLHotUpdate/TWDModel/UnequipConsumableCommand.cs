using BaseModel;

namespace TWDModel
{
	public class UnequipConsumableCommand : ModelCommand
	{
		public UnequipConsumableCommand()
		{
		}

		public UnequipConsumableCommand(ActorModel actor)
			: base(actor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			bool flag = model.EquipWeaponEquipment();
			if (flag)
			{
				model.UnequipConsumableEquipment();
			}
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
