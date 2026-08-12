using System;

namespace TWDModel
{
	[Serializable]
	public class EquipPrizeWheelWeight
	{
		public int Time;

		public int Weight;

		public static EquipPrizeWheelWeight Parse(string str)
		{
			return new EquipPrizeWheelWeight
			{
				Time = int.Parse(str.Split('_')[0]),
				Weight = int.Parse(str.Split('_')[1])
			};
		}
	}
}
