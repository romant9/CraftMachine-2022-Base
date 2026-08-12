using System.Collections.Generic;

namespace TWDModel
{
	public class GuardianVowPursuitTrait : ActionModifier
	{
		private GuardianVowSkill skill;

		public GuardianVowPursuitTrait(GuardianVowSkill ownerSkill)
		{
			skill = ownerSkill;
		}

		public void RebindSkill(GuardianVowSkill ownerSkill)
		{
			skill = ownerSkill;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor == null || skill == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (!actor.IsGuardianVowActive)
			{
				return ActionListClearFlag.Keep;
			}
			CombatModel combatModel = actor.manager?.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (!(action is PostDamageAction postDamageAction))
			{
				return ActionListClearFlag.Keep;
			}
			GuardianVowBinding guardianVowBindingAsGuardian = actor.GuardianVowBindingAsGuardian;
			if (guardianVowBindingAsGuardian == null)
			{
				return ActionListClearFlag.Keep;
			}
			ActorModel actorByActorDefinitionID = combatModel.GetActorByActorDefinitionID(guardianVowBindingAsGuardian.SovereignActorDefinitionID);
			if (actorByActorDefinitionID == null)
			{
				return ActionListClearFlag.Keep;
			}
			DamageAction damageAction = postDamageAction.DamageAction;
			if (damageAction == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (postDamageAction.DamagerActor != actorByActorDefinitionID)
			{
				return ActionListClearFlag.Keep;
			}
			if (!postDamageAction.IsMainTarget)
			{
				return ActionListClearFlag.Keep;
			}
			if (postDamageAction.IsTriggerExtraAttackDamage)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.SourceSupport != null)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.IsPushDamage)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction is DamageConsumableAction)
			{
				return ActionListClearFlag.Keep;
			}
			if (guardianVowBindingAsGuardian.PursuitTriggeredCount >= skill.PursuitMaxTimes)
			{
				return ActionListClearFlag.Keep;
			}
			ActorModel targetActor = postDamageAction.TargetActor;
			if (targetActor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (targetActor.IsDead)
			{
				return ActionListClearFlag.Keep;
			}
			if (targetActor.Faction == actor.Faction)
			{
				return ActionListClearFlag.Keep;
			}
			EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
			if (weaponEquipment == null || weaponEquipment.Ability == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (weaponEquipment.NeedsReloading)
			{
				return ActionListClearFlag.Keep;
			}
			if (weaponEquipment.Ability.CanAbilityBePerformedOnGridCell(combatModel, actor, actor.GridCoordinate, targetActor.GridCoordinate) != AbilityResult.Success)
			{
				return ActionListClearFlag.Keep;
			}
			FixedPoint pursuitChance = skill.PursuitChance;
			if (pursuitChance <= 0.0)
			{
				return ActionListClearFlag.Keep;
			}
			if (actor.IsStunned || actor.IsStruggling)
			{
				return ActionListClearFlag.Keep;
			}
			PlayerModel player = actor.manager.Player;
			if (player == null)
			{
				return ActionListClearFlag.Keep;
			}
			FixedPoint value = 0.0;
			player.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
			PlayerRandomChanceResult playerRandomChanceResult = player.RollDice(RollDiceType.GuardianVowPursuit, pursuitChance, value);
			if (playerRandomChanceResult == PlayerRandomChanceResult.Failed)
			{
				return ActionListClearFlag.Keep;
			}
			addedActions.Add(new AbilityAction(actor, weaponEquipment.Ability, targetActor.GridCoordinate, targetActor, OOTType.PassByAttack, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: true));
			guardianVowBindingAsGuardian.PursuitTriggeredCount++;
			actor.NotifyChange("AbilityVisited", new object[2]
			{
				"GuardianVowSkill",
				playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
			});
			actorByActorDefinitionID.NotifyChange("AbilityVisited", new object[2] { "GuardianVowSkill", false });
			return ActionListClearFlag.Keep;
		}
	}
}
