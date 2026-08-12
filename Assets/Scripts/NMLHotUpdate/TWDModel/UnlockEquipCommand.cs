using BaseModel;

namespace TWDModel
{
	public class UnlockEquipCommand : ModelCommand
	{
		public UnlockEquipCommand()
		{
		}

		public UnlockEquipCommand(EquipTokenItemModel equipTokenItem)
			: base(equipTokenItem)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			bool flag = manager.GetModel<EquipTokenItemModel>(base.ModelId).UnlockEquip();
			if (!flag)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
