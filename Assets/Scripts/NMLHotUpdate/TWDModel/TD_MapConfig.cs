using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class TD_MapConfig
	{
		public string TDMapId;

		public int MissionLevel;

		public string Missionid;

		public int ActorAmount;

		public int EnemyAmount;

		public int InitialHealth;

		public string Time;

		[JsonIgnore]
		public long MapMaxTime => (long)TimeSpan.Parse(Time).TotalMilliseconds;
	}
}
