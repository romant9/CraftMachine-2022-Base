using System.Collections.Generic;

namespace TWDModel
{
	public class BraceletsTrait : ActionModifier
	{
		private int range;

		public BraceletsTrait()
		{
		}

		public BraceletsTrait(int range)
		{
			this.range = range;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction)
			{
				if (postDamageAction.DamagerActor != actor || postDamageAction.TargetActor.IsDead || actor.IsStruggling)
				{
					return ActionListClearFlag.Keep;
				}
				DamageAction damageAction = postDamageAction.DamageAction;
				CombatModel combatModel = actor.manager.CombatModel;
				if (combatModel == null)
				{
					return ActionListClearFlag.Keep;
				}
				if (!CombatHelpers.IsWithinRange(combatModel, range, postDamageAction.DamagerActor.GridCoordinate, postDamageAction.TargetActor.GridCoordinate))
				{
					return ActionListClearFlag.Keep;
				}
				addedActions.Add(new CrippleAction(postDamageAction.DamagerActor, postDamageAction.TargetActor, 1, ignoreSourceBeingDead: false, null, () => damageAction.FinalDamage));
				PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.SuccessDueToExtension;
				postDamageAction.DamagerActor.NotifyChange("AbilityVisited", new object[2]
				{
					"Heirlooms_Daryl_Bracelets",
					playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
				});
			}
			return ActionListClearFlag.Keep;
		}
	}
}
