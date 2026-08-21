using BaseModel;

namespace TWDModel
{
	public class MoveAction : ModelActorAction
	{
		public bool CanBeInterruptedForPassByAttack { get; set; }

		public bool CanBeInterruptedForPassByPull { get; set; }

		public GridPath Path { get; private set; }

		public bool ConsumeActionPoint { get; private set; }

		public bool GloballyBlocking { get; private set; }

		public bool Replaced { get; set; }

		public MoveAction(ActorModel actor, GridPath path, bool consumeAP = true, bool globallyBlocking = false)
			: base(actor)
		{
			Path = path;
			ConsumeActionPoint = consumeAP;
			GloballyBlocking = globallyBlocking;
			CanBeInterruptedForPassByAttack = true;
			CanBeInterruptedForPassByPull = true;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute() && !base.Actor.IsStruggling && !base.Actor.IsBleedingOut && !base.Actor.IsStunned && !base.Actor.IsElectricShocked && !base.Actor.IsEatingLure && !base.Actor.IsRooted && !base.Actor.IsABTesterAed)
			{
				return !base.Actor.IsInFortifications;
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				bool num = combatModel.MoveActor(base.Actor, Path);
				if (num && ConsumeActionPoint)
				{
					if ((base.Actor.Faction == Faction.Survivor || base.Actor.Faction == Faction.Raider) && Path.MoveDistance > base.Actor.MoveRange && !Path.HasTargetCoordinate)
					{
						base.Actor.EndAbilityAction();
						base.Actor.EndMovement();
						return num;
					}
					base.Actor.EndMovement();
				}
				return num;
			}
			manager.Debug.LogWarning("MoveAction::Execute() failed -> CombatModel is null");
			return false;
		}
	}
}
