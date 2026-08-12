using System.Collections.Generic;

namespace TWDModel
{
	public class SkinnedTrait : ActionModifier
	{
		private FixedPoint Percent;

		private int Turns;

		public SkinnedTrait(FixedPoint makeSkinnedPercent, int makeSkinnedTurns)
		{
			Percent = makeSkinnedPercent;
			Turns = makeSkinnedTurns;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			PostDamageAction postDamageAction = action as PostDamageAction;
			if (postDamageAction != null && postDamageAction.DamagerActor == actor && !postDamageAction.TargetActor.IsDead && postDamageAction.DamageAction.SourceSupport == null)
			{
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.Skinned, Percent, value) != PlayerRandomChanceResult.Failed)
				{
					int num = Turns;
					if (actor.Faction != base.manager.CombatModel.TurnManager.ActiveFaction)
					{
						num++;
					}
					postDamageAction.TargetActor.NotifyChange("AbilityVisited", new object[2] { "Skinned", false });
					addedActions.Add(new SkinnedAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, num, resetTurn: true, ignoreSourceBeingDead: false, null, () => postDamageAction.DamageAction.FinalDamage));
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
