using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ItemDefinition
	{
		public string ItemName;

		public string Type;

		public string ImageIcon;

		public string ImageIconBg;

		public string NameLocaliztion;

		public string DetailDescription;

		public List<int> Acquisition;

		public bool IsSubType;

		public string ImageIconOnCloud;

		private List<string> acquisitionLocalization;

		[JsonIgnore]
		public List<string> AcquisitionLocalization => acquisitionLocalization;

		public void SetAcquisitionLocalization(List<string> locations)
		{
			acquisitionLocalization = locations;
		}
	}
}
