using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectApplyGore : AbilityEffect
	{
		private int turns { get; set; }

		public AbilityEffectApplyGore(int turns)
		{
			this.turns = turns;
		}

		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			source.IsInteractingWithGuts = true;
			source.SetInvisible(turns, source);
			return true;
		}
	}
}
