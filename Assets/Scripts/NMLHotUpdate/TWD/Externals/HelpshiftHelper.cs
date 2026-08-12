using System;
using System.Collections.Generic;
using TWDModel;

namespace TWD.Externals
{
	public class HelpshiftHelper
	{
		public static string[] CreateTags()
		{
			GameManager instance = GameManager.Instance;
			if (instance == null || instance.modelManager == null || instance.modelManager.Player == null)
			{
				return Array.Empty<string>();
			}
			return new List<string> { "language_" + SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage }.ToArray();
		}

		public static Dictionary<string, object> CreateCustomIssueFields()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			PlayerModel playerModel = GameManager.Instance.playerModel;
			if (playerModel == null)
			{
				return new Dictionary<string, object>();
			}
			dictionary.Add("player_level", CreateCustomIssueFieldItem(CifType.Number, playerModel.Level.ToString()));
			dictionary.Add("player_id", CreateCustomIssueFieldItem(CifType.SingleLine, playerModel.HashedId));
			if (playerModel.GuildModel != null)
			{
				dictionary.Add("guild_name", CreateCustomIssueFieldItem(CifType.SingleLine, playerModel.GuildModel.Name));
				dictionary.Add("guild_id", CreateCustomIssueFieldItem(CifType.SingleLine, playerModel.GuildModel.Id));
			}
			dictionary.Add("spend", CreateCustomIssueFieldItem(CifType.Number, playerModel.TotalUSDSpent.ToString()));
			if (playerModel.CurrentIAP != null)
			{
				dictionary.Add("last_purchase", CreateCustomIssueFieldItem(CifType.Dropdown, playerModel.CurrentIAP?.Created.ToShortDateString()));
			}
			dictionary.Add("country", CreateCustomIssueFieldItem(CifType.SingleLine, playerModel.Country));
			dictionary.Add("total_activity_days", CreateCustomIssueFieldItem(CifType.Number, playerModel.Blackboard.GetCounter("Counter.SessionDaysPlayed").ToString()));
			CalculateLastWeekSessionsAndDuration(playerModel, out var lastWeekDuration, out var lastWeekSessions);
			dictionary.Add("week_activity_minutes", CreateCustomIssueFieldItem(CifType.Number, (lastWeekDuration / 60000).ToString()));
			dictionary.Add("week_activity_sessions", CreateCustomIssueFieldItem(CifType.Number, lastWeekSessions.ToString()));
			dictionary.Add("game_saved", CreateCustomIssueFieldItem(CifType.Checkbox, (GameManager.Instance.GameCenterManager != null && GameManager.Instance.GameCenterManager.Authenticated) ? "true" : "false"));
			dictionary.Add("game_language", CreateCustomIssueFieldItem(CifType.SingleLine, playerModel.Language));
			return dictionary;
		}

		private static Dictionary<string, string> CreateCustomIssueFieldItem(CifType valueType, string value)
		{
			return new Dictionary<string, string>
			{
				{
					"type",
					valueType.ToString().ToLower()
				},
				{ "value", value }
			};
		}

		public static Dictionary<string, string> CreateMetadata()
		{
			GameManager instance = GameManager.Instance;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (instance == null || instance.modelManager == null || instance.modelManager.Player == null)
			{
				return new Dictionary<string, string>();
			}
			PlayerModel playerModel = instance.playerModel;
			dictionary.Add("spend", playerModel.TotalUSDSpent.ToString());
			dictionary.Add("level", playerModel.Level.ToString());
			dictionary.Add("total_activity_days", playerModel.Blackboard.GetCounter("Counter.SessionDaysPlayed").ToString());
			CalculateLastWeekSessionsAndDuration(playerModel, out var lastWeekDuration, out var lastWeekSessions);
			dictionary.Add("week_activity_minutes", (lastWeekDuration / 60000).ToString());
			dictionary.Add("week_activity_sessions", lastWeekSessions.ToString());
			if (playerModel.GuildModel != null)
			{
				dictionary.Add("guild", playerModel.GuildModel.Name);
			}
			dictionary.Add("game_saved", (instance.GameCenterManager != null && instance.GameCenterManager.Authenticated) ? "yes" : "no");
			return dictionary;
		}

		private static void CalculateLastWeekSessionsAndDuration(PlayerModel player, out long lastWeekDuration, out int lastWeekSessions)
		{
			long num = player.LifeTime - 604800000;
			long num2 = 0L;
			int num3 = 0;
			for (int i = 0; i < player.SessionHistory.Count; i++)
			{
				if (player.SessionHistory[i].StartTime > num)
				{
					num2 += player.SessionHistory[i].Length;
					num3++;
				}
			}
			lastWeekDuration = num2;
			lastWeekSessions = num3;
		}
	}
}
