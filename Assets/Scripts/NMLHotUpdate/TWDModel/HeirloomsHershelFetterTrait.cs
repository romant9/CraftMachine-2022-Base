using System.Collections.Generic;

namespace TWDModel
{
	public class HeirloomsHershelFetterTrait : ActionModifier
	{
		public FixedPoint Probability;

		public FixedPoint Floor;

		public FixedPoint RandomNumber;

		public FixedPoint AddFloor;

		public FixedPoint Raise;

		public FixedPoint RaiseProbability;

		public FixedPoint BurnDmgDiminish;

		public FixedPoint Limit;

		public FixedPoint Round;

		public HeirloomsHershelFetterTrait(FixedPoint probability, FixedPoint floor, FixedPoint randomNumber, FixedPoint addFloor, FixedPoint raise, FixedPoint raiseProbability, FixedPoint burnDmgDiminish, FixedPoint limit, FixedPoint round)
		{
			Probability = probability;
			Floor = floor;
			RandomNumber = randomNumber;
			AddFloor = addFloor;
			Raise = raise;
			RaiseProbability = raiseProbability;
			BurnDmgDiminish = burnDmgDiminish;
			Limit = limit;
			Round = round;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is DamageAction { DamagerActor: not null } damageAction && actor == damageAction.DamagerActor && damageAction.TargetActor.Faction != Faction.Environmental && damageAction.DamagerActor.Faction != Faction.Environmental && !damageAction.TargetActor.IsDead && damageAction.SourceSupport == null && !(damageAction is DamageConsumableAction))
			{
				FixedPoint value = 0.0;
				if (Probability != 0.0)
				{
					base.manager.Player.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
					if (actor.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, Probability, value) != PlayerRandomChanceResult.Failed)
					{
						Dictionary<Faction, HeirloomsHershelFetter> heirloomsHershelFetterFloor = damageAction.TargetActor.HeirloomsHershelFetterFloor;
						if (heirloomsHershelFetterFloor == null || heirloomsHershelFetterFloor.Count == 0 || !heirloomsHershelFetterFloor.ContainsKey(damageAction.DamagerActor.Faction))
						{
							damageAction.TargetActor.HeirloomsHershelFetterFloor = new Dictionary<Faction, HeirloomsHershelFetter>();
							HeirloomsHershelFetter heirloomsHershelFetter = new HeirloomsHershelFetter();
							heirloomsHershelFetter.Floor = Floor;
							heirloomsHershelFetter.Roundm = Round;
							heirloomsHershelFetter.UpdateAttributeValueNew(damageAction.TargetActor, damageAction.DamagerActor, Raise, RaiseProbability, BurnDmgDiminish);
							damageAction.TargetActor.HeirloomsHershelFetterFloor.Add(damageAction.DamagerActor.Faction, heirloomsHershelFetter);
						}
						else
						{
							HeirloomsHershelFetter heirloomsHershelFetter2 = damageAction.TargetActor.HeirloomsHershelFetterFloor?[damageAction.DamagerActor.Faction];
							if (heirloomsHershelFetter2.Floor < Limit)
							{
								if (heirloomsHershelFetter2.Floor + Floor > Limit)
								{
									heirloomsHershelFetter2.Floor = Limit;
									heirloomsHershelFetter2.Roundm = Round;
								}
								else
								{
									heirloomsHershelFetter2.Floor += Floor;
									heirloomsHershelFetter2.Roundm = Round;
								}
								heirloomsHershelFetter2.UpdateAttributeValueNew(damageAction.TargetActor, damageAction.DamagerActor, Raise, RaiseProbability, BurnDmgDiminish);
							}
							else
							{
								heirloomsHershelFetter2.Roundm = Round;
							}
						}
						SendMessage(damageAction.TargetActor);
					}
				}
			}
			if (action is PostDamageAction { TargetActor: not null } postDamageAction && postDamageAction.TargetActor.Faction != Faction.Environmental && postDamageAction.TargetActor.IsDead && postDamageAction.TargetActor.HeirloomsHershelFetterFloor != null && postDamageAction.TargetActor.HeirloomsHershelFetterFloor.Count > 0)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				List<ActorModel> list = actor.GridCoordinate.GetEnemiesByDistance(postDamageAction.TargetActor.GridCoordinate, combatModel, 1);
				int count = list.Count;
				if (list.Count <= 0)
				{
					return ActionListClearFlag.Keep;
				}
				if (count > 2)
				{
					list = actor.manager.Player.PlayerRandom.GetRandomRange(list, (int)RandomNumber);
				}
				foreach (ActorModel item in list)
				{
					if (!actor.IsDead && !actor.IsEnvironmental)
					{
						base.manager.ExecuteAction(new BurningOutAction(null, item, onRedHealthBar: false));
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private static void SendMessage(ActorModel target)
		{
			target.NotifyChange("ActorHeirloomsHershelFetterUpdate");
			target.NotifyChange("ActorHeirloomsHershelFetterMessage");
		}
	}
}
