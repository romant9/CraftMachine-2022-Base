using BaseModel;

namespace TWDModel
{
	public class DeathsDoorPursuitAction : GenericAbilityAction
	{
		private readonly bool blockSecondChance;

		public DeathsDoorPursuitAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor, bool blockSecondChance)
			: base(sourceActor, ability, targetCell, "ActorNotification.DeathsDoorPursuit", targetActor, OOTType.PassByAttack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true)
		{
			this.blockSecondChance = blockSecondChance;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute())
			{
				return !(base.TargetActor?.IsDead ?? true);
			}
			return false;
		}

		protected override AbilityModel GetOOTValidationAbility()
		{
			return base.Ability;
		}

		public override bool Execute(ModelManager manager)
		{
			bool deathsDoor_IsPursuitAttack = base.Actor.DeathsDoor_IsPursuitAttack;
			bool deathsBlockSecondChance = base.Actor.DeathsBlockSecondChance;
			base.Actor.DeathsDoor_IsPursuitAttack = true;
			base.Actor.DeathsBlockSecondChance = blockSecondChance;
			bool result = base.Execute(manager);
			base.Actor.DeathsDoor_IsPursuitAttack = deathsDoor_IsPursuitAttack;
			base.Actor.DeathsBlockSecondChance = deathsBlockSecondChance;
			return result;
		}
	}
}
