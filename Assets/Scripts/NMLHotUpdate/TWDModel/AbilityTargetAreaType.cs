using System;

namespace TWDModel
{
	[Serializable]
	public enum AbilityTargetAreaType
	{
		Circle = 0,
		Cone = 1,
		Line = 2,
		LineMax = 3,
		Chained = 4,
		ConeLeft = 5,
		ConeRight = 6,
		LineSeparated = 7
	}
}
