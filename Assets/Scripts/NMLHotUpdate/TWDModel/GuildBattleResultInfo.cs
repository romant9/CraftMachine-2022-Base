using System.Collections.Generic;

namespace TWDModel
{
	public class GuildBattleResultInfo
	{
		public int EndVictoryPoints;

		public int EndEnemyVictoryPoints;

		public int GuildTier;

		public int EnemyTier;

		public bool isFakeBattle;

		public string EnemyGroupId;

		public string EnemyGuildName;

		public Dictionary<string, KeyValuePair<string, PlayerEmblem>> EnemyLeaderboardInfo;

		public List<string> RegisteredPlayers;

		public List<string> EnemyRegisteredPlayers;

		public List<ScoreDataEntry> PlayerScores;

		public GuildBattleModel.GuildBattleResult BattleResult;

		public static Dictionary<string, KeyValuePair<string, PlayerEmblem>> SetCombatParticipants(Dictionary<string, GuildBattleParticipantInfo> playerInfoSnapshot, List<string> registeredPlayers)
		{
			Dictionary<string, KeyValuePair<string, PlayerEmblem>> dictionary = new Dictionary<string, KeyValuePair<string, PlayerEmblem>>();
			foreach (string registeredPlayer in registeredPlayers)
			{
				if (playerInfoSnapshot.TryGetValue(registeredPlayer, out var value))
				{
					dictionary.Add(registeredPlayer, new KeyValuePair<string, PlayerEmblem>(value.Name, value.PlayerEmblem));
				}
			}
			return dictionary;
		}
	}
}
