using System;

namespace TWDModel
{
	[Serializable]
	public class TeamTeamPreset : ITeamPresetData
	{
		public SurvivorModel[] Survivors { get; set; }

		public string[] Supports { get; set; }

		public TeamTeamPreset()
		{
			Survivors = new SurvivorModel[3];
			Supports = new string[3];
		}
	}
}
