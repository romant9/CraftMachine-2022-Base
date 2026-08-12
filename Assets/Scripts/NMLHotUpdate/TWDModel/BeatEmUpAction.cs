using BaseModel;

namespace TWDModel
{
	public class BeatEmUpAction : GenericAbilityAction
	{
		private FixedPoint damageMultiplier;

		public BeatEmUpAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, string notificationKey, FixedPoint damageMultiplier, ActorModel targetActor = null, OOTType ootType = OOTType.None, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, notificationKey, targetActor, ootType, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage)
		{
			this.damageMultiplier = damageMultiplier;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute())
			{
				return !base.TargetActor.IsDead;
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			base.Actor.GetWeaponEquipment().AddTemporaryTrait("RetaliateMultiplier", TraitExpirationType.Activation, damageMultiplier * 100.0);
			return base.Execute(manager);
		}
	}
}
