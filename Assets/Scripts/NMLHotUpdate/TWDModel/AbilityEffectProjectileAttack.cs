using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectProjectileAttack : AbilityEffect
	{
		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			bool flag = (combatModel.Manager as TWDModelManager).ExecuteAction(new ProjectileAction(source, targetActor, targetCell, ownerAbility));
			if (flag && ownerAbility.CanAbilityBePerformedOnGridCell(combatModel, source, source.GridCoordinate, targetCell) == AbilityResult.Success)
			{
				List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(ownerAbility, source, source.GridCoordinate, targetCell);
				CombatHelpers.RangeAttackAddChargePointFromRemoteWeaken(combatModel, source, listOfActorsToBeTargetted);
				if (listOfActorsToBeTargetted.Count > 0)
				{
					flag = CombatHelpers.AttackTarget(combatModel, source, listOfActorsToBeTargetted[0], ownerAbility, DamageType.Ranged, ignoreRandomHitChance: true, resolvedRolls, listOfActorsToBeTargetted.Count == 1, isMainTarget: true, ootType, isAssistAttack, isTriggerExtraAttackDamage);
					if (flag)
					{
						CombatHelpers.AttackTargets(combatModel, source, listOfActorsToBeTargetted.GetRange(1, listOfActorsToBeTargetted.Count - 1), ownerAbility, DamageType.Ranged, ignoreRandomHitChance: false, ootType, isAssistAttack, isTriggerExtraAttackDamage);
						if (ownerAbility.IsChargeAttack)
						{
							CombatHelpers.CheckForLeaderBuffLeadByExample(combatModel, source);
						}
						CombatHelpers.CheckForExtraApMovement(source, listOfActorsToBeTargetted, combatModel);
						source.ClearPerAttackFlags();
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
