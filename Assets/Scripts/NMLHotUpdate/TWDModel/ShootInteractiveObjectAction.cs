using BaseModel;

namespace TWDModel
{
	public class ShootInteractiveObjectAction : ModelAction
	{
		public InteractiveObjectModel Target { get; private set; }

		public ActorModel Attacker { get; private set; }

		public ShootInteractiveObjectAction(ActorModel actor, InteractiveObjectModel target)
			: base(actor)
		{
			Target = target;
			Attacker = actor;
		}

		public override bool Execute(ModelManager manager)
		{
			if ((manager as TWDModelManager).CombatModel != null && manager.GetModel<ActorModel>(base.ModelId) != null && Target.InteractBy == InteractBy.Shoot)
			{
				return true;
			}
			return false;
		}
	}
}
