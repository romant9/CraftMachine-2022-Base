using System.Collections.Generic;

namespace TWDModel
{
	public class QuestVariables
	{
		public string Operation;

		public string MissionKind;

		public string AbilityType;

		public string AbilityId;

		public string TargetType;

		public List<string> SurvivorClass = new List<string>();

		public List<string> Hero = new List<string>();

		public int Count;

		public string GameMode;

		public string ShopType;

		public long CurrentTime;

		public int CouncilLevel;

		public int EquipmentCount;

		public string TargetSpecificType;

		public int ChallengeRoundsComplete;

		public void Clear()
		{
			Operation = null;
			MissionKind = null;
			AbilityId = null;
			AbilityType = null;
			TargetType = null;
			SurvivorClass.Clear();
			Hero.Clear();
			Count = 0;
			GameMode = null;
			ShopType = null;
			CurrentTime = 0L;
			CouncilLevel = 0;
			EquipmentCount = 0;
			TargetSpecificType = null;
			ChallengeRoundsComplete = 0;
		}
	}
}
