using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class WhispererTrait : ActionModifier
	{
		private static readonly int transformTriggerDistance = 2;

		private static readonly string[] characterClasses = new string[6] { "DefaultScout", "DefaultBruiser", "DefaultShooter", "DefaultHunter", "DefaultWarrior", "DefaultAssault" };

		private static readonly string[] meleeCharacterClasses = new string[3] { "DefaultScout", "DefaultBruiser", "DefaultWarrior" };

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (action is DamageAction damageAction && damageAction.TargetActor.HasTraitsThatContains("Whisperer") && combatModel.AllActors.Contains(damageAction.TargetActor) && damageAction.SourceSupport == null)
			{
				if (damageAction.DamageType == DamageType.Ranged || damageAction.DamageType == DamageType.Melee)
				{
					damageAction.ZeroDamage();
				}
				ActorModel targetActor = damageAction.TargetActor;
				TransformWhisperer(targetActor, damageAction.DamagerActor, targetActor.GridCoordinate, addedActions);
			}
			if (action is MoveAction { Replaced: false } moveAction && combatModel.AllActors.Contains(moveAction.Actor))
			{
				bool flag = moveAction.Actor.HasTraitsThatContains("Whisperer");
				if (moveAction.Actor.Faction != Faction.Walker || flag)
				{
					bool flag2 = false;
					for (int i = 1; i < moveAction.Path.Length; i++)
					{
						GridCoordinate gridCoordinate = moveAction.Path[i];
						List<ActorModel> list = (flag ? GetSurvivorsWithinRange(gridCoordinate) : GetWhisperersWithinRange(gridCoordinate));
						if (list.Count <= 0)
						{
							continue;
						}
						if (flag)
						{
							if (combatModel.IsGridCellVisibleByAnySurvivor(moveAction.Actor.GridCoordinate))
							{
								ActorModel actorModel = TransformWhisperer(moveAction.Actor, null, gridCoordinate, addedActions);
								moveAction.Path.ClipTo(gridCoordinate);
								combatModel.TurnManager.NextActorOverride = actorModel;
								actorModel.MoveRangeConsumed = moveAction.Path.Length - 1;
							}
							continue;
						}
						foreach (ActorModel item2 in list)
						{
							if (combatModel.IsGridCellVisible(moveAction.Actor.GridCoordinate, item2.GridCoordinate))
							{
								TransformWhisperer(item2, moveAction.Actor, item2.GridCoordinate, addedActions);
								if (gridCoordinate != moveAction.Path.End && !flag2)
								{
									flag2 = true;
									moveAction.Replaced = true;
									GridPath path = GridPath.Create(new List<GridCoordinate>(moveAction.Path.Path.GetRange(i, moveAction.Path.Path.Count - i)));
									moveAction.Path.ClipTo(gridCoordinate);
									MoveAction item = new MoveAction(moveAction.Actor, path, consumeAP: false);
									addedActions.Add(item);
								}
							}
						}
					}
				}
			}
			if (action is TransformActorAction transformActorAction && transformActorAction.TargetActor.Faction != Faction.Walker)
			{
				ActorModel targetActor2 = transformActorAction.TargetActor;
				List<ActorModel> whisperersWithinRange = GetWhisperersWithinRange(targetActor2.GridCoordinate);
				if (whisperersWithinRange.Count > 0)
				{
					foreach (ActorModel item3 in whisperersWithinRange)
					{
						if (combatModel.IsGridCellVisibleByAnySurvivor(item3.GridCoordinate))
						{
							TransformWhisperer(item3, targetActor2, item3.GridCoordinate, addedActions);
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		private List<ActorModel> GetSurvivorsWithinRange(GridCoordinate coordinate)
		{
			List<ActorModel> list = new List<ActorModel>();
			CombatModel combatModel = base.manager.CombatModel;
			for (int i = 0; i < combatModel.Survivors.Count; i++)
			{
				ActorModel actorModel = combatModel.Survivors[i];
				if (!actorModel.IsDead && DistanceBetweenCells(actorModel.GridCoordinate, coordinate) <= transformTriggerDistance)
				{
					list.Add(actorModel);
				}
			}
			return list;
		}

		private List<ActorModel> GetWhisperersWithinRange(GridCoordinate coordinate)
		{
			List<ActorModel> list = new List<ActorModel>();
			CombatModel combatModel = base.manager.CombatModel;
			for (int i = 0; i < combatModel.Walkers.Count; i++)
			{
				ActorModel actorModel = combatModel.Walkers[i];
				if (!actorModel.IsDead && actorModel.HasTraitsThatContains("Whisperer") && DistanceBetweenCells(actorModel.GridCoordinate, coordinate) <= transformTriggerDistance)
				{
					list.Add(actorModel);
				}
			}
			return list;
		}

		private int DistanceBetweenCells(GridCoordinate a, GridCoordinate b)
		{
			return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
		}

		private ActorModel TransformWhisperer(ActorModel target, ActorModel instigator, GridCoordinate spawnPoint, List<ModelAction> addedActions)
		{
			CombatModel combatModel = base.manager.CombatModel;
			combatModel.RemoveActor(target);
			string text = meleeCharacterClasses[0];
			text = ((!target.HasTraitsThatContains("Whisperer.Melee")) ? base.manager.Player.PlayerRandom.GetRandomElement(characterClasses) : base.manager.Player.PlayerRandom.GetRandomElement(meleeCharacterClasses));
			ActorModel actorModel = combatModel.CreateActor(spawnPoint, Faction.Raider, target.Level, target.ActorTag, text, text, ActorGender.NotSpecified, WalkerVisualization.Normal, RaiderVisualization.Whisperer);
			combatModel.NotifyChange("actorTransformed", actorModel);
			actorModel.SetupForCombat(combatModel);
			actorModel.AIDataModel.Alertness = target.AIDataModel.Alertness;
			actorModel.AIDataModel.Mode = target.AIDataModel.Mode;
			if (target.HasAnyLevelTrait("DebuffMarkEnemy"))
			{
				TraitEntry traitWithTraitIdentifier = target.GetTraitWithTraitIdentifier("DebuffMarkEnemy");
				if (traitWithTraitIdentifier != null)
				{
					actorModel.AddTemporaryTrait(traitWithTraitIdentifier.TraitIdentifier, default(FixedPoint), null, 0L);
				}
			}
			combatModel.UpdateOccupiers();
			addedActions.Add(new TransformActorAction(target, actorModel, instigator));
			target.TurnState = TurnState.Completed;
			return actorModel;
		}
	}
}
