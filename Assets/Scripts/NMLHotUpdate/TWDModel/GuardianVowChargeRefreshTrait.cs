using System.Collections.Generic;

namespace TWDModel
{
	public class GuardianVowChargeRefreshTrait : ActionModifier
	{
		private GuardianVowSkill skill;

		public GuardianVowChargeRefreshTrait(GuardianVowSkill ownerSkill)
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
			if (!(action is PostAbilityExecuteAction postAbilityExecuteAction))
			{
				return ActionListClearFlag.Keep;
			}
			GuardianVowBinding guardianVowBindingAsGuardian = actor.GuardianVowBindingAsGuardian;
			if (guardianVowBindingAsGuardian == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (guardianVowBindingAsGuardian.ChargeAttackMaxTimes <= 0)
			{
				return ActionListClearFlag.Keep;
			}
			if (guardianVowBindingAsGuardian.ChargeRefreshUsedThisTurn >= guardianVowBindingAsGuardian.ChargeAttackMaxTimes)
			{
				return ActionListClearFlag.Keep;
			}
			ActorModel actorByActorDefinitionID = combatModel.GetActorByActorDefinitionID(guardianVowBindingAsGuardian.SovereignActorDefinitionID);
			if (actorByActorDefinitionID == null || actorByActorDefinitionID.IsDead)
			{
				return ActionListClearFlag.Keep;
			}
			if (postAbilityExecuteAction.DamagerActor != actorByActorDefinitionID)
			{
				return ActionListClearFlag.Keep;
			}
			if (!actorByActorDefinitionID.UsedChargeAttackThisTurn)
			{
				return ActionListClearFlag.Keep;
			}
			actorByActorDefinitionID.UsedChargeAttackThisTurn = false;
			guardianVowBindingAsGuardian.ChargeRefreshUsedThisTurn++;
			actorByActorDefinitionID.NotifyChange("UpdateGuardianVowEvent");
			return ActionListClearFlag.Keep;
		}
	}
}
