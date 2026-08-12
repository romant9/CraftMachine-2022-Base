using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public abstract class FactionToActorManager : TWDModelObject, IDestructibleCombatModel
	{
		public delegate void AreaActorStateRefreshEvent(ActorModel actor = null);

		protected virtual bool IgnoreExpireTurnRemove => false;

		protected abstract FactionToActorRelationType RelationType { get; }

		protected abstract Faction ExpirationCheckFactionTurn { get; }

		public event AreaActorStateRefreshEvent ActorAreaRefreshed;

		public FactionToActorManager()
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
					if (combatModel.Models[num] is FactionToActorRelation factionToActorRelation && !IgnoreExpireTurnRemove && turnManager.TurnCount >= factionToActorRelation.ExpiryTurn && factionToActorRelation.Type == RelationType)
					{
						OnRemoveRelation(factionToActorRelation);
						combatModel.RemoveModel(factionToActorRelation);
						RefreshActorAreaStates();
					}
				}
				for (int num2 = combatModel.Models.Count - 1; num2 >= 0; num2--)
				{
					if (combatModel.Models[num2] is FactionToActorRelation relation)
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

		public void RemoveRelationForExternal(FactionToActorRelation relation)
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

		public void AddRelation(FactionToActorRelation newRelation)
		{
			CombatModel combatModel = base.manager.CombatModel;
			FactionToActorRelationShouldAddRelationStatus factionToActorRelationShouldAddRelationStatus = CheckShouldAdd(newRelation);
			switch (factionToActorRelationShouldAddRelationStatus)
			{
			case FactionToActorRelationShouldAddRelationStatus.CanNotAdd:
				PostCheckShouldAdd(factionToActorRelationShouldAddRelationStatus, newRelation);
				break;
			case FactionToActorRelationShouldAddRelationStatus.CanAdd:
				combatModel.AddModel(newRelation);
				NotifyRelationAdded(newRelation);
				break;
			case FactionToActorRelationShouldAddRelationStatus.AlreadyHave:
				PostCheckShouldAdd(factionToActorRelationShouldAddRelationStatus, newRelation);
				break;
			}
			RefreshActorAreaStates();
			NotifyRelationChanged(factionToActorRelationShouldAddRelationStatus, newRelation);
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

		protected virtual void Tick(IEnumerable<ActorModel> actorModels)
		{
		}

		protected abstract void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null);

		protected abstract FactionToActorRelationShouldAddRelationStatus CheckShouldAdd(FactionToActorRelation newRelation);

		protected abstract void PostCheckShouldAdd(FactionToActorRelationShouldAddRelationStatus shouldAddRelationStatus, FactionToActorRelation newRelation);

		protected abstract void OnRemoveRelation(FactionToActorRelation relation);

		protected abstract void OnExpirationFactionTurn(FactionToActorRelation relation);

		protected abstract void NotifyRelationChanged(FactionToActorRelationShouldAddRelationStatus shouldAddRelationStatus, FactionToActorRelation relation);

		protected abstract void NotifyRelationAdded(FactionToActorRelation relation);

		public override bool IsValid()
		{
			return true;
		}
	}
}
