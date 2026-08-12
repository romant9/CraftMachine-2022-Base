using BaseModel;

namespace TWDModel
{
	public sealed class RestoreAPAction : ModelAction
	{
		public ActorModel SourceActor { get; private set; }

		public bool Avoided { get; set; }

		public RestoreAPAction(ActorModel source)
			: base(source)
		{
			SourceActor = source;
		}

		public override bool Execute(ModelManager manager)
		{
			if (SourceActor != null && !Avoided && !SourceActor.IsElectricShocked && !SourceActor.IsStunned)
			{
				SourceActor.ResetActionPointsForExternal();
				SourceActor.UsedChargeAttackThisTurn = false;
				SourceActor.FightingFuryActivated = false;
				SourceActor.AdditionalAttackCount = 0;
				SourceActor.AdditionalAttackConsumed = false;
				SourceActor.CanMoveWithoutAttacking = false;
				SourceActor.GivenAdditionalAttacks = 0;
				SourceActor.FightingFuryTargetCount = 0;
				SourceActor.GainedAPFromAbilityExecution = false;
				SourceActor.GainedAPFromPreviousAbilityExecution = false;
				EquipmentItemModel weaponEquipment = SourceActor.GetWeaponEquipment();
				if (weaponEquipment != null && weaponEquipment.Ability != null)
				{
					weaponEquipment.Ability.UsesThisTurn = 0;
					weaponEquipment.Ability.MaxUsesPerTurn = 1;
				}
				return true;
			}
			return false;
		}
	}
}
