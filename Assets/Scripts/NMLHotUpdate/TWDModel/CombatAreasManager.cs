using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public abstract class CombatAreasManager : TWDModelObject, IDestructibleCombatModel
	{
		public delegate void AreaActorStateRefreshEvent(ActorModel actor = null);

		protected abstract CombatAreaType CombatAreaType { get; }

		protected abstract Faction ExpirationCheckFactionTurn { get; }

		public event AreaActorStateRefreshEvent ActorAreaRefreshed;

		public override void SetManager(ModelManager mgr)
		{
			base.SetManager(mgr);
			base.manager.CombatModel.TurnManager.FactionChanged += TurnManagerOnFactionChanged;
			base.manager.CombatModel.Changed += CombatModelOnChanged;
			base.manager.PreActionExecution += ManagerOnActionExecuted;
		}

		public void SetManagerForTrapFlameManager(ModelManager mgr)
		{
			base.SetManager(mgr);
		}

		public override bool IsValid()
		{
			return true;
		}

		public virtual void Destroy()
		{
			if (base.manager == null)
			{
				return;
			}
			base.manager.PreActionExecution -= ManagerOnActionExecuted;
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel != null)
			{
				combatModel.Changed -= CombatModelOnChanged;
				TurnManager turnManager = combatModel.TurnManager;
				if (turnManager != null)
				{
					turnManager.FactionChanged -= TurnManagerOnFactionChanged;
				}
			}
		}

		protected void TurnManagerOnFactionChanged(Faction currentFaction, Faction newFaction)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (ExpirationCheckFactionTurn == newFaction)
			{
				TurnManager turnManager = combatModel.TurnManager;
				for (int num = combatModel.Models.Count - 1; num >= 0; num--)
				{
					if (combatModel.Models[num] is CombatArea combatArea && turnManager.TurnCount >= combatArea.ExpiryTurn && combatArea.Type == CombatAreaType)
					{
						combatModel.RemoveModel(combatArea);
						PostOnFactionChangedRemoved(combatModel);
						RefreshActorAreaStates();
					}
				}
			}
			switch (newFaction)
			{
			case Faction.Survivor:
				Tick(combatModel.Survivors);
				break;
			case Faction.Raider:
				Tick(combatModel.Raiders);
				break;
			case Faction.Walker:
				Tick(combatModel.Walkers);
				break;
			}
		}

		protected void ManagerOnActionExecuted(ModelAction action)
		{
			if (action is MoveAction moveAction)
			{
				RefreshActorAreaState(moveAction.Actor, moveAction.Path.End, action);
			}
			else if (action is PushActorAction pushActorAction)
			{
				RefreshActorAreaState(pushActorAction.Actor, pushActorAction.Path.End, action);
			}
			else if (action is SpawnAction spawnAction)
			{
				RefreshActorAreaState(spawnAction.Actor, spawnAction.SpawnLocation, action);
			}
		}

		protected void CombatModelOnChanged(ModelObject model, string changed, object args)
		{
			if (changed == "actorKilled")
			{
				ActorModel actorModel = (ActorModel)args;
				RefreshActorAreaState(actorModel, actorModel.GridCoordinate);
			}
		}

		protected void RefreshActorAreaStates()
		{
			CombatModel combatModel = base.manager.CombatModel;
			RefreshActorAreaStates(combatModel.Survivors);
			RefreshActorAreaStates(combatModel.Raiders);
			RefreshActorAreaStates(combatModel.Walkers);
			this.ActorAreaRefreshed?.Invoke();
		}

		public void RefreshActorAreaState(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			RefreshActorAreaStateInternal(actor, coord, actorAction);
			this.ActorAreaRefreshed?.Invoke(actor);
		}

		public void AddArea(CombatArea newArea)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (ShouldAdd(newArea))
			{
				combatModel.AddModel(newArea);
			}
			RefreshActorAreaStates();
		}

		public void RefreshActorAreaStates(IList<ActorModel> actorModels)
		{
			if (actorModels == null)
			{
				return;
			}
			foreach (ActorModel actorModel in actorModels)
			{
				RefreshActorAreaStateInternal(actorModel, actorModel.GridCoordinate);
			}
		}

		protected virtual void Tick(IEnumerable<ActorModel> actorModels)
		{
		}

		protected abstract void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null);

		protected abstract bool ShouldAdd(CombatArea area);

		protected virtual void PostOnFactionChangedRemoved(CombatModel combatModel)
		{
		}
	}
}
