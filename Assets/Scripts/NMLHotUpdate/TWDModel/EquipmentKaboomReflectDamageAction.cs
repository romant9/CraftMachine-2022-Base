using BaseModel;

namespace TWDModel
{
	public class EquipmentKaboomReflectDamageAction : ModelActorAction
	{
		private readonly int damage;

		public EquipmentKaboomReflectDamageAction(ActorModel actor, int damage)
			: base(actor)
		{
			this.damage = damage;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute())
			{
				return damage > 0;
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager)?.CombatModel;
			if (combatModel == null || base.Actor == null)
			{
				return false;
			}
			ActorModel actorModel = null;
			if (base.Actor.HelpreHandActorModel == null && combatModel.AbilityManager != null && combatModel.manager?.Player != null)
			{
				ActorModel helpHandActor = CombatHelpers.getHelpHandActor(combatModel, base.Actor);
				if (helpHandActor != null)
				{
					base.Actor.HelpreHandActorModel = helpHandActor;
					actorModel = helpHandActor;
				}
			}
			try
			{
				return CombatHelpers.ExecuteDamage(combatModel, null, base.Actor, damage, 0, DamageType.Base, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed, null, dealDamagePostAbility: false, null, noChargeGain: true, null, isMainTarget: true, isTriggerExtraAttackDamage: false, OOTType.None, isChargeAttack: false, isEquipmentKaboomReflect: true);
			}
			finally
			{
				if (actorModel != null && base.Actor.HelpreHandActorModel == actorModel)
				{
					base.Actor.HelpreHandActorModel = null;
				}
			}
		}
	}
}
