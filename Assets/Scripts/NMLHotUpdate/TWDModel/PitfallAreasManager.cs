using System;
using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	public class PitfallAreasManager : CombatAreasManager
	{
		public FixedPoint MaxAreaCount;

		public Dictionary<string, int> ActorCooldownUntilTurns;

		protected override CombatAreaType CombatAreaType => CombatAreaType.Pitfall;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		public PitfallAreasManager()
		{
		}

		public PitfallAreasManager(FixedPoint maxAreaCount)
		{
			MaxAreaCount = maxAreaCount;
			ActorCooldownUntilTurns = new Dictionary<string, int>();
		}

		public int GetActorCooldownUntilTurn(string actorName)
		{
			if (ActorCooldownUntilTurns.ContainsKey(actorName))
			{
				return ActorCooldownUntilTurns[actorName];
			}
			return 0;
		}

		public void SetActorCooldownUntilTurn(string actorName, int unitlTurn)
		{
			if (ActorCooldownUntilTurns.ContainsKey(actorName))
			{
				ActorCooldownUntilTurns[actorName] = unitlTurn;
			}
			else
			{
				ActorCooldownUntilTurns.Add(actorName, unitlTurn);
			}
		}

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if (actor.IsDead)
			{
				return;
			}
			List<GridCoordinate> list = ((actorAction is MoveAction moveAction) ? moveAction.Path.Path : new List<GridCoordinate> { coord });
			truncatedPathEntry(list, actor, actorAction);
			GridPath path = GridPath.Create(list);
			CalibratePath(path, base.manager.CombatModel);
			List<PitfallArea> list2 = base.manager.CombatModel.Models.OfType<PitfallArea>().ToList();
			List<PitfallArea> list3 = new List<PitfallArea>();
			foreach (PitfallArea item in list2)
			{
				if (actor.Faction != item.Faction && item.IsInArea(list.Last()))
				{
					list3.Add(item);
				}
			}
			PitfallArea inWorkingPitfallArea = actor.InWorkingPitfallArea;
			PitfallArea pitfallArea = actor.InWorkingPitfallArea;
			foreach (PitfallArea item2 in list3)
			{
				if (!actor.FallInCombatAreas.Contains(item2) && !actor.IsDisoriented)
				{
					actor.FallInCombatAreas.Add(item2);
				}
				if (item2.ExpiryTurn > (pitfallArea?.ExpiryTurn ?? 0))
				{
					pitfallArea = item2;
				}
			}
			if (pitfallArea != inWorkingPitfallArea)
			{
				actor.InWorkingPitfallArea = pitfallArea;
				if (actor.HasTrait("UnleashedActive") && actor.IsPitfalled)
				{
					actor.FinishTimedEffect(interrupted: true);
					int affectTurn = Math.Max(0, actor.InWorkingPitfallArea.ExpiryTurn - base.manager.CombatModel.TurnManager.TurnCount);
					ApplyEffect(actor.InWorkingPitfallArea, actor, affectTurn);
				}
				else
				{
					int num = -1;
					ApplyEffect(affectTurn: (base.manager.CombatModel.TurnManager.ActiveFaction != actor.InWorkingPitfallArea.Faction) ? Math.Max(0, actor.InWorkingPitfallArea.ExpiryTurn - base.manager.CombatModel.TurnManager.TurnCount - 1) : Math.Max(0, actor.InWorkingPitfallArea.ExpiryTurn - base.manager.CombatModel.TurnManager.TurnCount), pitfallArea: actor.InWorkingPitfallArea, actor: actor);
				}
			}
			else if (pitfallArea != null && pitfallArea.Faction != actor.Faction && actor.ExclusiveTimedEffect == null)
			{
				int affectTurn2 = Math.Max(0, actor.InWorkingPitfallArea.ExpiryTurn - base.manager.CombatModel.TurnManager.TurnCount - 1);
				ApplyEffect(actor.InWorkingPitfallArea, actor, affectTurn2);
			}
		}

		private void truncatedPathEntry(List<GridCoordinate> path, ActorModel actor, ModelAction currentAction = null)
		{
			if (!actor.IsDisoriented && !truncatedPathByNearPitfallGrid(actor, currentAction))
			{
				truncatedPathOnMovingIntoPitfall(path, actor, currentAction);
			}
		}

		private bool truncatedPathByNearPitfallGrid(ActorModel actor, ModelAction currentAction = null)
		{
			if (currentAction is MoveAction moveAction)
			{
				CombatModel combatModel = base.manager.CombatModel;
				List<TWDModelObject> models = combatModel.GetModels<PitfallArea>();
				if (models.Count == 0)
				{
					return false;
				}
				GridCoordinate gridCoordinate = GridCoordinate.Invalid;
				foreach (TWDModelObject item in models)
				{
					if (!(item is PitfallArea pitfallArea) || actor.Faction == pitfallArea.Faction)
					{
						continue;
					}
					if (pitfallArea.IsInArea(actor.GridCoordinate))
					{
						gridCoordinate = actor.GridCoordinate;
						break;
					}
					if (pitfallArea.IsNearAreaGrid(actor.GridCoordinate))
					{
						GridCoordinate minDistanceAreaGrid = pitfallArea.GetMinDistanceAreaGrid(actor.GridCoordinate);
						if (!(minDistanceAreaGrid == GridCoordinate.Invalid) && !CombatHelpers.IsOccupiedOrBlocked(combatModel, minDistanceAreaGrid, actor) && combatModel.CanTraverse(actor, actor.GridCoordinate, minDistanceAreaGrid))
						{
							gridCoordinate = minDistanceAreaGrid;
							break;
						}
					}
				}
				if (gridCoordinate != GridCoordinate.Invalid)
				{
					moveAction.Path.ClipTo(moveAction.Path.Start);
					GridPath path = GridPath.Create(new List<GridCoordinate> { gridCoordinate });
					moveAction.Path.Append(path);
					return true;
				}
			}
			return false;
		}

		private void truncatedPathOnMovingIntoPitfall(List<GridCoordinate> path, ActorModel actor, ModelAction currentAction = null)
		{
			Faction activeFaction = base.manager.CombatModel.TurnManager.ActiveFaction;
			foreach (TWDModelObject model in base.manager.CombatModel.Models)
			{
				if (model is PitfallArea pitfallArea && actor.Faction != pitfallArea.Faction && path.Any(pitfallArea.IsInArea) && actor.Faction == activeFaction)
				{
					TruncatedPath(pitfallArea, actor, currentAction);
				}
			}
		}

		private void TruncatedPath(PitfallArea pitfallArea, ActorModel actor, ModelAction currentAction = null)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (!(currentAction is MoveAction moveAction))
			{
				return;
			}
			for (int i = 1; i < moveAction.Path.Count - 1; i++)
			{
				GridCoordinate gridCoordinate = moveAction.Path[i];
				if (!pitfallArea.IsInArea(gridCoordinate))
				{
					continue;
				}
				bool flag = combatModel.IsBlocked(moveAction.Path[i]);
				ActorModel occupier = combatModel.GetOccupier(moveAction.Path[i]);
				if (!flag && occupier == null)
				{
					moveAction.Path.ClipTo(gridCoordinate);
					break;
				}
				if (flag || (occupier != null && occupier.Faction != actor.Faction))
				{
					int num = i;
					while (num >= 0 && CombatHelpers.IsOccupiedOrBlocked(combatModel, moveAction.Path[num], actor))
					{
						gridCoordinate = moveAction.Path[num];
						num--;
					}
					moveAction.Path.ClipTo(gridCoordinate);
					break;
				}
			}
		}

		private bool CalibratePath(GridPath path, CombatModel combatModel)
		{
			if (path.IsValid)
			{
				if (path.Count > 1 && combatModel.GetOccupier(path.End) != null)
				{
					path.RemoveLast();
					return CalibratePath(path, combatModel);
				}
				return true;
			}
			return false;
		}

		protected override bool ShouldAdd(CombatArea newArea)
		{
			int num = 0;
			foreach (TWDModelObject model in base.manager.CombatModel.Models)
			{
				if (model is PitfallArea pitfallArea && pitfallArea.Faction == newArea.Faction)
				{
					num++;
					if (pitfallArea.Coordinate == newArea.Coordinate && pitfallArea.Radius == newArea.Radius)
					{
						pitfallArea.ExpiryTurn = newArea.ExpiryTurn;
						return false;
					}
				}
			}
			return num < MaxAreaCount;
		}

		protected override void Tick(IEnumerable<ActorModel> actorModels)
		{
			if (actorModels == null)
			{
				return;
			}
			foreach (ActorModel item in new List<ActorModel>(actorModels))
			{
				if (actorModels.Contains(item))
				{
					RemoveActorPitfallArea(item);
					if (!item.IsPitfalled && item.HasTrait("UnleashedActive"))
					{
						item.RemoveTrait("UnleashedActive");
					}
					if (!item.IsPitfalled)
					{
						RefreshActorAreaStateInternal(item, item.GridCoordinate);
					}
				}
			}
		}

		public void RemoveActorPitfallArea(ActorModel actorModel)
		{
			List<PitfallArea> list = base.manager.CombatModel.Models.OfType<PitfallArea>().ToList();
			List<PitfallArea> list2 = new List<PitfallArea>();
			foreach (CombatArea fallInCombatArea in actorModel.FallInCombatAreas)
			{
				bool flag = true;
				if (!(fallInCombatArea is PitfallArea pitfallArea))
				{
					continue;
				}
				foreach (PitfallArea item in list)
				{
					if (pitfallArea == item && pitfallArea.IsInArea(actorModel.GridCoordinate))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list2.Add(pitfallArea);
				}
			}
			foreach (PitfallArea item2 in list2)
			{
				actorModel.FallInCombatAreas.Remove(item2);
				if (actorModel.InWorkingPitfallArea == item2)
				{
					actorModel.InWorkingPitfallArea = null;
				}
				if (actorModel.HasTrait("UnleashedActive"))
				{
					actorModel.RemoveTrait("UnleashedActive");
				}
				if (actorModel.IsPitfalled)
				{
					actorModel.FinishTimedEffect(interrupted: true);
				}
			}
		}

		private void ApplyEffect(PitfallArea pitfallArea, ActorModel actor, int affectTurn)
		{
			if (affectTurn > 0)
			{
				if (!actor.FallInCombatAreas.Contains(pitfallArea))
				{
					actor.FallInCombatAreas.Add(pitfallArea);
				}
				if (pitfallArea != actor.InWorkingPitfallArea)
				{
					actor.InWorkingPitfallArea = pitfallArea;
				}
				if (!actor.HasTrait("UnleashedActive"))
				{
					actor.AddTemporaryTrait("UnleashedActive", default(FixedPoint), null, 0L);
				}
				base.manager.ExecuteAction(new PitfallAction(pitfallArea.Owner, actor, affectTurn));
			}
		}
	}
}
