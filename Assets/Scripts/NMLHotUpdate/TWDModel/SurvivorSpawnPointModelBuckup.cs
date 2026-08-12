namespace TWDModel
{
	public class SurvivorSpawnPointModelBuckup : ActorSpawnPointModelBackup
	{
		public string ActorID { get; set; }

		public int MinLevelOffset { get; set; }

		public int MaxLevelOffset { get; set; }

		public int RarityLevel { get; set; }

		public SurvivorClass SurvivorClass { get; set; }

		public string WeaponOverrideId { get; set; }

		public string ArmorOverrideId { get; set; }

		public int EquipmentLevel { get; set; }

		public int EquipmentRarityLevel { get; set; }

		public int RosterIndex { get; set; }

		public int MovementOverride { get; set; }

		public bool IsNotGivenToPlayer { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public override void RecordStatus(ActorSpawnPointModel model)
		{
			base.RecordStatus(model);
			ActorID = (base.Model as SurvivorSpawnPointModel).ActorID;
			MinLevelOffset = (base.Model as SurvivorSpawnPointModel).MinLevelOffset;
			MaxLevelOffset = (base.Model as SurvivorSpawnPointModel).MaxLevelOffset;
			RarityLevel = (base.Model as SurvivorSpawnPointModel).RarityLevel;
			SurvivorClass = (base.Model as SurvivorSpawnPointModel).SurvivorClass;
			WeaponOverrideId = (base.Model as SurvivorSpawnPointModel).WeaponOverrideId;
			ArmorOverrideId = (base.Model as SurvivorSpawnPointModel).ArmorOverrideId;
			EquipmentLevel = (base.Model as SurvivorSpawnPointModel).EquipmentLevel;
			EquipmentRarityLevel = (base.Model as SurvivorSpawnPointModel).EquipmentRarityLevel;
			RosterIndex = (base.Model as SurvivorSpawnPointModel).RosterIndex;
			MovementOverride = (base.Model as SurvivorSpawnPointModel).MovementOverride;
			IsNotGivenToPlayer = (base.Model as SurvivorSpawnPointModel).IsNotGivenToPlayer;
		}

		public override void BackUp()
		{
			base.BackUp();
			(base.Model as SurvivorSpawnPointModel).ActorID = ActorID;
			(base.Model as SurvivorSpawnPointModel).MinLevelOffset = MinLevelOffset;
			(base.Model as SurvivorSpawnPointModel).MaxLevelOffset = MaxLevelOffset;
			(base.Model as SurvivorSpawnPointModel).RarityLevel = RarityLevel;
			(base.Model as SurvivorSpawnPointModel).SurvivorClass = SurvivorClass;
			(base.Model as SurvivorSpawnPointModel).WeaponOverrideId = WeaponOverrideId;
			(base.Model as SurvivorSpawnPointModel).ArmorOverrideId = ArmorOverrideId;
			(base.Model as SurvivorSpawnPointModel).EquipmentLevel = EquipmentLevel;
			(base.Model as SurvivorSpawnPointModel).EquipmentRarityLevel = EquipmentRarityLevel;
			(base.Model as SurvivorSpawnPointModel).RosterIndex = RosterIndex;
			(base.Model as SurvivorSpawnPointModel).MovementOverride = MovementOverride;
			(base.Model as SurvivorSpawnPointModel).IsNotGivenToPlayer = IsNotGivenToPlayer;
		}
	}
}
