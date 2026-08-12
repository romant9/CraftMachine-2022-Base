using BaseModel;

namespace TWDModel
{
	public class EquipmentBloodMarkSettleAction : ModelActorAction
	{
		private readonly ActorModel source;

		private readonly int damage;

		public EquipmentBloodMarkSettleAction(ActorModel actor, int damage, ActorModel source = null)
			: base(actor)
		{
			this.source = source;
			this.damage = damage;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute() && damage > 0 && base.Actor != null)
			{
				return !base.Actor.IsDead;
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager)?.CombatModel;
			if (combatModel == null || base.Actor == null || base.Actor.IsDead || damage <= 0)
			{
				return false;
			}
			return CombatHelpers.ExecuteDamage(combatModel, source, base.Actor, damage, 0, DamageType.BloodMarkSettlement, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed, null, dealDamagePostAbility: false, null, noChargeGain: true, null, isMainTarget: false, isTriggerExtraAttackDamage: false, OOTType.None, isChargeAttack: false, isEquipmentKaboomReflect: false, applyIncomingDamageMitigation: true);
		}
	}
}
