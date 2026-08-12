using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleLogPlayerEntry
	{
		[Flags]
		public enum Status
		{
			Registered = 1,
			ValidatedSuccess = 2
		}

		public long RegisteredBattleTimeSlot;

		public long BattleTimeSlot;

		public Status BattleStatus;

		public string GuildId;

		public int VP;

		public int RP;

		[JsonIgnore]
		public bool IsValidated => BattleStatus == Status.ValidatedSuccess;
	}
}
