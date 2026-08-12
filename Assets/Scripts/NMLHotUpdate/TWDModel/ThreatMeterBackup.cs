using BaseModel;

namespace TWDModel
{
	public class ThreatMeterBackup : TWDModelObject
	{
		[IgnoreModelProperty]
		public ThreatMeterModel Model { get; set; }

		public int ThreatLevel { get; set; }

		public int TurnCounter { get; set; }

		public int SpawnLevelOffset { get; set; }

		public int InitialTurnCountToWave { get; set; }

		public int InitialThreatLevel { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void Start()
		{
			base.Start();
		}

		public void RecordStatus(ThreatMeterModel model)
		{
			Model = model;
			ThreatLevel = model.ThreatLevel;
			TurnCounter = model.TurnCounter;
			SpawnLevelOffset = model.SpawnLevelOffset;
			InitialTurnCountToWave = model.InitialTurnCountToWave;
			InitialThreatLevel = model.InitialThreatLevel;
		}

		public void BackUp()
		{
			Model.ThreatLevel = ThreatLevel;
			Model.TurnCounter = TurnCounter;
			Model.SpawnLevelOffset = SpawnLevelOffset;
			Model.InitialThreatLevel = InitialThreatLevel;
			Model.InitialTurnCountToWave = InitialTurnCountToWave;
		}
	}
}
