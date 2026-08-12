using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleParticipantInfo : UtilsList.IDeepClonable<GuildBattleParticipantInfo>
	{
		public string HashedPlayerId { get; set; }

		public string Name { get; set; }

		public int PlayerAdjustedLevel { get; set; }

		public int PlayerActualLevel { get; set; }

		public List<SurvivorMockData> SelectedSurvivors { get; set; }

		public PlayerEmblem PlayerEmblem { get; set; }

		public GuildBattleParticipantInfo()
		{
		}

		private GuildBattleParticipantInfo(GuildBattleParticipantInfo otherPlayer)
		{
			HashedPlayerId = otherPlayer.HashedPlayerId;
			Name = otherPlayer.Name;
			PlayerAdjustedLevel = otherPlayer.PlayerAdjustedLevel;
			PlayerActualLevel = otherPlayer.PlayerActualLevel;
			PlayerEmblem = otherPlayer.PlayerEmblem;
			SelectedSurvivors = UtilsList.DeepCloneList(otherPlayer.SelectedSurvivors);
		}

		public GuildBattleParticipantInfo DeepClone()
		{
			GuildBattleParticipantInfo guildBattleParticipantInfo = new GuildBattleParticipantInfo(this);
			guildBattleParticipantInfo.Start();
			return guildBattleParticipantInfo;
		}

		public void Start()
		{
			for (int i = 0; i < SelectedSurvivors.Count; i++)
			{
				SurvivorMockData survivorMockData = SelectedSurvivors[i];
				survivorMockData.OwnerHashedPlayerId = HashedPlayerId;
				survivorMockData.Start();
			}
		}

		public bool HasValidDefense()
		{
			List<SurvivorMockData> selectedSurvivors = SelectedSurvivors;
			if (selectedSurvivors == null)
			{
				return false;
			}
			return selectedSurvivors.Count > 0;
		}


		#region myparams
		public int PlayerBaseIndex { get; set; }
		#endregion
	}
}
