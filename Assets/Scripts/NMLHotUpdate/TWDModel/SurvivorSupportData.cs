using System;

namespace TWDModel
{
	[Serializable]
	public class SurvivorSupportData
	{
		public string SupportId { get; set; }

		public int SupportIndex { get; set; }

		public int SupportLevel { get; set; }

		public SurvivorSupportData(string supportId, int supportIndex, int supportLevel)
		{
			SupportId = supportId;
			SupportIndex = supportIndex;
			SupportLevel = supportLevel;
		}
	}
}
