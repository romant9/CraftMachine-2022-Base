using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SPTraitsRemoldRandomPackage
	{
		public string ID;

		public string PackageTag;

		public List<string> TraitsRemold;

		public int PackageStar;

		[JsonIgnore]
		public Dictionary<string, int> TraitsRemoldInfos { get; set; }

		[JsonIgnore]
		public List<SPTraitsRemoldDefinitions> TraitsRemoldDefinitions { get; set; }

		public int GetTraitsRemoldWeight(string traitsId)
		{
			if (TraitsRemoldInfos.ContainsKey(traitsId))
			{
				return TraitsRemoldInfos[traitsId];
			}
			return 0;
		}
	}
}
