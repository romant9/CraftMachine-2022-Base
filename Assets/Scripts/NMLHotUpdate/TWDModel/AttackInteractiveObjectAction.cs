using BaseModel;

namespace TWDModel
{
	public class AttackInteractiveObjectAction : ModelAction
	{
		public InteractiveObjectModel Target { get; private set; }

		public ActorModel Attacker { get; private set; }

		public AttackInteractiveObjectAction(ActorModel actor, InteractiveObjectModel target)
			: base(actor)
		{
			Target = target;
			Attacker = actor;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
				if (model != null)
				{
					bool num = combatModel.AttackInteractiveObject(model, Target);
					if (num)
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
