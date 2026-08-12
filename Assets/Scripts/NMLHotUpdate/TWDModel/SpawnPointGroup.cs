using Newtonsoft.Json;

namespace TWDModel
{
	public class SpawnPointGroup
	{
		public string DisplayName;

		public int MinMissionLevel;

		public int MaxMissionLevel;

		public SpawnPointLocation PrimarySpawnPointLocation;

		public int Order;

		[JsonIgnore]
		public string UnlockKey
		{
			get
			{
				if (PrimarySpawnPointLocation == null)
				{
					return null;
				}
				return $"spawnpointgroup.{PrimarySpawnPointLocation.Id:X0}";
			}
		}
	}
}
