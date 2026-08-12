using System;

namespace TWDModel
{
	[Serializable]
	public class BattlePassNotificationDefinition
	{
		public const string NormalNotification = "Normal";

		public const string BeginnerNotification = "Beginner";

		public string LocalisationKey;

		public string BattlePassType;

		public FixedPoint IntervalFromSeasonStart;
	}
}
