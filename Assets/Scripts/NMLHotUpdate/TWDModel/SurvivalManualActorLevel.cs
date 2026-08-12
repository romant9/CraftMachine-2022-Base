using System;

namespace TWDModel
{
	[Serializable]
	public class SurvivalManualActorLevel
	{
		public int ID;

		public int Type;

		public int Level;

		public int Attribute_hp_add;

		public int Attribute_attack_add;

		public int CostToken;

		public int UnlockActorStarLevel;
	}
}
