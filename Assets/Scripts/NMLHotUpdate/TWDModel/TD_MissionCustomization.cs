using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class TD_MissionCustomization
	{
		public string Missionid;

		public List<int> MeleeStartLocation;

		public List<int> MeleeEndLocation;

		public List<int> RangeStartLocation;

		public List<int> RangeEndLocation;
	}
}
