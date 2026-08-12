using System.Collections.Generic;

namespace TWDModel
{
	public class FocusModeTrait : ActionModifier
	{
		private bool isChargeAttack;

		private FixedPoint FocusModeChance;

		private FixedPoint FocusModeCoolOff;

		public FocusModeTrait()
		{
		}

		public FocusModeTrait(FixedPoint focusModeChance, FixedPoint focusModeCoolOff)
		{
			FocusModeChance = focusModeChance;
			FocusModeCoolOff = focusModeCoolOff;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction abilityAction && abilityAction.Actor == actor && !abilityAction.Ability.IsConsumableAbility)
			{
				isChargeAttack = abilityAction.Ability.IsChargeAttack;
				if (isChargeAttack)
				{
					if (actor.FocusModeState)
					{
						actor.FocusModeState = false;
						actor.FocusCoolOff = (int)FocusModeCoolOff;
						actor.FocusModeStateChargeCD = true;
						actor.NotifyChange("AbortFocusMode");
						actor.NotifyChange("HideFocusModeBTN");
						actor.NotifyChange("AbilityVisited", new object[2] { "FocusMode", false });
					}
					else
					{
						actor.FocusModeState = true;
						actor.FocusCoolOff = (int)FocusModeCoolOff;
						actor.FocusModeStateChargeCD = true;
						actor.NotifyChange("JoinFocusMode");
						actor.NotifyChange("AbilityVisited", new object[2] { "FocusMode", false });
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
