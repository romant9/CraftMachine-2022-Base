namespace TWDModel
{
	public class RaiderSpawnPointModelBuckup : ActorSpawnPointModelBackup
	{
		public int ReplaceWithSurvivorPlayerIndex { get; set; }

		public SurvivorClass Class { get; set; }

		public string ActorClassID { get; set; }

		public string ActorID { get; set; }

		public AIMode AIMode { get; set; }

		public string WeaponOverrideId { get; set; }

		public string ArmorOverrideId { get; set; }

		public int EquipmentLevel { get; set; }

		public int EquipmentRarityLevel { get; set; }

		public bool SpawnCountInUse { get; set; }

		public RaiderVisualization RaiderVisualization { get; set; }

		public bool SpawnUsed { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void RecordStatus(ActorSpawnPointModel model)
		{
			base.RecordStatus(model);
			ActorClassID = (base.Model as RaiderSpawnPointModel).ActorClassID;
			ActorID = (base.Model as RaiderSpawnPointModel).ActorID;
			ReplaceWithSurvivorPlayerIndex = (base.Model as RaiderSpawnPointModel).ReplaceWithSurvivorPlayerIndex;
			Class = (base.Model as RaiderSpawnPointModel).Class;
			AIMode = (base.Model as RaiderSpawnPointModel).AIMode;
			WeaponOverrideId = (base.Model as RaiderSpawnPointModel).WeaponOverrideId;
			ArmorOverrideId = (base.Model as RaiderSpawnPointModel).ArmorOverrideId;
			EquipmentLevel = (base.Model as RaiderSpawnPointModel).EquipmentLevel;
			EquipmentRarityLevel = (base.Model as RaiderSpawnPointModel).EquipmentRarityLevel;
			SpawnCountInUse = (base.Model as RaiderSpawnPointModel).SpawnCountInUse;
			RaiderVisualization = (base.Model as RaiderSpawnPointModel).RaiderVisualization;
			SpawnUsed = (base.Model as RaiderSpawnPointModel).SpawnUsed;
		}

		public override void BackUp()
		{
			base.BackUp();
			(base.Model as RaiderSpawnPointModel).ActorClassID = ActorClassID;
			(base.Model as RaiderSpawnPointModel).ActorID = ActorID;
			(base.Model as RaiderSpawnPointModel).ReplaceWithSurvivorPlayerIndex = ReplaceWithSurvivorPlayerIndex;
			(base.Model as RaiderSpawnPointModel).Class = Class;
			(base.Model as RaiderSpawnPointModel).AIMode = AIMode;
			(base.Model as RaiderSpawnPointModel).WeaponOverrideId = WeaponOverrideId;
			(base.Model as RaiderSpawnPointModel).ArmorOverrideId = ArmorOverrideId;
			(base.Model as RaiderSpawnPointModel).EquipmentLevel = EquipmentLevel;
			(base.Model as RaiderSpawnPointModel).EquipmentRarityLevel = EquipmentRarityLevel;
			(base.Model as RaiderSpawnPointModel).SpawnCountInUse = SpawnCountInUse;
			(base.Model as RaiderSpawnPointModel).RaiderVisualization = RaiderVisualization;
			(base.Model as RaiderSpawnPointModel).SpawnUsed = SpawnUsed;
		}
	}
}
