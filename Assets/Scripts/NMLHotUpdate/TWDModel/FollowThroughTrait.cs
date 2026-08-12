using System.Collections.Generic;

namespace TWDModel
{
	public class FollowThroughTrait : ActionModifier
	{
		private static bool requireSourceActorNeighbour;

		public const string FollowthroughLocalization = "FollowThrough";

		private FixedPoint chance;

		public FollowThroughTrait()
		{
		}

		public FollowThroughTrait(FixedPoint chance)
		{
			this.chance = chance / 100.0;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction && actor != null && postDamageAction.DamagerActor == actor && postDamageAction.TargetActor.IsDead && postDamageAction.TargetActor.Faction != Faction.Environmental && !postDamageAction.DamagerActor.FollowThroughTriggeredInAttack && base.manager.CombatModel.AbilityManager.AbilityUnderApplication != null)
			{
				DamageAction damageAction = postDamageAction.DamageAction;
				CombatModel combatModel = base.manager.CombatModel;
				ActorModel damagerActor = damageAction.DamagerActor;
				ActorModel targetActor = damageAction.TargetActor;
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
				FixedPoint value = 0.0;
				FixedPoint value2 = chance;
				combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseFollowThroughChance", ref value2, actor);
				combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseEquipFollowThroughChance", ref value2, actor);
				if (value2 > 0.0)
				{
					base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.FollowThrough, value2, value);
				}
				if (combatModel != null && damagerActor != null && targetActor != null && playerRandomChanceResult != PlayerRandomChanceResult.Failed)
				{
					FixedPoint value3 = 0.0;
					combatModel.AbilityManager.VisitParameter("AbilityModifierExtraAttackDamageModifier", ref value3, damagerActor);
					combatModel.AbilityManager.VisitParameter("AbilityModifierNewExtraAttackDamageModifier", ref value3, damagerActor);
					CombatHelpers.FollowThrough(damageAction, value, value3, addedActions, requireSourceActorNeighbour, "FollowThrough");
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
