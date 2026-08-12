using BaseModel;

namespace TWDModel
{
	public class ModifyDashTraitCommand : ModelCommand
	{
		public ModifyDashTraitCommand()
		{
		}

		public ModifyDashTraitCommand(ActorModel actor)
			: base(actor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			model.dashTraitAttackFlag = false;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
