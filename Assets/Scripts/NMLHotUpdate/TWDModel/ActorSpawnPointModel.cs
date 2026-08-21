using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ActorSpawnPointModel : TWDSpatialModelObject, InteractionReceiver, TriggerReceiver
	{
		public const string Spawned = "Spawned";

		public int getSpawnCoordinatesAmountLocal = -1;

		public Faction Faction { get; set; }

		public ActivationType ActivationType { get; set; }

		public TriggerState TriggerStateToReact { get; set; }

		public ThreatState ActivationThreatState { get; set; }

		public int TriggerTurnDelay { get; set; }

		public int ActivationTurn { get; set; }

		public int SpawnCountPerAction { get; set; }

		public int ActivationCount { get; set; }

		public int TotalSpawnCount { get; set; }

		public int LevelOffset { get; set; }

		public int SpawnTag { get; set; }

		public SpawnPointState State { get; set; }

		public int CurrentActivationCount { get; set; }

		public int CurrentSpawnCount { get; set; }

		public AIAlertness Alertness { get; set; }

		public List<string> ScriptedBehaviors { get; set; }

		public List<string> AdditionalTraits { get; set; }

		public int TriggeredActivationTurn { get; set; }

		public ActorGender Gender { get; set; }

		public MissionFailCondition MissionFailCondition { get; set; }

		public bool UseSpawnRotationOverride { get; set; }

		public float SpawnRotationY { get; set; }

		[JsonIgnore]
		public bool IsThreatActivated => ActivationType == ActivationType.Threat;

		[JsonIgnore]
		public bool CanActivate => State == SpawnPointState.Deactive;

		public ActorSpawnPointModel()
		{
			Gender = ActorGender.NotSpecified;
		}

		public ActorSpawnPointModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public override void Initialize()
		{
			base.Initialize();
			State = SpawnPointState.Deactive;
			CurrentActivationCount = 0;
			CurrentSpawnCount = 0;
			TriggeredActivationTurn = -1;
		}

		public void Activate(bool instant = false, ActorModel instigator = null)
		{
			if (State == SpawnPointState.Deactive)
			{
				State = SpawnPointState.Active;
				if (instant)
				{
					Spawn(instigator);
				}
			}
		}

		public void Reset()
		{
			State = SpawnPointState.Deactive;
			CurrentActivationCount = 0;
			CurrentSpawnCount = 0;
			TriggeredActivationTurn = -1;
			getSpawnCoordinatesAmountLocal = -1;
		}

		private void Deactivate()
		{
			State = SpawnPointState.Deactive;
		}

		private void Finish()
		{
			State = SpawnPointState.Finished;
		}

		public void OnInteractionStep(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			if (interactiveObject.UsedTurns == 0)
			{
				if (TriggerStateToReact == TriggerState.Start)
				{
					Trigger(instigator);
				}
			}
			else if (TriggerStateToReact == TriggerState.Step)
			{
				Trigger(instigator);
			}
		}

		public void OnInteractionCanceled(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnAttacked(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			if (TriggerStateToReact == TriggerState.Attacked)
			{
				Trigger(instigator);
			}
		}

		public void OnDestroyed(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			if (TriggerStateToReact == TriggerState.Destroyed)
			{
				Trigger(instigator);
			}
		}

		public void OnInteractionCompleted(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			if (TriggerStateToReact == TriggerState.Completed)
			{
				Trigger(instigator);
			}
		}

		public void OnTriggered(ActorModel instigator)
		{
			Trigger(instigator);
		}

		public void EnableContinuousSpawning()
		{
			ActivationCount = -1;
			if (State == SpawnPointState.Finished)
			{
				State = SpawnPointState.Active;
			}
		}

		public void StopAndClose()
		{
			State = SpawnPointState.Closed;
		}

		public bool ActivateAtTurn(int turn)
		{
			if ((ActivationType != ActivationType.AtTurn || ActivationTurn != turn) && (ActivationType != ActivationType.Triggered || TriggeredActivationTurn != turn))
			{
				if (ActivationType == ActivationType.OutpostInitial)
				{
					return turn == 0;
				}
				return false;
			}
			return true;
		}

		private void Trigger(ActorModel instigator)
		{
			if (State == SpawnPointState.Finished || State == SpawnPointState.Closed)
			{
				return;
			}
			if (ActivationType == ActivationType.Triggered || ActivationType == ActivationType.TriggeredOOT)
			{
				if (ActivationType == ActivationType.Triggered && TriggerTurnDelay > 0)
				{
					TriggeredActivationTurn = base.manager.CombatModel.TurnManager.TurnCount + TriggerTurnDelay;
				}
				else
				{
					Activate(ActivationType == ActivationType.TriggeredOOT, instigator);
				}
			}
			else if (State == SpawnPointState.Active)
			{
				Deactivate();
			}
		}

		public void CheckSpawn()
		{
			if (State == SpawnPointState.Active)
			{
				Spawn(null);
			}
		}

		public virtual int GetAvailableSpawnCoordinatesAmount()
		{
			if (getSpawnCoordinatesAmountLocal == -1)
			{
				getSpawnCoordinatesAmountLocal = GetSpawnCoordinates().Count;
			}
			return getSpawnCoordinatesAmountLocal;
		}

		private void Spawn(ActorModel instigator)
		{
			int num = InternalSpawn(instigator);
			CurrentSpawnCount += num;
			CurrentActivationCount++;
			NotifyChange("Spawned", num);
			if (ActivationCount >= 0 && CurrentActivationCount >= ActivationCount)
			{
				State = SpawnPointState.Finished;
			}
		}

		protected List<GridCoordinate> SolveAdjacentSpawnCoordinates()
		{
			CombatModel combatModel = base.manager.CombatModel;
			GridModel grid = combatModel.Grid;
			List<GridCoordinate> list = new List<GridCoordinate>();
			if (base.Location.Coordinates.Count > 1)
			{
				for (int i = 0; i < base.Location.Coordinates.Count; i++)
				{
					if (!grid.IsCoordinateValid(base.Location.Coordinates[i]))
					{
						continue;
					}
					foreach (GridCoordinate item in grid.Neighbors(base.Location.Coordinates[i]))
					{
						if (!base.Location.Coordinates.Contains(item) && !list.Contains(item) && !combatModel.IsBlocked(item) && combatModel.GetOccupier(item) == null && combatModel.CanTraverse(null, base.Location.Coordinates[i], item))
						{
							list.Add(item);
						}
					}
				}
			}
			else
			{
				foreach (GridCoordinate item2 in grid.Neighbors(base.Location.Coordinate))
				{
					if (!combatModel.IsBlocked(item2) && combatModel.GetOccupier(item2) == null)
					{
						list.Add(item2);
					}
				}
			}
			UtilsArray.ShuffleList(list, base.manager.Player.PlayerRandom);
			return list;
		}

		protected List<GridCoordinate> GetSpawnCoordinates()
		{
			CombatModel combatModel = base.manager.CombatModel;
			GridModel grid = combatModel.Grid;
			List<GridCoordinate> list = new List<GridCoordinate>();
			if (base.Location.Coordinates.Count > 1)
			{
				for (int i = 0; i < base.Location.Coordinates.Count; i++)
				{
					if (grid.IsCoordinateValid(base.Location.Coordinates[i]) && !combatModel.IsBlocked(base.Location.Coordinates[i]) && combatModel.GetOccupier(base.Location.Coordinates[i]) == null)
					{
						list.Add(base.Location.Coordinates[i]);
					}
				}
			}
			else if (grid.IsCoordinateValid(base.Location.Coordinate) && !combatModel.IsBlocked(base.Location.Coordinate) && combatModel.GetOccupier(base.Location.Coordinate) == null)
			{
				list.Add(base.Location.Coordinate);
			}
			UtilsArray.ShuffleList(list, base.manager.Player.PlayerRandom);
			return list;
		}

		protected virtual int InternalSpawn(ActorModel instigator)
		{
			return 0;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
