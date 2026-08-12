using System;
using BaseModel;

namespace TWDModel
{
	public class EquipEquipmentCommand : ModelCommand
	{
		public EquipmentCategory Category { get; private set; }

		public EquipEquipmentCommand()
		{
		}

		public EquipEquipmentCommand(ActorModel actor, EquipmentCategory equipmentCategory)
			: base(actor)
		{
			Category = equipmentCategory;
		}

		public static bool PerformActions(TWDModelManager manager, ActorModel actor, EquipmentCategory category)
		{
			bool flag = false;
			switch (category)
			{
			case EquipmentCategory.MeleeWeapon:
			case EquipmentCategory.RangeWeapon:
				flag = actor.EquipWeaponEquipment();
				if (!flag)
				{
					manager.Player.Debug.LogError("EquipEquipmentCommand: Could not find equipment: " + Enum.GetName(typeof(EquipmentCategory), category));
				}
				break;
			case EquipmentCategory.ChargeEquipment:
				flag = actor.EquipChargeEquipment();
				if (!flag)
				{
					manager.Player.Debug.LogError("EquipEquipmentCommand: Could not find charge equipment or charge not available: " + EquipmentCategory.ChargeEquipment);
				}
				break;
			}
			return flag;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			bool flag = PerformActions(manager as TWDModelManager, model, Category);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
