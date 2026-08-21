namespace BaseModel
{
	public class WorldBossCellStatusResult
	{
		public bool Success { get; set; }

		public string ErrorMessage { get; set; }

		public int Status { get; set; }

		public string OccupyingGroupId { get; set; }

		public string OccupyingPlayerHashedId { get; set; }

		public string FightingGroupId { get; set; }

		public string FightingPlayerHashedId { get; set; }

		public int RemainingDurability { get; set; }

		public bool HasDefender { get; set; }

		public bool IsEmpty => Status == 0;

		public bool IsOccupied => Status == 2;

		public bool IsFighting => Status == 1;
	}
}
