using BaseModel;

namespace TWDModel
{
	public class UseInteractiveObjectAction : ModelAction
	{
		public InteractiveObjectModel Target { get; private set; }

		public UseInteractiveObjectAction(ActorModel actor, InteractiveObjectModel target)
			: base(actor)
		{
			Target = target;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
				if (model != null)
				{
					bool num = combatModel.UseInteractiveObject(model, Target);
					if (num && !model.IsInteractingWithGuts)
					{
						model.EndAbilityAction();
					}
					return num;
				}
			}
			return false;
		}
	}
}
