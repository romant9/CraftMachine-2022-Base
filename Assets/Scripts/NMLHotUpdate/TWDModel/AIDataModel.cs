using System.Collections.Generic;

namespace TWDModel
{
	public class AIDataModel : TWDModelObject
	{
		public static string CurrentTarget = "CurrentTarget";

		public static string BuddyAidTarget = "BuddyAidTarget";

		public static string MoveToCoordinate = "MoveToCoordinate";

		public static string WanderingMoveTarget = "WanderingMoveTarget";

		public static string LastEnemyLocation = "LastEnemyLocation";

		public static string FollowTarget = "FollowTarget";

		public static string LastNoiseCoordinate = "LastNoiseCoordinate";

		public static string DamageReceived = "DamageReceived";

		public static string EnemySeen = "EnemySeen";

		public static string HeardNoise = "HeardNoise";

		public static string WakeUp = "WakeUp";

		public static string InteractiveObjectTarget = "InteractiveObjectTarget";

		public static string ForceCivilianTargets = "ForceCivilianTargets";

		public static string IsStuck = "IsStuck";

		private AIAlertness alertnessState;

		private List<string> scriptedBehaviorClasses;

		public AIAlertness Alertness
		{
			get
			{
				return alertnessState;
			}
			set
			{
				if (value != alertnessState)
				{
					AIAlertness prevState = alertnessState;
					alertnessState = value;
					NotifyAlertnessStateChanged(prevState, alertnessState);
				}
			}
		}

		public AIMode Mode { get; set; }

		public Dictionary<string, TWDModelObject> ModelReferences { get; set; }

		public Dictionary<string, GridCoordinate> GridCoordinates { get; set; }

		public Dictionary<string, AIEvent> Events { get; set; }

		public List<string> ScriptedBehaviorClasses
		{
			get
			{
				return scriptedBehaviorClasses;
			}
			set
			{
				if (ScriptedBehaviorClasses != value)
				{
					scriptedBehaviorClasses = value;
					NotifyScriptedBehaviorsChanged();
				}
			}
		}

		public FixedPoint Initiative { get; set; }

		public event ScriptedBehaviorsChangedHandler ScriptedBehaviorsChanged;

		public event AIAlertnessStateChangedHandler AIAlertnessStateChanged;

		public void Clear()
		{
			ModelReferences.Clear();
			GridCoordinates.Clear();
			Events.Clear();
		}

		public void SetCurrentTarget(ActorModel target)
		{
			SetModelReference(CurrentTarget, target);
			if (target != null)
			{
				SetGridCoordinate(LastEnemyLocation, target.GridCoordinate);
			}
		}

		public ActorModel GetCurrentTarget()
		{
			return GetModelReference<ActorModel>(CurrentTarget);
		}

		public void SetBuddyAidTarget(ActorModel target)
		{
			SetModelReference(BuddyAidTarget, target);
		}

		public void RemoveReferences(TWDModelObject modelReference)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, TWDModelObject> modelReference2 in ModelReferences)
			{
				if (modelReference2.Value == modelReference)
				{
					list.Add(modelReference2.Key);
				}
			}
			foreach (string item in list)
			{
				ModelReferences.Remove(item);
			}
		}

		public void SetModelReference(string key, TWDModelObject model)
		{
			if (ModelReferences.ContainsKey(key))
			{
				if (model != null)
				{
					ModelReferences[key] = model;
				}
				else
				{
					ModelReferences.Remove(key);
				}
			}
			else
			{
				ModelReferences.Add(key, model);
			}
		}

		public T GetModelReference<T>(string key) where T : TWDModelObject
		{
			if (ModelReferences.ContainsKey(key))
			{
				return ModelReferences[key] as T;
			}
			return null;
		}

		public void SetGridCoordinate(string key, GridCoordinate coordinate)
		{
			if (GridCoordinates.ContainsKey(key))
			{
				if (coordinate.IsValid)
				{
					GridCoordinates[key] = coordinate;
				}
				else
				{
					GridCoordinates.Remove(key);
				}
			}
			else
			{
				GridCoordinates.Add(key, coordinate);
			}
		}

		public GridCoordinate GetGridCoordinate(string key)
		{
			if (GridCoordinates.ContainsKey(key))
			{
				return GridCoordinates[key];
			}
			return GridCoordinate.Invalid;
		}

		public void SetEvent(string key, int durationInTurns = -1)
		{
			AIEvent value = new AIEvent(base.manager.CombatModel.TurnManager.TurnCount, durationInTurns);
			if (Events.ContainsKey(key))
			{
				Events[key] = value;
			}
			else
			{
				Events.Add(key, value);
			}
		}

		public void ClearEvent(string key)
		{
			if (Events.ContainsKey(key))
			{
				Events.Remove(key);
			}
		}

		public bool HasEvent(string key)
		{
			if (Events.ContainsKey(key))
			{
				return Events[key].IsValid(base.manager.CombatModel.TurnManager.TurnCount);
			}
			return false;
		}

		public override void Initialize()
		{
			base.Initialize();
			Alertness = AIAlertness.Idle;
			ModelReferences = new Dictionary<string, TWDModelObject>();
			GridCoordinates = new Dictionary<string, GridCoordinate>();
			Events = new Dictionary<string, AIEvent>();
			ScriptedBehaviorClasses = new List<string>();
		}

		public void OnCombatStart()
		{
			Reset();
		}

		public void OnCombatEnd()
		{
			Reset();
		}

		public void Reset()
		{
			ModelReferences.Clear();
			GridCoordinates.Clear();
			Events.Clear();
			ScriptedBehaviorClasses.Clear();
		}

		private void NotifyScriptedBehaviorsChanged()
		{
			this.ScriptedBehaviorsChanged?.Invoke();
		}

		private void NotifyAlertnessStateChanged(AIAlertness prevState, AIAlertness newState)
		{
			this.AIAlertnessStateChanged?.Invoke(prevState, newState);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
