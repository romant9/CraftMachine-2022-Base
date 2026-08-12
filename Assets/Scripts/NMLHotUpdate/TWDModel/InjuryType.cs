using System;

namespace TWDModel
{
	[Serializable]
	public enum InjuryType
	{
		None = 0,
		Minor = 1,
		Major = 2,
		Critical = 3,
		OutOfAction = 4,
		Count = 5
	}
}
