using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierStatusEffectRemoval : ActionModifier
	{
		private string[] statusEffectsToRemove;

		public AbilityModifierStatusEffectRemoval()
		{
		}

		public AbilityModifierStatusEffectRemoval(string statusEffects)
		{
			statusEffectsToRemove = statusEffects.Split(',');
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is HealAction healAction && (actor == null || actor == healAction.TargetActor))
			{
				string[] array = statusEffectsToRemove;
				foreach (string statusEffect in array)
				{
					RemoveStatusEffect(healAction.TargetActor, statusEffect);
				}
			}
			return ActionListClearFlag.Keep;
		}

		private void RemoveStatusEffect(ActorModel actor, string statusEffect)
		{
			string text = statusEffect.ToLower().Trim();
			if (!(text == "burning"))
			{
				if (text == "bleeding" && actor.HasTrait("Bleeding"))
				{
					actor.RemoveTrait("Bleeding");
				}
			}
			else if (actor.HasTrait("Burning"))
			{
				actor.RemoveTrait("Burning");
			}
		}
	}
}
