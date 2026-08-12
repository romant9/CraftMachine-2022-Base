using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class DelayedActionGrenadeAreaManager : CombatAreasManager
	{
		protected override CombatAreaType CombatAreaType => CombatAreaType.DelayedActionGrenade;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		public override void SetManager(ModelManager mgr)
		{
			base.SetManager(mgr);
			base.manager.ActionExecuted -= OnActionExecutedPostLanding;
			base.manager.ActionExecuted += OnActionExecutedPostLanding;
		}

		public override void Destroy()
		{
			if (base.manager != null)
			{
				base.manager.ActionExecuted -= OnActionExecutedPostLanding;
			}
			base.Destroy();
		}

		protected override bool ShouldAdd(CombatArea newArea)
		{
			return true;
		}

		protected override void Tick(IEnumerable<ActorModel> actorModels)
		{
			List<DelayedActionGrenadeArea> list = base.manager.CombatModel.Models.OfType<DelayedActionGrenadeArea>().ToList();
			if (list.Count == 0)
			{
				return;
			}
			int turnCount = base.manager.CombatModel.TurnManager.TurnCount;
			foreach (DelayedActionGrenadeArea item in list)
			{
				if (turnCount >= item.DetonateTurn)
				{
					base.manager.ExecuteAction(new DetonateGrenadeAction(item));
				}
			}
		}

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if (actor == null || actor.IsDead || !(actorAction is MoveAction { Path: not null } moveAction) || moveAction.Path.Path == null)
			{
				return;
			}
			List<DelayedActionGrenadeArea> list = base.manager.CombatModel.Models.OfType<DelayedActionGrenadeArea>().ToList();
			if (list.Count == 0)
			{
				return;
			}
			List<GridCoordinate> path = moveAction.Path.Path;
			for (int i = 1; i < path.Count; i++)
			{
				GridCoordinate cell = path[i];
				if (list.Any((DelayedActionGrenadeArea b) => b.EffectiveAreaGridCoordinate == cell && b.Faction != actor.Faction))
				{
					moveAction.Path.ClipTo(cell);
					moveAction.Path.ClearTargetCoordinate();
					moveAction.CanBeInterruptedForPassByAttack = false;
					moveAction.CanBeInterruptedForPassByPull = false;
					break;
				}
			}
		}

		private void OnActionExecutedPostLanding(ModelAction action)
		{
			ActorModel actor = GetLandedActor(action);
			if (actor == null || actor.IsDead)
			{
				return;
			}
			GridCoordinate cell = actor.GridCoordinate;
			foreach (DelayedActionGrenadeArea item in (from b in base.manager.CombatModel.Models.OfType<DelayedActionGrenadeArea>()
				where b.EffectiveAreaGridCoordinate == cell && b.Faction != actor.Faction
				select b).ToList())
			{
				base.manager.ExecuteAction(new DetonateGrenadeAction(item));
			}
		}

		private static ActorModel GetLandedActor(ModelAction action)
		{
			if (!(action is MoveAction moveAction))
			{
				if (!(action is PushActorAction pushActorAction))
				{
					if (action is SpawnAction spawnAction)
					{
						return spawnAction.Actor;
					}
					return null;
				}
				return pushActorAction.Actor;
			}
			return moveAction.Actor;
		}
	}
}
