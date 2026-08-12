using System.Collections.Generic;

namespace TWDModel
{
	public class BloodFrenzyTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityBeforeRemoveActiveTraitAction abilityBeforeRemoveActiveTraitAction && abilityBeforeRemoveActiveTraitAction.Source == actor && abilityBeforeRemoveActiveTraitAction.Source.Faction == actor.Faction && abilityBeforeRemoveActiveTraitAction.AbilityAction != null && abilityBeforeRemoveActiveTraitAction.AbilityAction.IsFromAbilityCommand)
			{
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("Equipment_Active_BloodFrenzy_Hp", ref value, abilityBeforeRemoveActiveTraitAction.Source);
				if (abilityBeforeRemoveActiveTraitAction.Source.Hitpoints > 0)
				{
					int num = (int)(abilityBeforeRemoveActiveTraitAction.Source.Hitpoints - abilityBeforeRemoveActiveTraitAction.Source.Hitpoints * value);
					if (num <= 0)
					{
						num = 1;
					}
					abilityBeforeRemoveActiveTraitAction.Source.SetHitpoints(num, DefenseSystemType.None, IsDealShield: false);
					abilityBeforeRemoveActiveTraitAction.Source.NotifyChange("ActorHealthChanged");
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
