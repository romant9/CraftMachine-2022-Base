using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectBazookaAttack : AbilityEffect
	{
		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			bool flag = (combatModel.Manager as TWDModelManager).ExecuteAction(new FireWeaponAction(source, targetActor, targetCell, ownerAbility));
			if (flag && ownerAbility.CanAbilityBePerformedOnGridCell(combatModel, source, source.GridCoordinate, targetCell) == AbilityResult.Success)
			{
				List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(ownerAbility, source, source.GridCoordinate, targetCell);
				CombatHelpers.RangeAttackAddChargePointFromRemoteWeaken(combatModel, source, listOfActorsToBeTargetted);
				if (listOfActorsToBeTargetted.Count > 0)
				{
					if (resolvedRolls == null)
					{
						resolvedRolls = new Dictionary<RollDiceType, PlayerRandomChanceResult>();
					}
					if (!resolvedRolls.ContainsKey(RollDiceType.BodyShot))
					{
						resolvedRolls.Add(RollDiceType.BodyShot, PlayerRandomChanceResult.Failed);
					}
					flag = CombatHelpers.AttackTarget(combatModel, source, listOfActorsToBeTargetted[0], ownerAbility, DamageType.Ranged, ignoreRandomHitChance: true, resolvedRolls, listOfActorsToBeTargetted.Count == 1, isMainTarget: true, ootType, isAssistAttack, isTriggerExtraAttackDamage);
					resolvedRolls.Remove(RollDiceType.BodyShot);
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
					if (source.HasTraitsThatContains("Equipment_Active_BloodFrenzy"))
					{
						FixedPoint value = 0.0;
						combatModel.AbilityManager.VisitParameter("Equipment_Active_BloodFrenzy_Hp", ref value, source);
						if (source.Hitpoints > 0)
						{
							int num = (int)(source.Hitpoints - source.Hitpoints * value);
							if (num <= 0)
							{
								num = 1;
							}
							source.SetHitpoints(num, DefenseSystemType.None, IsDealShield: false);
							source.NotifyChange("ActorHealthChanged");
						}
					}
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public override AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			AbilityResult abilityResult = AbilityResult.Success;
			if (sourceActor.IsReloading)
			{
				return AbilityResult.FailedOutOfUses;
			}
			return base.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, acceptInteractiveObjects);
		}

		public override AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			if (sourceActor.IsReloading)
			{
				return AbilityResult.FailedOutOfUses;
			}
			return base.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, preComputedRange, acceptInteractiveObjects);
		}
	}
}
