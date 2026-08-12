using BaseModel;

namespace TWDModel
{
	public class UpdateActorVisibilityCommand : ModelCommand
	{
		public UpdateActorVisibilityCommand()
		{
		}

		public UpdateActorVisibilityCommand(ActorModel actor)
			: base(actor)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			TWDModelResult result = TWDModelResult.Error;
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null && model != null)
			{
				combatModel.UpdateActorVisibility(model);
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
