using System.Collections.Generic;

namespace TWDModel
{
	public class PerseveranceTrait : ActionModifier
	{
		private readonly int extraChargePointChance;

		public PerseveranceTrait(int extraChargePointChance)
		{
			this.extraChargePointChance = extraChargePointChance;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostDamageAction postDamageAction)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (combatModel != null && !combatModel.MissionCompleted)
				{
					TraitEntry traitWithTraitIdentifier = actor.GetTraitWithTraitIdentifier("Perseverance");
					if (actor.Faction != combatModel.TurnManager.ActiveFaction || traitWithTraitIdentifier == null || postDamageAction.TargetActor.IsDead || actor != postDamageAction.DamagerActor || actor.IsDead || actor.ChargeMeter.ChargeLevel == actor.ChargeMeter.MaxLevel || actor.SelectedAbility.IsChargeAttack || (postDamageAction.DamageAction.DealDamagePostAbility && postDamageAction.DamageAction.TargetActor.Hitpoints - postDamageAction.DamageAction.FinalDamage <= 0) || (!postDamageAction.DamageAction.DealDamagePostAbility && postDamageAction.DamageAction.FinalDamage == 0) || postDamageAction.DamageAction.SourceSupport != null)
					{
						return ActionListClearFlag.Keep;
					}
					PlayerRandomChanceResult playerRandomChanceResult = PlayerRandomChanceResult.Failed;
					if (extraChargePointChance > 0)
					{
						FixedPoint value = 0.0;
						base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
						playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.GainChargePoint, (FixedPoint)extraChargePointChance / (FixedPoint)100.0, value);
					}
					if (playerRandomChanceResult != PlayerRandomChanceResult.Failed && actor.ChargeMeter != null)
					{
						actor.AddChargePoints(1);
						actor.NotifyChange("AbilityVisited", new object[2]
						{
							"Perseverance",
							playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
						});
					}
					return ActionListClearFlag.Keep;
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
