using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class DifficultyIncrementalDebuff
	{
		public string Identifier;

		public ChallengeDebuffType DebuffType;

		public string Name;

		public string Image;

		public List<FixedPoint> ConstructionParameters;

		public string Description;

		public string LTTokenIcon;
	}
}
