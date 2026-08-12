using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public abstract class ActorToActorRelationsManager : TWDModelObject, IDestructibleCombatModel
	{
		public delegate void AreaActorStateRefreshEvent(ActorModel actor = null);

		protected abstract RelationType RelationType { get; }

		protected abstract Faction ExpirationCheckFactionTurn { get; }

		public event AreaActorStateRefreshEvent ActorAreaRefreshed;

		public ActorToActorRelationsManager()
		{
		}

		public override void SetManager(ModelManager mgr)
		{
			base.SetManager(mgr);
			base.manager.CombatModel.TurnManager.FactionChanged -= TurnManagerOnFactionChanged;
			base.manager.CombatModel.TurnManager.FactionChanged += TurnManagerOnFactionChanged;
			base.manager.CombatModel.Changed -= CombatModelOnChanged;
			base.manager.CombatModel.Changed += CombatModelOnChanged;
			base.manager.PreActionExecution -= ManagerOnActionExecuted;
			base.manager.PreActionExecution += ManagerOnActionExecuted;
		}

		protected void TurnManagerOnFactionChanged(Faction currentFaction, Faction newFaction)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (ExpirationCheckFactionTurn == newFaction)
			{
				TurnManager turnManager = combatModel.TurnManager;
				for (int num = combatModel.Models.Count - 1; num >= 0; num--)
				{
					if (combatModel.Models[num] is ActorToActorRelation actorToActorRelation && !IgnoreExpireTurnRemove(actorToActorRelation) && turnManager.TurnCount >= actorToActorRelation.ExpiryTurn && actorToActorRelation.Type == RelationType)
					{
						OnRemoveRelation(actorToActorRelation);
						combatModel.RemoveModel(actorToActorRelation);
						RefreshActorAreaStates();
					}
				}
				for (int num2 = combatModel.Models.Count - 1; num2 >= 0; num2--)
				{
					if (combatModel.Models[num2] is ActorToActorRelation relation)
					{
						OnExpirationFactionTurn(relation);
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

		public void RemoveRelationForExternal(ActorToActorRelation relation)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel != null)
			{
				combatModel.RemoveModel(relation);
				OnRemoveRelation(relation);
				RefreshActorAreaStates();
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

		public void RefreshActorAreaState(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			RefreshActorAreaStateInternal(actor, coord, actorAction);
			this.ActorAreaRefreshed?.Invoke(actor);
		}

		protected void RefreshActorAreaStates()
		{
			CombatModel combatModel = base.manager.CombatModel;
			RefreshActorAreaStates(combatModel.Survivors);
			RefreshActorAreaStates(combatModel.Raiders);
			RefreshActorAreaStates(combatModel.Walkers);
			this.ActorAreaRefreshed?.Invoke();
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

		public void AddRelation(ActorToActorRelation newRelation)
		{
			CombatModel combatModel = base.manager.CombatModel;
			ShouldAddRelationStatus shouldAddRelationStatus = CheckShouldAdd(newRelation);
			switch (shouldAddRelationStatus)
			{
			case ShouldAddRelationStatus.CanNotAdd:
				PostCheckShouldAdd(shouldAddRelationStatus, newRelation);
				break;
			case ShouldAddRelationStatus.CanAdd:
				combatModel.AddModel(newRelation);
				break;
			case ShouldAddRelationStatus.AlreadyHave:
				PostCheckShouldAdd(shouldAddRelationStatus, newRelation);
				break;
			}
			RefreshActorAreaStates();
			NotifyRelationChanged(shouldAddRelationStatus, newRelation);
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

		private bool IgnoreExpireTurnRemove(ActorToActorRelation relation)
		{
			if (relation is GrenadeFragmentDamageRelation)
			{
				return true;
			}
			return false;
		}

		protected virtual void Tick(IEnumerable<ActorModel> actorModels)
		{
		}

		protected abstract void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null);

		protected abstract ShouldAddRelationStatus CheckShouldAdd(ActorToActorRelation newRelation);

		protected abstract void PostCheckShouldAdd(ShouldAddRelationStatus shouldAddRelationStatus, ActorToActorRelation newRelation);

		protected abstract void OnRemoveRelation(ActorToActorRelation relation);

		protected abstract void OnExpirationFactionTurn(ActorToActorRelation relation);

		protected abstract void NotifyRelationChanged(ShouldAddRelationStatus shouldAddRelationStatus, ActorToActorRelation relation);
	}
}
