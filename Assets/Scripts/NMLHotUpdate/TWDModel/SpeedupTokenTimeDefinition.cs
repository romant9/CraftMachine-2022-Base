using System;

namespace TWDModel
{
	[Serializable]
	public class SpeedupTokenTimeDefinition
	{
		public string Currency;

		public long SpeedupTime;

		public SpeedupType SpeedupType;

		public string Title;

		public long GetSpeedupMSTime()
		{
			return SpeedupTime * 1000;
		}
	}
}
