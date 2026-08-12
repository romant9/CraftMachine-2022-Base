using System.Collections.Generic;

namespace TWDModel
{
	public class GuardianVowTransferTrait : ActionModifier
	{
		private GuardianVowSkill skill;

		public GuardianVowTransferTrait(GuardianVowSkill ownerSkill)
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
			if (actor.IsDead)
			{
				return ActionListClearFlag.Keep;
			}
			if (!(action is PreDealDamageAction { DamageAction: var damageAction } preDealDamageAction))
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.DamageType == DamageType.GuardianVowTransfer)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.Dodged)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.DamageType == DamageType.Fire)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.DamageType == DamageType.Heal)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.DamageType == DamageType.Bleeding)
			{
				return ActionListClearFlag.Keep;
			}
			GuardianVowBinding guardianVowBindingAsGuardian = actor.GuardianVowBindingAsGuardian;
			if (guardianVowBindingAsGuardian == null)
			{
				return ActionListClearFlag.Keep;
			}
			CombatModel combatModel = actor.manager?.CombatModel;
			if (combatModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			ActorModel actorByActorDefinitionID = combatModel.GetActorByActorDefinitionID(guardianVowBindingAsGuardian.SovereignActorDefinitionID);
			if (actorByActorDefinitionID == null || actorByActorDefinitionID.IsDead)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.TargetActor != actorByActorDefinitionID)
			{
				return ActionListClearFlag.Keep;
			}
			if (damageAction.DamagerActor == actor)
			{
				return ActionListClearFlag.Keep;
			}
			if (skill.GuardRange <= 0)
			{
				return ActionListClearFlag.Keep;
			}
			if (skill.TransferRatio <= 0.0)
			{
				return ActionListClearFlag.Keep;
			}
			if (!CombatHelpers.IsWithinRange(combatModel, skill.GuardRange, actor.GridCoordinate, actorByActorDefinitionID.GridCoordinate))
			{
				return ActionListClearFlag.Keep;
			}
			int[] array = CombatHelpers.GuardianVowTransferDamageCalculation(combatModel, preDealDamageAction, actorByActorDefinitionID, actor, damageAction.DamageType, damageAction.BaseDamage, damageAction.AdditionalCriticalDamage, skill.TransferRatio, skill.TransferReduction);
			int newDamage = array[0];
			int newAdditionalCriticalDamage = array[1];
			int num = array[2];
			int num2 = array[3];
			damageAction.UpBaseDamage(newDamage);
			damageAction.UpAdditionalCriticalDamage(newAdditionalCriticalDamage);
			if (num + num2 <= 0)
			{
				return ActionListClearFlag.Keep;
			}
			int num3 = num;
			int num4 = num2;
			ActorModel helpHandActor = CombatHelpers.getHelpHandActor(combatModel, actor);
			if (helpHandActor != null && helpHandActor != actorByActorDefinitionID && helpHandActor != actor && !helpHandActor.IsDead)
			{
				int[] array2 = CombatHelpers.HelpHandDamageCalculation(combatModel, preDealDamageAction, actor, helpHandActor, damageAction.DamageType, num, num2);
				num3 = array2[0];
				num4 = array2[1];
				int num5 = array2[2] + array2[3];
				if (num5 > 0)
				{
					CombatHelpers.ExecuteDamage(combatModel, null, helpHandActor, num5, 0, DamageType.HelpHand, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
					helpHandActor.NotifyChange("AbilityVisited", new object[2] { "HelpHand", false });
				}
			}
			int num6 = num3 + num4;
			if (num6 <= 0)
			{
				return ActionListClearFlag.Keep;
			}
			CombatHelpers.ExecuteDamage(combatModel, null, actor, num6, 0, DamageType.GuardianVowTransfer, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
			actor.NotifyChange("AbilityVisited", new object[2] { "GuardianVowSkill", false });
			actorByActorDefinitionID.NotifyChange("AbilityVisited", new object[2] { "GuardianVowSkill", false });
			return ActionListClearFlag.Keep;
		}
	}
}
