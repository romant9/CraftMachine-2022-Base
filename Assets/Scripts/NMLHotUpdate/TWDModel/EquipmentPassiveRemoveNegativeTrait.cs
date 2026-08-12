using System.Collections.Generic;

namespace TWDModel
{
	public class EquipmentPassiveRemoveNegativeTrait : ActionModifier
	{
		private enum Channel
		{
			EndTurn = 0,
			StartTurn = 1,
			OwnTurn = 2
		}

		private readonly FixedPoint endTurnChance;

		private readonly int endTurnRemoveCount;

		private readonly int endTurnInterval;

		private readonly FixedPoint startTurnChance;

		private readonly int startTurnRemoveCount;

		private readonly int startTurnInterval;

		private readonly FixedPoint ownTurnChance;

		private readonly int ownTurnRemoveCount;

		private readonly int ownTurnInterval;

		private readonly List<string> effectIndex;

		public EquipmentPassiveRemoveNegativeTrait(FixedPoint endTurnChance, int endTurnRemoveCount, int endTurnInterval, FixedPoint startTurnChance, int startTurnRemoveCount, int startTurnInterval, FixedPoint ownTurnChance, int ownTurnRemoveCount, int ownTurnInterval, List<string> effectIndex)
		{
			this.endTurnChance = endTurnChance;
			this.endTurnRemoveCount = endTurnRemoveCount;
			this.endTurnInterval = endTurnInterval;
			this.startTurnChance = startTurnChance;
			this.startTurnRemoveCount = startTurnRemoveCount;
			this.startTurnInterval = startTurnInterval;
			this.ownTurnChance = ownTurnChance;
			this.ownTurnRemoveCount = ownTurnRemoveCount;
			this.ownTurnInterval = ownTurnInterval;
			this.effectIndex = ((effectIndex != null) ? new List<string>(effectIndex) : new List<string>());
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action == null || actor == null || actor.IsDead)
			{
				return ActionListClearFlag.Keep;
			}
			CombatModel combatModel = ((base.manager != null) ? base.manager.CombatModel : null);
			if (combatModel == null || combatModel.TurnManager == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is PreChangeTurnAction preChangeTurnAction)
			{
				if (preChangeTurnAction.NextActiveFaction == Faction.Survivor)
				{
					TryRunChannel(actor, combatModel, Channel.EndTurn);
				}
			}
			else if (action is PostChangeTurnAction)
			{
				if (combatModel.TurnManager.ActiveFaction == Faction.Survivor && combatModel.TurnManager.TurnCount > 0)
				{
					TryRunChannel(actor, combatModel, Channel.StartTurn);
				}
			}
			else if (action is ActiveActorChangedAction activeActorChangedAction && activeActorChangedAction.NewActor == actor && !IsIncapacitated(actor))
			{
				TryRunChannel(actor, combatModel, Channel.OwnTurn);
			}
			return ActionListClearFlag.Keep;
		}

		private static bool IsIncapacitated(ActorModel actor)
		{
			if (actor.AIController != null)
			{
				return actor.AIController.IsActorIncapacitated;
			}
			return false;
		}

		private void TryRunChannel(ActorModel actor, CombatModel combat, Channel channel)
		{
			GetChannelConfig(channel, actor, out var chance, out var removeCount, out var interval, out var nextReadyTurn);
			int turnCount = combat.TurnManager.TurnCount;
			if (turnCount >= nextReadyTurn)
			{
				FixedPoint value = 0.0;
				combat.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.EquipmentPassiveRemoveNegative, chance, value) != PlayerRandomChanceResult.Failed && RemoveRandomNegatives(actor, combat, removeCount) >= 1)
				{
					actor.NotifyChange("EquipmentPassiveRemoveNegativeVisited");
					SetNextReadyTurn(channel, actor, turnCount + interval + 1);
				}
			}
		}

		private void GetChannelConfig(Channel channel, ActorModel actor, out FixedPoint chance, out int removeCount, out int interval, out int nextReadyTurn)
		{
			switch (channel)
			{
			case Channel.EndTurn:
				chance = endTurnChance;
				removeCount = endTurnRemoveCount;
				interval = endTurnInterval;
				nextReadyTurn = actor.NextReadyEquipmentPassiveRemoveNegativeEndTurn;
				break;
			case Channel.StartTurn:
				chance = startTurnChance;
				removeCount = startTurnRemoveCount;
				interval = startTurnInterval;
				nextReadyTurn = actor.NextReadyEquipmentPassiveRemoveNegativeStartTurn;
				break;
			default:
				chance = ownTurnChance;
				removeCount = ownTurnRemoveCount;
				interval = ownTurnInterval;
				nextReadyTurn = actor.NextReadyEquipmentPassiveRemoveNegativeOwnTurn;
				break;
			}
		}

		private static void SetNextReadyTurn(Channel channel, ActorModel actor, int nextReadyTurn)
		{
			switch (channel)
			{
			case Channel.EndTurn:
				actor.NextReadyEquipmentPassiveRemoveNegativeEndTurn = nextReadyTurn;
				break;
			case Channel.StartTurn:
				actor.NextReadyEquipmentPassiveRemoveNegativeStartTurn = nextReadyTurn;
				break;
			default:
				actor.NextReadyEquipmentPassiveRemoveNegativeOwnTurn = nextReadyTurn;
				break;
			}
		}

		private int RemoveRandomNegatives(ActorModel actor, CombatModel combat, int removeCount)
		{
			if (removeCount <= 0 || effectIndex == null || effectIndex.Count == 0)
			{
				return 0;
			}
			List<string> list = CollectActiveNegatives(actor);
			if (list.Count == 0)
			{
				return 0;
			}
			List<string> list2;
			if (removeCount >= list.Count)
			{
				list2 = list;
			}
			else
			{
				list2 = new List<string>(removeCount);
				for (int i = 0; i < removeCount; i++)
				{
					list2.Add(base.manager.Player.PlayerRandom.GetRandomElement(list, remove: true));
				}
			}
			int num = 0;
			for (int j = 0; j < list2.Count; j++)
			{
				if (actor.TryRemoveNegativeEffectByName(combat, list2[j]))
				{
					num++;
				}
			}
			return num;
		}

		private List<string> CollectActiveNegatives(ActorModel actor)
		{
			List<string> list = new List<string>();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string item in effectIndex)
			{
				if (string.IsNullOrEmpty(item))
				{
					continue;
				}
				string text = item.Trim();
				if (!hashSet.Contains(text))
				{
					hashSet.Add(text);
					if (actor.HasRemovableNegativeEffect(text))
					{
						list.Add(text);
					}
				}
			}
			return list;
		}
	}
}
