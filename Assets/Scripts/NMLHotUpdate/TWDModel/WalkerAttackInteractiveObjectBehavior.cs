namespace TWDModel
{
	public class WalkerAttackInteractiveObjectBehavior : ScriptedBehavior
	{
		public WalkerAttackInteractiveObjectBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			if (base.AIDataModel.HasEvent(AIDataModel.DamageReceived))
			{
				ActorModel currentTarget = base.AIDataModel.GetCurrentTarget();
				if (currentTarget != null && GridHelpers.GetPathTowardsTarget(base.Actor, base.CombatModel, currentTarget.GridCoordinate).IsValid)
				{
					return 0;
				}
			}
			InteractiveObjectModel interactiveObjectModel = AIBehaviorHelpers.FindNearestDestroyableObject(base.Actor);
			base.AIDataModel.SetModelReference(AIDataModel.InteractiveObjectTarget, interactiveObjectModel);
			if (interactiveObjectModel != null)
			{
				if (AIBehaviorHelpers.IsTargetInAttackRange(base.Actor, base.CombatModel, interactiveObjectModel.Location.Coordinate))
				{
					if (base.Actor.AbilityCompleted)
					{
						return 0;
					}
					return 200;
				}
				if (base.Actor.MoveCompleted)
				{
					return 0;
				}
				return 200;
			}
			return 0;
		}

		public override void ExecuteAction()
		{
			InteractiveObjectModel modelReference = base.AIDataModel.GetModelReference<InteractiveObjectModel>(AIDataModel.InteractiveObjectTarget);
			if (modelReference != null)
			{
				if (AIBehaviorHelpers.IsTargetInAttackRange(base.Actor, base.CombatModel, modelReference.Location.Coordinate))
				{
					AttackInteractiveObjectCommand.PerformActions(base.Actor.manager, base.Actor, modelReference);
					base.Actor.EndAction();
					return;
				}
				GridCoordinate coordinate = modelReference.Location.Coordinate;
				if (base.CombatModel.Grid.IsCoordinateValid(coordinate))
				{
					GridPath pathTowardsTarget = GridHelpers.GetPathTowardsTarget(base.Actor, base.CombatModel, coordinate, base.Actor.MoveRange + 1);
					if (pathTowardsTarget.IsValid)
					{
						MoveCommand.PerformActions(base.Actor.manager, base.Actor, pathTowardsTarget);
					}
				}
				if (!base.Actor.MoveCompleted)
				{
					base.Actor.EndMovement();
				}
			}
			else
			{
				base.Actor.EndAction();
				base.Actor.manager.Debug.LogWarning("Walker '" + base.Actor?.ToString() + "' tried to attack interactive object but could not find valid target!");
			}
		}
	}
}
