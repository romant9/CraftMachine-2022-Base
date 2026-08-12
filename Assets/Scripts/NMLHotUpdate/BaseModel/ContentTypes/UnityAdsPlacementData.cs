using System;

namespace BaseModel.ContentTypes
{
	[Serializable]
	public class UnityAdsPlacementData
	{
		public AdUsage UsageType { get; set; }

		public string PlacementId { get; set; }
	}
}
