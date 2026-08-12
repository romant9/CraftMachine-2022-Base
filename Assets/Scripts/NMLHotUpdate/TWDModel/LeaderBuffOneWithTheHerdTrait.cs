using System.Collections.Generic;

namespace TWDModel
{
	public class LeaderBuffOneWithTheHerdTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor == null)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is MoveAction { Replaced: false } moveAction)
			{
				if (moveAction.Actor != actor)
				{
					return ActionListClearFlag.Keep;
				}
				if (moveAction.Actor.HasAnyLevelTrait("LeaderBuffOneWithTheHerdStalker"))
				{
					return ActionListClearFlag.Keep;
				}
				if (moveAction.Actor.IsInvisible)
				{
					return ActionListClearFlag.Keep;
				}
				if (!moveAction.CanBeInterruptedForPassByPull)
				{
					return ActionListClearFlag.Keep;
				}
				GridCoordinate end = moveAction.Path.End;
				GetIntersectingCoordinates(base.manager, moveAction.Path, out var intersectingCoordinateList, out var walkersToAddToTheHerd, isStalker: false);
				bool consumeAP = moveAction.Path.MoveDistance > moveAction.Actor.MoveRange;
				MoveAction moveAction2 = moveAction;
				actor.GainedChargePointOnMove = false;
				for (int i = 0; i < walkersToAddToTheHerd.Count; i++)
				{
					GridPath gridPath = GridPath.Create(moveAction2.Path);
					GridCoordinate gridCoordinate = intersectingCoordinateList[i];
					ActorModel actorModel = walkersToAddToTheHerd[i];
					if (!(gridCoordinate != GridCoordinate.Invalid) || actorModel.HasTraitsThatContains("Whisperer"))
					{
						continue;
					}
					gridPath.ClipFromStartUntil(gridCoordinate);
					HerdAction item = new HerdAction(actor, actorModel, 1, i);
					addedActions.Add(item);
					actor.GridCoordinate = gridCoordinate;
					if (moveAction2.Path.Start != gridCoordinate)
					{
						moveAction2.Path.ClipTo(gridCoordinate);
						if ((i <= 0 || !(gridCoordinate == intersectingCoordinateList[i - 1])) && gridCoordinate != end && gridPath.IsValid)
						{
							moveAction2.Replaced = true;
							moveAction2 = new MoveAction(moveAction.Actor, gridPath, consumeAP, globallyBlocking: true);
							moveAction2.CanBeInterruptedForPassByPull = false;
							addedActions.Add(moveAction2);
							actor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffOneWithTheHerd", false });
						}
					}
				}
			}
			return ActionListClearFlag.Keep;
		}

		public static void GetIntersectingCoordinates(TWDModelManager manager, GridPath path, out List<GridCoordinate> intersectingCoordinateList, out List<ActorModel> walkersToAddToTheHerd, bool isStalker)
		{
			walkersToAddToTheHerd = new List<ActorModel>();
			intersectingCoordinateList = new List<GridCoordinate>();
			List<GridCoordinate> list = new List<GridCoordinate>(5);
			CombatModel combatModel = manager.CombatModel;
			GridModel grid = combatModel.Grid;
			for (int i = 0; i < combatModel.Walkers.Count; i++)
			{
				ActorModel actorModel = combatModel.Walkers[i];
				if (!actorModel.IsVisibleToSurvivors || (!isStalker && actorModel.ExclusiveTimedEffect != null && actorModel.ExclusiveTimedEffect.Type != TimedEffectType.Herd) || (path.HasTargetCoordinate && path.TargetCoordinate == actorModel.GridCoordinate))
				{
					continue;
				}
				list.Clear();
				foreach (GridCoordinate item in grid.Neighbors(actorModel.GridCoordinate))
				{
					if (path.Contains(item) && combatModel.GetOccupier(item) == null && combatModel.CanTraverse(null, item, actorModel.GridCoordinate) && combatModel.IsGridCellVisible(item, actorModel.GridCoordinate) && item != path.End)
					{
						list.Add(item);
					}
				}
				if (list.Count >= 2)
				{
					intersectingCoordinateList.Add(list[0]);
					walkersToAddToTheHerd.Add(actorModel);
				}
			}
			for (int j = 0; j <= intersectingCoordinateList.Count - 2; j++)
			{
				for (int k = 0; k <= intersectingCoordinateList.Count - 2; k++)
				{
					if (path.Path.IndexOf(intersectingCoordinateList[k]) > path.Path.IndexOf(intersectingCoordinateList[k + 1]) || (path.Path.IndexOf(intersectingCoordinateList[k]) == path.Path.IndexOf(intersectingCoordinateList[k + 1]) && walkersToAddToTheHerd[k].GridCoordinate.DistanceTo(path.Start) > walkersToAddToTheHerd[k + 1].GridCoordinate.DistanceTo(path.Start)))
					{
						GridCoordinate value = intersectingCoordinateList[k + 1];
						intersectingCoordinateList[k + 1] = intersectingCoordinateList[k];
						intersectingCoordinateList[k] = value;
						ActorModel value2 = walkersToAddToTheHerd[k + 1];
						walkersToAddToTheHerd[k + 1] = walkersToAddToTheHerd[k];
						walkersToAddToTheHerd[k] = value2;
					}
				}
			}
			if (!isStalker || walkersToAddToTheHerd.Count == 0)
			{
				return;
			}
			for (int num = walkersToAddToTheHerd.Count - 1; num > 0; num--)
			{
				if (walkersToAddToTheHerd[num].ExclusiveTimedEffect != null && walkersToAddToTheHerd[num].ExclusiveTimedEffect.Type != TimedEffectType.Herd)
				{
					walkersToAddToTheHerd.RemoveAt(num);
				}
			}
		}
	}
}
