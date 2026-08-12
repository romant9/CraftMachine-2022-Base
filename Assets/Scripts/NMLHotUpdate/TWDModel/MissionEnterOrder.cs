using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class MissionEnterOrder
	{
		public int Id;

		public List<int> CouncilLevelLimit;

		public List<int> SortInt;

		public int minimal()
		{
			return CouncilLevelLimit[0];
		}

		public int max()
		{
			return CouncilLevelLimit[1];
		}
	}
}
