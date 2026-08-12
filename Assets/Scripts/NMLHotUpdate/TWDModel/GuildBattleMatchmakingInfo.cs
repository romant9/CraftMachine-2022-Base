using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleMatchmakingInfo : IGuildBattleMatchmakingInfoBase
	{
		public string GroupId { get; set; }

		public string GuildName { get; set; }

		public int TotalVictoryPoints { get; private set; }

		public int Tier { get; set; }

		public List<string> RegisteredPlayersList { get; set; }

		public Dictionary<string, GuildBattleParticipantInfo> PlayerInfoSnapshot { get; private set; }

		public int GuildLevel { get; private set; }

		public int GuildAdjustedLevel { get; private set; }

		public GuildBattleMatchmakingInfo()
		{
			PlayerInfoSnapshot = new Dictionary<string, GuildBattleParticipantInfo>();
			RegisteredPlayersList = new List<string>();
		}

		public void Start()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, GuildBattleParticipantInfo> item in PlayerInfoSnapshot)
			{
				if (item.Value.SelectedSurvivors == null || item.Value.SelectedSurvivors.Count == 0)
				{
					list.Add(item.Key);
				}
			}
			foreach (string item2 in list)
			{
				PlayerInfoSnapshot.Remove(item2);
			}
			foreach (GuildBattleParticipantInfo value in PlayerInfoSnapshot.Values)
			{
				value.Start();
			}
		}

		public void UpdateInfoOnEndBattle(int newTier, int newTotalVictoryPoints)
		{
			if (Tier != newTier)
			{
				Tier = newTier;
			}
			if (TotalVictoryPoints != newTotalVictoryPoints)
			{
				TotalVictoryPoints = newTotalVictoryPoints;
			}
		}

		public bool UpdateInfoOnPlayerChanged(GuildWarModel warModel, GuildBattleParticipantInfo playerInfo)
		{
			bool flag = false;
			if (playerInfo != null && ShouldUpdateGuildBattlePlayerSnapshot(playerInfo))
			{
				flag = true;
				PlayerInfoSnapshot[playerInfo.HashedPlayerId] = playerInfo;
				if (warModel.CurrentBattle.EnemyPlayersInfoList != null)
				{
					GuildBattleParticipantInfo currentGuildBattlePlayerInfo = warModel.CurrentBattle.GetCurrentGuildBattlePlayerInfo(playerInfo.HashedPlayerId);
					if (currentGuildBattlePlayerInfo != null)
					{
						if (playerInfo.Name != currentGuildBattlePlayerInfo.Name)
						{
							currentGuildBattlePlayerInfo.Name = playerInfo.Name;
						}
						if (playerInfo.PlayerEmblem != currentGuildBattlePlayerInfo.PlayerEmblem)
						{
							currentGuildBattlePlayerInfo.PlayerEmblem = playerInfo.PlayerEmblem;
						}
					}
				}
			}
			if (GuildAdjustedLevel == 0 || GuildLevel == 0 || flag)
			{
				UpdateGuildLevels();
			}
			return flag;
		}

		private void UpdateGuildLevels()
		{
			Tuple<FixedPoint, FixedPoint> tuple = GvGModelHelper.CalculateGuildLevel(PlayerInfoSnapshot);
			GuildAdjustedLevel = (int)tuple.First;
			GuildLevel = (int)tuple.Second;
		}

		public void UpdatePlayerInfo(GuildBattleParticipantInfo playerInfo)
		{
			if (playerInfo.HasValidDefense())
			{
				PlayerInfoSnapshot[playerInfo.HashedPlayerId] = playerInfo;
				UpdateGuildLevels();
			}
		}

		public bool ShouldUpdateGuildBattlePlayerSnapshot(GuildBattleParticipantInfo playerInfo)
		{
			if (PlayerInfoSnapshot == null)
			{
				return true;
			}
			if (playerInfo == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(playerInfo.HashedPlayerId))
			{
				return false;
			}
			if (!PlayerInfoSnapshot.TryGetValue(playerInfo.HashedPlayerId, out var value))
			{
				return true;
			}
			if (playerInfo.PlayerAdjustedLevel != value.PlayerAdjustedLevel)
			{
				return true;
			}
			if (playerInfo.Name != value.Name)
			{
				return true;
			}
			if (!playerInfo.PlayerEmblem.Equals(value.PlayerEmblem))
			{
				return true;
			}
			return false;
		}

		public bool DeleteGuildBattlePlayerSnapshot(string playerId)
		{
			bool flag = false;
			if (PlayerInfoSnapshot != null)
			{
				flag = PlayerInfoSnapshot.Remove(playerId);
			}
			return flag | RegisteredPlayersList.Remove(playerId);
		}

		public List<GuildBattleParticipantInfo> GetGuildBattleParticipantList()
		{
			List<GuildBattleParticipantInfo> list = new List<GuildBattleParticipantInfo>();
			if (PlayerInfoSnapshot != null)
			{
				foreach (GuildBattleParticipantInfo value in PlayerInfoSnapshot.Values)
				{
					list.Add(value.DeepClone());
				}
			}
			return list;
		}

		public GuildBattleParticipantInfo GetParticipantInfo(string playerHashedId)
		{
			GuildBattleParticipantInfo value = null;
			PlayerInfoSnapshot.TryGetValue(playerHashedId, out value);
			return value;
		}

		public void ResetParticipants()
		{
			PlayerInfoSnapshot.Clear();
			RegisteredPlayersList.Clear();
			GuildLevel = 0;
			GuildAdjustedLevel = 0;
		}


		#region mycode
		public void SetSnapshot(Dictionary<string, GuildBattleParticipantInfo> playerInfoSnapshot)
		{
			PlayerInfoSnapshot = playerInfoSnapshot;
		}
		#endregion
	}
}
