using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ActorSpawnPointModelBackup : TWDModelObject
	{
		protected int getSpawnCoordinatesAmountLocal = -1;

		[IgnoreModelProperty]
		public ActorSpawnPointModel Model { get; set; }

		public ActivationType ActivationType { get; set; }

		public int SpawnCountPerAction { get; set; }

		public int ActivationCount { get; set; }

		public int TotalSpawnCount { get; set; }

		public SpawnPointState State { get; set; }

		public int CurrentActivationCount { get; set; }

		public int CurrentSpawnCount { get; set; }

		public AIAlertness Alertness { get; set; }

		public Faction Faction { get; set; }

		public TriggerState TriggerStateToReact { get; set; }

		public ThreatState ActivationThreatState { get; set; }

		public int TriggerTurnDelay { get; set; }

		public int ActivationTurn { get; set; }

		public int LevelOffset { get; set; }

		public int SpawnTag { get; set; }

		public List<string> ScriptedBehaviors { get; set; }

		public List<string> AdditionalTraits { get; set; }

		public int TriggeredActivationTurn { get; set; }

		public ActorGender Gender { get; set; }

		public MissionFailCondition MissionFailCondition { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public virtual void RecordStatus(ActorSpawnPointModel model)
		{
			Model = model;
			ActivationType = model.ActivationType;
			SpawnCountPerAction = model.SpawnCountPerAction;
			ActivationCount = model.ActivationCount;
			TotalSpawnCount = model.TotalSpawnCount;
			State = model.State;
			CurrentActivationCount = model.CurrentActivationCount;
			CurrentSpawnCount = model.CurrentSpawnCount;
			Alertness = model.Alertness;
			TriggeredActivationTurn = model.TriggeredActivationTurn;
			Faction = model.Faction;
			TriggerStateToReact = model.TriggerStateToReact;
			ActivationThreatState = model.ActivationThreatState;
			TriggerTurnDelay = model.TriggerTurnDelay;
			ActivationTurn = model.ActivationTurn;
			LevelOffset = model.LevelOffset;
			SpawnTag = model.SpawnTag;
			ScriptedBehaviors = ((model.ScriptedBehaviors == null) ? null : new List<string>(model.ScriptedBehaviors));
			AdditionalTraits = ((model.AdditionalTraits == null) ? null : new List<string>(model.AdditionalTraits));
			Gender = model.Gender;
			MissionFailCondition = model.MissionFailCondition;
			getSpawnCoordinatesAmountLocal = model.getSpawnCoordinatesAmountLocal;
		}

		public virtual void BackUp()
		{
			Model.ActivationType = ActivationType;
			Model.SpawnCountPerAction = SpawnCountPerAction;
			Model.ActivationCount = ActivationCount;
			Model.TotalSpawnCount = TotalSpawnCount;
			Model.State = State;
			Model.CurrentActivationCount = CurrentActivationCount;
			Model.CurrentSpawnCount = CurrentSpawnCount;
			Model.Alertness = Alertness;
			Model.TriggeredActivationTurn = TriggeredActivationTurn;
			Model.Faction = Faction;
			Model.TriggerStateToReact = TriggerStateToReact;
			Model.ActivationThreatState = ActivationThreatState;
			Model.TriggerTurnDelay = TriggerTurnDelay;
			Model.ActivationTurn = ActivationTurn;
			Model.LevelOffset = LevelOffset;
			Model.SpawnTag = SpawnTag;
			Model.ScriptedBehaviors = ((ScriptedBehaviors == null) ? null : new List<string>(ScriptedBehaviors));
			Model.AdditionalTraits = ((AdditionalTraits == null) ? null : new List<string>(AdditionalTraits));
			Model.Gender = Gender;
			Model.MissionFailCondition = MissionFailCondition;
			Model.getSpawnCoordinatesAmountLocal = getSpawnCoordinatesAmountLocal;
		}
	}
}
