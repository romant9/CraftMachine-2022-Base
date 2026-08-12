using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectProjectileConsumableAttack : AbilityEffect
	{
		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			bool flag = (combatModel.Manager as TWDModelManager).ExecuteAction(new ProjectileAction(source, targetActor, targetCell, ownerAbility));
			if (flag && ownerAbility.CanAbilityBePerformedOnGridCell(combatModel, source, source.GridCoordinate, targetCell) == AbilityResult.Success)
			{
				List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(ownerAbility, source, source.GridCoordinate, targetCell);
				if (listOfActorsToBeTargetted.Count > 0)
				{
					flag = CombatHelpers.AttackTargetConsumable(combatModel, source, listOfActorsToBeTargetted[0], ownerAbility, DamageType.Ranged, ignoreRandomHitChance: true, resolvedRolls, listOfActorsToBeTargetted.Count == 1);
					listOfActorsToBeTargetted.RemoveAt(0);
					if (flag)
					{
						CombatHelpers.AttackTargetsConsumable(combatModel, source, listOfActorsToBeTargetted, ownerAbility, DamageType.Ranged);
					}
				}
			}
			return flag;
		}
	}
}
