using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class MissionSpawnPoint
	{
		public string MapId;

		public bool IsDeadly;

		[NonSerialized]
		[JsonIgnore]
		public MissionSpawnPointGroup OwningGroup;

		public int MissionLevel;

		public string MissionId;

		public DropEventDefinition.DropEventTag LootTag;

		[NonSerialized]
		[JsonIgnore]
		public List<MissionSpawnPoint> SpawnPointsToUnlock;

		[JsonIgnore]
		public bool IsExplicit
		{
			get
			{
				if (MissionId != null)
				{
					return MissionId.Length > 0;
				}
				return false;
			}
		}

		[JsonIgnore]
		public int RequiredSurvivorLevel => Math.Max(1, MissionLevel / 3);
	}
}
