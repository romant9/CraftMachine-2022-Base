using System;

namespace TWDModel
{
	[Serializable]
	public class DailyQuestRewardSetDefinition
	{
		public string Id;

		public int Chance;

		public string Q1;

		public string Q2;

		public string Q3;

		public string Q4;

		public string Q5;

		public int PointsFromFinishAll;
	}
}
