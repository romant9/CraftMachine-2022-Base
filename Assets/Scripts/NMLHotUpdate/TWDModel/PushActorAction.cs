using BaseModel;

namespace TWDModel
{
	public class PushActorAction : ModelActorAction
	{
		public PushEffect PushEffect { get; private set; }

		public GridPath Path { get; private set; }

		public PushActorAction(PushEffect effect)
			: base(effect.DamageAction.TargetActor)
		{
			PushEffect = effect;
			Path = GridPath.Create();
			Path.AddNode(effect.DamageAction.TargetActor.GridCoordinate);
			Path.AddNode(PushEffect.PushCoordinate);
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				if (base.Actor.IsStruggling)
				{
					TimedEffect exclusiveTimedEffect = base.Actor.ExclusiveTimedEffect;
					ActorModel actorModel = ((exclusiveTimedEffect != null) ? (exclusiveTimedEffect.Target as ActorModel) : null);
					ActorModel actorModel2 = exclusiveTimedEffect?.Instigator;
					if (actorModel != null && actorModel.IsStruggling)
					{
						actorModel.FinishTimedEffect(interrupted: true);
					}
					if (actorModel2 != null && actorModel2.IsStruggling)
					{
						actorModel2.FinishTimedEffect(interrupted: true);
					}
				}
				return combatModel.PushActor(base.Actor, Path);
			}
			manager.Debug.LogWarning("PushActorAction::Execute() failed -> CombatModel is null");
			return false;
		}
	}
}
