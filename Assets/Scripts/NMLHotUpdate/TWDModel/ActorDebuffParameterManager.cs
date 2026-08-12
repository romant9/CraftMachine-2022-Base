using System.Collections.Generic;

namespace TWDModel
{
	public class ActorDebuffParameterManager : TWDModelObject, IDestructibleCombatModel
	{
		public List<DebuffParameterBase> DebuffParameters { get; set; }

		public ActorDebuffParameterManager()
		{
		}

		public ActorDebuffParameterManager(ActorDebuffParameterManager actorDebuffParameterManager)
		{
		}

		public void RemoveAllAndAddNewParameterByParameterKeyAndSource(DebuffParameterBase newDebuffParameter)
		{
			RemoveParametersByParameterKeyAndSource(newDebuffParameter.ParameterKey, newDebuffParameter.Source);
			DebuffParameters.Add(newDebuffParameter);
		}

		public void RemoveAllAndAddNewParameterByParameterKey(DebuffParameterBase newDebuffParameter)
		{
			RemoveParametersByParameterKey(newDebuffParameter.ParameterKey);
			DebuffParameters.Add(newDebuffParameter);
		}

		public void RemoveParametersByParameterKey(string parameterKey)
		{
			DebuffParameters.RemoveAll((DebuffParameterBase x) => x.ParameterKey == parameterKey);
		}

		public void RemoveParametersByParameterKeyAndSource(string parameterKey, ActorModel source)
		{
			DebuffParameters.RemoveAll((DebuffParameterBase x) => x.ParameterKey == parameterKey && x.Source == source);
		}

		public bool TryGetParameterValueByParameterKey<T>(string parameterName, out T value)
		{
			value = default(T);
			if (DebuffParameters == null)
			{
				return false;
			}
			foreach (DebuffParameterBase debuffParameter in DebuffParameters)
			{
				if (debuffParameter.ParameterKey == parameterName && debuffParameter is DebuffParameterAbstract<T> { ParameterValue: not null } debuffParameterAbstract)
				{
					value = debuffParameterAbstract.ParameterValue;
					return true;
				}
			}
			return false;
		}

		public bool TryGetParameterValueByParameterKeyAndSource<T>(string parameterName, ActorModel source, out T value)
		{
			value = default(T);
			if (DebuffParameters == null)
			{
				return false;
			}
			foreach (DebuffParameterBase debuffParameter in DebuffParameters)
			{
				if (debuffParameter.ParameterKey == parameterName && debuffParameter.Source == source && debuffParameter is DebuffParameterAbstract<T> { ParameterValue: not null } debuffParameterAbstract)
				{
					value = debuffParameterAbstract.ParameterValue;
					return true;
				}
			}
			return false;
		}

		public void ClearDebuffParameters()
		{
			if (DebuffParameters != null)
			{
				DebuffParameters.Clear();
			}
		}

		public override void Start()
		{
			base.Start();
			if (DebuffParameters == null)
			{
				DebuffParameters = new List<DebuffParameterBase>();
			}
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= RemoveExpiryParameterOnFactionChanged;
				turnManager.FactionChanged += RemoveExpiryParameterOnFactionChanged;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			DebuffParameters = new List<DebuffParameterBase>();
		}

		public void SetupForCombat()
		{
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= RemoveExpiryParameterOnFactionChanged;
				turnManager.FactionChanged += RemoveExpiryParameterOnFactionChanged;
			}
		}

		public void RemoveExpiryParameterOnFactionChanged(Faction currentFaction, Faction newFaction)
		{
			CombatModel combatModel = base.manager.CombatModel;
			if (DebuffParameters == null || DebuffParameters.Count <= 0)
			{
				return;
			}
			TurnManager turnManager = combatModel.TurnManager;
			for (int num = DebuffParameters.Count - 1; num >= 0; num--)
			{
				if (DebuffParameters[num].ExpirationCheckFactionTurn == newFaction && !DebuffParameters[num].IgnoreExpiryTurn && turnManager.TurnCount >= DebuffParameters[num].ExpiryTurn)
				{
					DebuffParameters.RemoveAt(num);
				}
			}
		}

		public virtual void Destroy()
		{
			TurnManager turnManager = base.manager.CombatModel?.TurnManager;
			if (turnManager != null)
			{
				turnManager.FactionChanged -= RemoveExpiryParameterOnFactionChanged;
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
