using System.Collections.Generic;

namespace TWDModel
{
	public class TacticalResupplyTrait : ActionModifier
	{
		public enum DeferTacticalResupplyType
		{
			None = 0,
			Enemy = 1,
			InteractiveObject = 2
		}

		private FixedPoint DropMagazinePercentage { get; set; }

		private int DropMagazineRadius { get; set; }

		private int DropMagazineCount { get; set; }

		private int MagazineDuration { get; set; }

		private int MaxMagazinesPerActor { get; set; }

		private DeferTacticalResupplyType ResupplyType { get; set; }

		public TacticalResupplyTrait(FixedPoint dropMagazinePercentage, int dropMagazineRadius, int dropMagazineCount, int magazineDuration, int maxMagazinesPerActor)
		{
			DropMagazinePercentage = dropMagazinePercentage;
			DropMagazineRadius = dropMagazineRadius;
			DropMagazineCount = dropMagazineCount;
			MagazineDuration = magazineDuration;
			MaxMagazinesPerActor = maxMagazinesPerActor;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is AbilityAction { IsFromAbilityCommand: not false, Actor: not null } abilityAction && abilityAction.Actor == actor)
			{
				FixedPoint value = 0.0;
				base.manager.CombatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, actor);
				if (base.manager.Player.RollDice(RollDiceType.DropMagazine, DropMagazinePercentage, value) != PlayerRandomChanceResult.Failed)
				{
					addedActions.Add(new DropMagazineAction(actor, DropMagazineRadius, DropMagazineCount, MagazineDuration, MaxMagazinesPerActor, "TacticalResupply"));
				}
			}
			if (action is PostMoveSuccessAction { Actor: not null } postMoveSuccessAction && postMoveSuccessAction.Actor == actor)
			{
				MagazineArea magazineArea = TryFindMagazineAtActorPosition(actor);
				if (magazineArea != null)
				{
					PickUpMagazine(actor, magazineArea);
					if (!postMoveSuccessAction.MoveAction.Path.HasTargetCoordinate)
					{
						ResumeOneAP(actor);
					}
					else
					{
						GridCoordinate targetCoordinate = postMoveSuccessAction.MoveAction.Path.TargetCoordinate;
						bool flag = base.manager.CombatModel.IsInteractiveObjectCoordinate(targetCoordinate);
						ActorModel occupier = base.manager.CombatModel.GetOccupier(targetCoordinate);
						if (occupier != null && actor.IsEnemy(occupier))
						{
							ResupplyType = DeferTacticalResupplyType.Enemy;
						}
						else if (flag)
						{
							ResupplyType = DeferTacticalResupplyType.InteractiveObject;
						}
					}
				}
			}
			if (action is PostAbilityExecuteAction { DamagerActor: not null } postAbilityExecuteAction && postAbilityExecuteAction.DamagerActor == actor && ResupplyType == DeferTacticalResupplyType.Enemy)
			{
				ResumeOneAP(actor);
				ResupplyType = DeferTacticalResupplyType.None;
			}
			if (action is InteractiveObjectFinishedAction { SourceActor: not null } interactiveObjectFinishedAction && interactiveObjectFinishedAction.SourceActor == actor && ResupplyType == DeferTacticalResupplyType.InteractiveObject)
			{
				ResumeOneAP(actor);
				ResupplyType = DeferTacticalResupplyType.None;
			}
			return ActionListClearFlag.Keep;
		}

		private void ResumeOneAP(ActorModel actor)
		{
			if (actor.AbilityCompleted)
			{
				if (!actor.IsInteractingWithObject)
				{
					actor.TurnState = TurnState.Idle;
				}
				actor.SecondMoveCompleted = false;
				actor.AbilityCompleted = false;
			}
			else if (actor.MoveCompleted)
			{
				if (actor.SecondMoveCompleted)
				{
					actor.SecondMoveCompleted = false;
				}
				else
				{
					actor.MoveCompleted = false;
				}
			}
			actor.NotifyChange("RefreshCommandSkill");
			actor.NotifyChange("actorExtraAbilityAction");
		}

		private MagazineArea TryFindMagazineAtActorPosition(ActorModel actor)
		{
			foreach (MagazineArea model in base.manager.CombatModel.GetModels<MagazineArea>())
			{
				if (!(model.Coordinate != actor.GridCoordinate) && model.Faction == actor.Faction && !string.IsNullOrEmpty(model.RequiredTraitIdentifier) && actor.HasTraitsThatContains(model.RequiredTraitIdentifier))
				{
					return model;
				}
			}
			return null;
		}

		private void PickUpMagazine(ActorModel actor, MagazineArea magazine)
		{
			actor.TacticalResupplyMagazineNextDragLineCritPending = true;
			base.manager.CombatModel.RemoveModel(magazine);
			base.manager.CombatModel.NotifyChange("MagazineAreasUpdate");
		}
	}
}
