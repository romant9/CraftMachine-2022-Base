using System.Collections.Generic;

namespace TWDModel
{
	public sealed class EquipmentPassiveFightBackTrait : ActionModifier
	{
		private readonly FixedPoint triggerChance;

		private readonly FixedPoint damageMultiplier;

		private readonly int maxTriggersPerRound;

		public const int SortOrder = 8;

		public EquipmentPassiveFightBackTrait(FixedPoint triggerChance, FixedPoint damageMultiplier, int maxTriggersPerRound)
		{
			this.triggerChance = triggerChance;
			this.damageMultiplier = damageMultiplier;
			this.maxTriggersPerRound = maxTriggersPerRound;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (!(action is PostDamageAction postDamageAction) || actor == null || postDamageAction.TargetActor != actor || actor.FightBackTimesThisRound >= maxTriggersPerRound || maxTriggersPerRound <= 0)
			{
				return ActionListClearFlag.Keep;
			}
			CombatModel combatModel = actor.manager?.CombatModel;
			ActorModel damagerActor = postDamageAction.DamagerActor;
			AbilityModel abilityModel = actor.GetWeaponEquipment()?.Ability;
			if (combatModel == null || combatModel.MissionCompleted || combatModel.AbilityManager == null || base.manager?.Player == null || damagerActor == null || damagerActor == actor || damagerActor.IsDead || !actor.IsEnemy(damagerActor) || abilityModel == null)
			{
				return ActionListClearFlag.Keep;
			}
			GridCoordinate targetCell = (damagerActor.IsMultiCell ? damagerActor.GetClosestOccupiedCell(actor.GridCoordinate) : damagerActor.GridCoordinate);
			EquipmentPassiveFightBackAction equipmentPassiveFightBackAction = new EquipmentPassiveFightBackAction(actor, abilityModel, targetCell, damagerActor, damageMultiplier, maxTriggersPerRound);
			if (!equipmentPassiveFightBackAction.CanExecute())
			{
				return ActionListClearFlag.Keep;
			}
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
			PlayerRandomChanceResult playerRandomChanceResult = base.manager.Player.RollDice(RollDiceType.FightBack, triggerChance, value);
			if (playerRandomChanceResult == PlayerRandomChanceResult.Failed)
			{
				return ActionListClearFlag.Keep;
			}
			equipmentPassiveFightBackAction.TriggeredByLuck = playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension;
			equipmentPassiveFightBackAction.SetSortOrder(8);
			addedActions.Add(equipmentPassiveFightBackAction);
			return ActionListClearFlag.Keep;
		}
	}
}
