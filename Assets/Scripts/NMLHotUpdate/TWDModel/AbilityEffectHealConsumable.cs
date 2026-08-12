using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectHealConsumable : AbilityEffect
	{
		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			bool flag = true;
			if (flag && ownerAbility.CanAbilityBePerformedOnGridCell(combatModel, source, source.GridCoordinate, targetCell) == AbilityResult.Success)
			{
				List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(ownerAbility, source, source.GridCoordinate, targetCell);
				if (listOfActorsToBeTargetted.Count > 0)
				{
					flag = CombatHelpers.AttackTargetConsumable(combatModel, source, listOfActorsToBeTargetted[0], ownerAbility, DamageType.Heal, ignoreRandomHitChance: true);
					listOfActorsToBeTargetted.RemoveAt(0);
					if (flag)
					{
						CombatHelpers.AttackTargetsConsumable(combatModel, source, listOfActorsToBeTargetted, ownerAbility, DamageType.Heal);
					}
				}
				else
				{
					flag = true;
				}
			}
			return flag;
		}
	}
}
