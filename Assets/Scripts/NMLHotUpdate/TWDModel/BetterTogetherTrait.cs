using System.Collections.Generic;

namespace TWDModel
{
	public class BetterTogetherTrait : ActionModifier
	{
		private CombatModel combatModel;

		private bool isLeaderTrait = true;

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PostChangeTurnAction)
			{
				combatModel = base.manager.CombatModel;
				if (combatModel != null && !combatModel.MissionCompleted && combatModel.TurnManager.TurnCount > 0)
				{
					if (actor.Faction != combatModel.TurnManager.ActiveFaction)
					{
						return ActionListClearFlag.Keep;
					}
					List<ActorModel> factionActorsInCellDistance = GetFactionActorsInCellDistance(combatModel, actor);
					if (factionActorsInCellDistance == null || factionActorsInCellDistance.Count <= 0)
					{
						return ActionListClearFlag.Keep;
					}
					HandleActorsInRangeAsTeamMember(factionActorsInCellDistance, actor);
					actor.BetterTogetherMultiplier = (isLeaderTrait ? (factionActorsInCellDistance.Count * 2) : factionActorsInCellDistance.Count);
					actor.NotifyChange("BetterTogetherCountChanged");
					if (actor.Faction == Faction.Survivor)
					{
						for (int i = 0; i < actor.BetterTogetherMultiplier; i++)
						{
							RollChargePoint(actor, actor);
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private List<ActorModel> GetFactionActorsInCellDistance(CombatModel combatModel, ActorModel sourceActor)
		{
			List<ActorModel> list = new List<ActorModel>();
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("LeaderBuffBetterTogetherSurvivorDistance", ref value, sourceActor);
			if (value <= 0.0)
			{
				return null;
			}
			if (sourceActor != null && combatModel != null)
			{
				List<ActorModel> factionActors = combatModel.GetFactionActors(sourceActor.Faction);
				GridCoordinate gridCoordinate = sourceActor.GridCoordinate;
				foreach (ActorModel item in factionActors)
				{
					if (!item.IsDead && !item.IsEnvironmental && item != sourceActor && gridCoordinate.ChebyshevDistance(item.GridCoordinate) <= value)
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		private void HandleActorsInRangeAsTeamMember(IEnumerable<ActorModel> survivorsInRange, ActorModel sourceActor)
		{
			foreach (ActorModel item in survivorsInRange)
			{
				if (!item.HasAnyLevelTrait("BaseBetterTogether"))
				{
					item.BetterTogetherMultiplier = 1;
					if (sourceActor.Faction == Faction.Survivor)
					{
						RollChargePoint(sourceActor, item);
					}
					isLeaderTrait = false;
				}
			}
		}

		private void RollChargePoint(ActorModel traitSourceActor, ActorModel chargePointsToActor)
		{
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("LeaderBuffBetterTogetherExtraChargePointChance", ref value, traitSourceActor);
			FixedPoint value2 = 0.0;
			combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value2, chargePointsToActor);
			PlayerRandomChanceResult playerRandomChanceResult = combatModel.AbilityManager.manager.Player.RollDice(RollDiceType.FollowThrough, value, value2);
			if (playerRandomChanceResult != PlayerRandomChanceResult.Failed && chargePointsToActor.ChargeMeter.ChargeLevel < chargePointsToActor.ChargeMeter.MaxLevel)
			{
				chargePointsToActor.AddChargePoints(1);
				chargePointsToActor.NotifyChange("AbilityVisited", new object[2]
				{
					"LeaderBuffBetterTogether",
					playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
				});
			}
		}
	}
}
