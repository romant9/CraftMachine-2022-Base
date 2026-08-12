using System.Collections.Generic;

namespace BaseModel.ContentTypes
{
	public sealed class UnityAdsIds : ContentTypeBase
	{
		public string UnityAdsGameIdIOS { get; set; }

		public string UnityAdsGameIdAndroid { get; set; }

		public string UnityAdsGameIdIOSKorea { get; set; }

		public List<UnityAdsPlacementData> UnityAdsIOSPlacementData { get; set; }

		public List<UnityAdsPlacementData> UnityAdsIOSKoreaPlacementData { get; set; }

		public List<UnityAdsPlacementData> UnityAdsAndroidPlacementData { get; set; }
	}
}
