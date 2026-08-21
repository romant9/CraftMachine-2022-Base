using System.Collections.Generic;

namespace TWDModel
{
	public class RippedTrait : ActionModifier
	{
		private FixedPoint RootPercent;

		private int RootTurns;

		public RippedTrait(FixedPoint rootPercent, int rootTurns)
		{
			RootPercent = rootPercent;
			RootTurns = rootTurns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null && postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null)
			{
				if (!postDamageAction.TargetActor.HasAnyLevelTrait("Skinned"))
				{
					return ActionListClearFlag.Keep;
				}
				if (ResistNegativeEffectsTrait.TryResist(postDamageAction.TargetActor, "Ripped"))
				{
					return ActionListClearFlag.Keep;
				}
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.Ripped, RootPercent, value) != PlayerRandomChanceResult.Failed)
				{
					int rootTurns = RootTurns;
					addedActions.Add(new RootAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, rootTurns, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
