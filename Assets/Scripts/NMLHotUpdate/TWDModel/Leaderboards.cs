using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Leaderboards
	{
		public class OutpostLeaderboardDetails
		{
			public string Name { get; set; }

			public int Level { get; set; }

			public int OutpostLevel { get; set; }

			public string OutpostTierId { get; set; }
		}

		public class SurvivalManualLeaderboardDetails
		{
			public string Name;

			public string Country;

			public List<int> SurvivalManualIds;

			public int Level;

			public string GroupId;

			public string GroupName;

			public string PlayerEmblem;
		}

		public class ChallengeLeaderboardDetails
		{
			public string Name;

			public string Country;

			public int CurrentChallengeStars;

			public int Level;

			public string GroupId;

			public string GroupName;

			public string PlayerEmblem;
		}

		public class EndlessModeLeaderBoardDetails
		{
			public string Name;

			public int Level;

			public int ExpertModeEntryCount;

			public string PlayerEmblem;
		}

		public class EndlessModeLeaderSurvivorClassLeaderBoardDetails
		{
			public string Name;

			public int Level;

			public string PlayerEmblem;

			public SurvivorClass LeaderSurvivorClass;

			public string LeaderActorDefinitionId;

			public string LeaderCharacterPrefabName;
		}

		public class GuildBattleLiveScoreLeaderboardDetails
		{
			public string GroupId { get; private set; }

			public string GroupName { get; private set; }

			public GuildBattleLiveScoreLeaderboardDetails(string groupId, string groupName)
			{
				GroupId = groupId;
				GroupName = groupName;
			}
		}

		public class GuildBattlePlayersScoreLeaderboardDetails
		{
			public string PlayerHashedId { get; private set; }

			public string PlayerName { get; private set; }

			public string PlayerEmblem { get; private set; }

			public string GroupId { get; private set; }

			public GuildBattlePlayersScoreLeaderboardDetails(string playerName, string playerHashedId, string groupId, string playerEmblem)
			{
				PlayerName = playerName;
				PlayerHashedId = playerHashedId;
				GroupId = groupId;
				PlayerEmblem = playerEmblem;
			}
		}

		public static string ChallengeStarsGlobal = "ChallengeStarsGlobal";

		public static string ChallengeStarsCountryPrefix = "ChallengeStars_";

		private static string GuildChallengeStarsGlobal = "GuildChallengeStarsGlobal";

		private static string GuildChallengeStarsCountryPrefix = "GuildChallengeStars_";

		private static string ChallengeStarsWeeklyPrefix = "ChallengeStarsWeekly_";

		private static string ChallengeStarsWeeklyGlobalPrefix = "ChallengeStarsWeekly_global_";

		public static string EndlessModeCycle = "EndlessModeCycle_";

		private static string ApocalypseChallengeStarsWeeklyPrefix = " ApocalypseChallengeStarsWeekly_";

		private static string ApocalypseChallengeStarsWeeklyGlobalPrefix = " ApocalypseChallengeStarsWeekly_global_";

		public static string GvgGuildGlobalVpAllTimeTotal = "GvgGuildGlobalVpAllTimeTotal";

		public static string GvgGuildGlobalVpSeasonTotalPrefix = "GvgGuildGlobalVpSeasonTotal";

		public static string GvgGuildGlobalVpWarTotalPrefix = "GvgGuildGlobalVpWarTotal";

		public static string GvgGuildMembersVpAllTimeTotalPrefix = "GvgGuildMembersVpAllTimeTotal";

		public static string GvgGuildMembersVpSeasonTotalPrefix = "GvgGuildMembersVpSeasonTotal";

		public static string GvgGuildMembersVpWarTotalPrefix = "GvgGuildMembersVpWarTotal";

		private static string GuildBattleLiveScorePrefix = "GuildBattleLiveScore";

		private static string SurvivalManual = "SurvivalManual";

		public static string GvgEndBattleTag = "Ended";

		public static string GetGuildChallengeGlobalLeaderboardName(string challengeId)
		{
			return GuildChallengeStarsGlobal + challengeId;
		}

		public static string GetGuildChallengeCountryLeaderboardName(string country, string challengeId)
		{
			return GuildChallengeStarsCountryPrefix + country + "_" + challengeId;
		}

		public static string GetPlayerApocalypseChallengeWeeklyLeaderboardName(string challengeId)
		{
			return ApocalypseChallengeStarsWeeklyGlobalPrefix + challengeId;
		}

		public static string GetPlayerApocalypseChallengeWeeklyCountryLeaderboardName(string country, string challengeId)
		{
			return ApocalypseChallengeStarsWeeklyPrefix + country + "_" + challengeId;
		}

		public static string GetPlayerChallengeWeeklyLeaderboardName(string challengeId)
		{
			return ChallengeStarsWeeklyGlobalPrefix + challengeId;
		}

		public static string GetPlayerSurvivalManualLeaderboardName()
		{
			return SurvivalManual;
		}

		public static string GetPlayerChallengeWeeklyCountryLeaderboardName(string country, string challengeId)
		{
			return ChallengeStarsWeeklyPrefix + country + "_" + challengeId;
		}

		public static string GetPlayerChallengeWeeklyLeaderboardNameWithZoneId(string challengeId, int zoneId)
		{
			return $"{ChallengeStarsWeeklyGlobalPrefix}{challengeId}_{zoneId}";
		}

		public static string GetPlayerChallengeWeeklyCountryLeaderboardNameWithZoneId(string country, string challengeId, int zoneId)
		{
			return $"{ChallengeStarsWeeklyPrefix}{country}_{challengeId}_{zoneId}";
		}

		public static string GetEndlessModeLeaderboardName(int endlessModeIdentifier)
		{
			return EndlessModeCycle + endlessModeIdentifier;
		}

		public static string GetEndlessModeLeaderboardNameByClass(int endlessModeIdentifier, SurvivorClass survivorClass)
		{
			return $"{EndlessModeCycle}{endlessModeIdentifier}_{survivorClass}";
		}

		public static string GetEndlessModeLeaderboardNameByClassWithZoneId(int endlessModeIdentifier, int zoneId, SurvivorClass survivorClass)
		{
			return $"{EndlessModeCycle}{endlessModeIdentifier}_{zoneId}_{survivorClass}";
		}

		public static string GetEndlessModeLeaderboardNameWithZoneId(int endlessModeIdentifier, int zoneId)
		{
			return EndlessModeCycle + endlessModeIdentifier + "_" + zoneId;
		}

		public static LeaderboardEntry CreateChallengeLeaderboardEntry(PlayerModel player)
		{
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.Id = player.HashedId;
			leaderboardEntry.Tags = null;
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			leaderboardEntry.ScoreAt = (long)(DateTime.UtcNow - dateTime).TotalSeconds;
			ChallengeLeaderboardDetails challengeLeaderboardDetails = new ChallengeLeaderboardDetails();
			challengeLeaderboardDetails.Name = player.Name;
			challengeLeaderboardDetails.GroupId = (player.IsGuildMember ? player.GuildId : null);
			challengeLeaderboardDetails.GroupName = (player.IsGuildMember ? player.GuildModel.Name : null);
			challengeLeaderboardDetails.Level = player.Level;
			challengeLeaderboardDetails.PlayerEmblem = player.manager.GetMessageSerializer().Serialize(player.PlayerEmblem);
			if (player.WeeklyChallenge != null)
			{
				leaderboardEntry.Score = player.WeeklyChallenge.AllTimeNumberStars;
				challengeLeaderboardDetails.CurrentChallengeStars = player.WeeklyChallenge.NumberStars;
				if (player.Country != null)
				{
					challengeLeaderboardDetails.Country = player.Country;
				}
			}
			leaderboardEntry.Details = player.manager.GetMessageSerializer().SerializeObject(challengeLeaderboardDetails);
			return leaderboardEntry;
		}

		public static LeaderboardEntry CreateSurvivalManualLeaderboardEntry(PlayerModel player)
		{
			if (player == null)
			{
				return null;
			}
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry
			{
				Id = player.HashedId,
				Tags = null,
				ScoreAt = GetUnixTimeSecondsUtc()
			};
			SurvivalManualLeaderboardDetails survivalManualLeaderboardDetails = new SurvivalManualLeaderboardDetails
			{
				Name = player.Name,
				Level = player.Level,
				PlayerEmblem = ((player.manager != null && player.manager.GetMessageSerializer() != null) ? player.manager.GetMessageSerializer().Serialize(player.PlayerEmblem) : null),
				GroupId = ((player.IsGuildMember && player.GuildModel != null) ? player.GuildId : null),
				GroupName = ((player.IsGuildMember && player.GuildModel != null) ? player.GuildModel.Name : null),
				SurvivalManualIds = new List<int>()
			};
			if (player.SurvivalManualManager != null)
			{
				leaderboardEntry.Score = player.SurvivalManualManager.GetSystemLV();
				ModelList<SurvivalManualModel> survivalManualModels = player.SurvivalManualManager.SurvivalManualModels;
				if (survivalManualModels != null)
				{
					foreach (SurvivalManualModel item in survivalManualModels)
					{
						if (item != null && item.SurvivalManualEmblesState)
						{
							survivalManualLeaderboardDetails.SurvivalManualIds.Add(item.ID);
						}
					}
				}
				if (!string.IsNullOrEmpty(player.Country))
				{
					survivalManualLeaderboardDetails.Country = player.Country;
				}
			}
			else
			{
				leaderboardEntry.Score = 0L;
			}
			leaderboardEntry.Details = ((player.manager != null && player.manager.GetMessageSerializer() != null) ? player.manager.GetMessageSerializer().SerializeObject(survivalManualLeaderboardDetails) : null);
			return leaderboardEntry;
		}

		private static long GetUnixTimeSecondsUtc()
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)(DateTime.UtcNow - dateTime).TotalSeconds;
		}

		public static LeaderboardEntry CreateCurrentChallengeLeaderboardEntry(PlayerModel player)
		{
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.Id = player.HashedId;
			leaderboardEntry.Tags = null;
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			leaderboardEntry.ScoreAt = (long)(DateTime.UtcNow - dateTime).TotalSeconds;
			ChallengeLeaderboardDetails challengeLeaderboardDetails = new ChallengeLeaderboardDetails();
			challengeLeaderboardDetails.Name = player.Name;
			challengeLeaderboardDetails.GroupId = (player.IsGuildMember ? player.GuildId : null);
			challengeLeaderboardDetails.GroupName = (player.IsGuildMember ? player.GuildModel.Name : null);
			challengeLeaderboardDetails.Level = player.Level;
			challengeLeaderboardDetails.PlayerEmblem = player.manager.GetMessageSerializer().Serialize(player.PlayerEmblem);
			if (player.WeeklyChallenge != null)
			{
				leaderboardEntry.Score = player.WeeklyChallenge.NumberStars;
				challengeLeaderboardDetails.CurrentChallengeStars = player.WeeklyChallenge.NumberStars;
				if (player.Country != null)
				{
					challengeLeaderboardDetails.Country = player.Country;
				}
			}
			leaderboardEntry.Details = player.manager.GetMessageSerializer().SerializeObject(challengeLeaderboardDetails);
			return leaderboardEntry;
		}

		public static LeaderboardEntry CreateCurrentApocalypseChallengeLeaderboardEntry(PlayerModel player)
		{
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.Id = player.HashedId;
			leaderboardEntry.Tags = null;
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			leaderboardEntry.ScoreAt = (long)(DateTime.UtcNow - dateTime).TotalSeconds;
			ChallengeLeaderboardDetails challengeLeaderboardDetails = new ChallengeLeaderboardDetails();
			challengeLeaderboardDetails.Name = player.Name;
			challengeLeaderboardDetails.GroupId = (player.IsGuildMember ? player.GuildId : null);
			challengeLeaderboardDetails.GroupName = (player.IsGuildMember ? player.GuildModel.Name : null);
			challengeLeaderboardDetails.Level = player.Level;
			challengeLeaderboardDetails.PlayerEmblem = player.manager.GetMessageSerializer().Serialize(player.PlayerEmblem);
			if (player.WeeklyChallenge != null)
			{
				leaderboardEntry.Score = player.ApocalypseWeeklyChallenge.NumberStars;
				challengeLeaderboardDetails.CurrentChallengeStars = player.ApocalypseWeeklyChallenge.NumberStars;
				if (player.Country != null)
				{
					challengeLeaderboardDetails.Country = player.Country;
				}
			}
			leaderboardEntry.Details = player.manager.GetMessageSerializer().SerializeObject(challengeLeaderboardDetails);
			return leaderboardEntry;
		}

		public static LeaderboardEntry CreateChallengeLeaderboardEntry(GuildModel guild, ModelManager manager)
		{
			LeaderboardEntry obj = new LeaderboardEntry
			{
				Id = guild.Id,
				Tags = null
			};
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
			obj.ScoreAt = (long)(DateTime.UtcNow - dateTime).TotalSeconds;
			obj.Score = guild.CurrentChallengeStars;
			ChallengeLeaderboardDetails value = new ChallengeLeaderboardDetails
			{
				Name = guild.Name,
				CurrentChallengeStars = guild.CurrentChallengeStars,
				Country = guild.CountryCode
			};
			obj.Details = manager.GetMessageSerializer().SerializeObject(value);
			return obj;
		}

		public static LeaderboardEntry CreateEndlessModeLeaderBoardEntry(PlayerModel player)
		{
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.Id = player.HashedId;
			leaderboardEntry.Tags = null;
			leaderboardEntry.ScoreAt = player.UtcTimeStamp;
			EndlessModeLeaderBoardDetails endlessModeLeaderBoardDetails = new EndlessModeLeaderBoardDetails();
			if (player.EndlessModeManager != null)
			{
				endlessModeLeaderBoardDetails.Name = player.Name;
				endlessModeLeaderBoardDetails.Level = player.Level;
				endlessModeLeaderBoardDetails.ExpertModeEntryCount = player.EndlessModeManager.GetExpertModeAttemptEntryCount();
				endlessModeLeaderBoardDetails.PlayerEmblem = player.manager.GetMessageSerializer().Serialize(player.PlayerEmblem);
				leaderboardEntry.Score = player.EndlessModeManager.OverAllScore;
			}
			leaderboardEntry.Details = player.manager.GetMessageSerializer().SerializeObject(endlessModeLeaderBoardDetails);
			return leaderboardEntry;
		}

		public static LeaderboardEntry CreateEndlessModeLeaderBoardEntryByLeaderSurvivorClass(PlayerModel player, SurvivorClass leaderSurvivorClass, EndlessModeAttemptData endlessModeExpertAttemptData, long currentMaxScore)
		{
			if (player == null)
			{
				return null;
			}
			if (endlessModeExpertAttemptData == null)
			{
				return null;
			}
			if (player.EndlessModeManager == null)
			{
				return null;
			}
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.Id = player.HashedId;
			leaderboardEntry.Tags = null;
			leaderboardEntry.ScoreAt = player.UtcTimeStamp;
			EndlessModeLeaderSurvivorClassLeaderBoardDetails endlessModeLeaderSurvivorClassLeaderBoardDetails = new EndlessModeLeaderSurvivorClassLeaderBoardDetails();
			if (endlessModeExpertAttemptData.SurvivorMockData == null || endlessModeExpertAttemptData.SurvivorMockData.Count == 0)
			{
				return null;
			}
			SurvivorMockData survivorMockData = endlessModeExpertAttemptData.SurvivorMockData[0];
			if (survivorMockData == null)
			{
				return null;
			}
			if (player.EndlessModeManager != null)
			{
				endlessModeLeaderSurvivorClassLeaderBoardDetails.Name = player.Name;
				endlessModeLeaderSurvivorClassLeaderBoardDetails.Level = player.Level;
				endlessModeLeaderSurvivorClassLeaderBoardDetails.PlayerEmblem = ((player.manager != null && player.manager.GetMessageSerializer() != null) ? player.manager.GetMessageSerializer().Serialize(player.PlayerEmblem) : null);
				endlessModeLeaderSurvivorClassLeaderBoardDetails.LeaderSurvivorClass = leaderSurvivorClass;
				endlessModeLeaderSurvivorClassLeaderBoardDetails.LeaderActorDefinitionId = survivorMockData.ActorDefinitionId;
				endlessModeLeaderSurvivorClassLeaderBoardDetails.LeaderCharacterPrefabName = survivorMockData.CharacterPrefabName;
				leaderboardEntry.Score = currentMaxScore;
			}
			leaderboardEntry.Details = ((player.manager != null && player.manager.GetMessageSerializer() != null) ? player.manager.GetMessageSerializer().SerializeObject(endlessModeLeaderSurvivorClassLeaderBoardDetails) : null);
			return leaderboardEntry;
		}

		public static string GetGuildBattleLiveScoreLeaderboardName(string battleId, long randomSeed)
		{
			return string.Format("{0}_{1}", GuildBattleLiveScorePrefix, battleId + "_" + randomSeed);
		}

		public static string GetLeaderboardNameGuildGlobalSeason(int seasonId)
		{
			return $"{GvgGuildGlobalVpSeasonTotalPrefix}_{seasonId.ToString()}";
		}

		public static string GetLeaderboardNameGuildGlobalWar(int warId)
		{
			return $"{GvgGuildGlobalVpWarTotalPrefix}_{warId.ToString()}";
		}

		public static string GetLeaderboardNameGuildMembersSeason(int seasonId, string guildId)
		{
			return $"{GvgGuildMembersVpSeasonTotalPrefix}_{seasonId.ToString()}_{guildId}";
		}

		public static string GetLeaderboardNameGuildMembersWar(int warId, string guildId)
		{
			return $"{GvgGuildMembersVpWarTotalPrefix}_{warId.ToString()}_{guildId}";
		}

		public static string GetLeaderboardNameGuildMembersAlltime(string guildId)
		{
			return $"{GvgGuildMembersVpAllTimeTotalPrefix}_{guildId}_";
		}

		public static LeaderboardEntry CreateGuildBattleLiveScoreLeaderboardEntry(ModelManager manager, string guildId, string guildName, bool battleEnded = false)
		{
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.Id = guildId;
			if (battleEnded)
			{
				leaderboardEntry.Tags = new string[1] { GvgEndBattleTag };
			}
			else
			{
				leaderboardEntry.Tags = null;
			}
			leaderboardEntry.ScoreAt = (long)(DateTime.UtcNow - TWDModelManager.Epoch).TotalSeconds;
			GuildBattleLiveScoreLeaderboardDetails value = new GuildBattleLiveScoreLeaderboardDetails(guildId, guildName);
			leaderboardEntry.Details = manager.GetMessageSerializer().SerializeObject(value);
			return leaderboardEntry;
		}

		public static LeaderboardEntry CreateGuildBattlePlayersScoreLeaderboardEntry(ModelManager manager, string playerName, string playerHashedId, string guildId, string playerEmblem, int score)
		{
			LeaderboardEntry obj = new LeaderboardEntry
			{
				Id = playerHashedId,
				Tags = null,
				ScoreAt = (long)(DateTime.UtcNow - TWDModelManager.Epoch).TotalSeconds,
				Score = score
			};
			GuildBattlePlayersScoreLeaderboardDetails value = new GuildBattlePlayersScoreLeaderboardDetails(playerName, playerHashedId, guildId, playerEmblem);
			obj.Details = manager.GetMessageSerializer().SerializeObject(value);
			return obj;
		}
	}
}
