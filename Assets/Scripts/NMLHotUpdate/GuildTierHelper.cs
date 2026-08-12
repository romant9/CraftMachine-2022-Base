using TWDModel;
using UnityEngine;

public class GuildTierHelper
{
	private static GameEconomyData cachedGED;

	private static GameEconomyData gameEconomyData
	{
		get
		{
			if (cachedGED == null)
			{
				cachedGED = GameManager.Instance.gameEconomyData;
			}
			return cachedGED;
		}
	}

	public static GuildTierDefinition GetCurrentGuildTier()
	{
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (guildModel != null)
		{
			return gameEconomyData.GetGuildTierDefinition(guildModel.GuildBattleTier);
		}
		return null;
	}

	public static float GetCurrentProgressToNextTier()
	{
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (guildModel == null)
		{
			return 0f;
		}
		GuildTierDefinition guildTierDefinition = gameEconomyData.GetGuildTierDefinition(guildModel.GuildBattleTier);
		GuildTierDefinition nextGuildTier = GetNextGuildTier(guildModel.GuildBattleTier);
		if (nextGuildTier == null)
		{
			return 1f;
		}
		return Mathf.Clamp01((float)(guildModel.CurrentVictoryPoints - guildTierDefinition.VictoryPointsRequired) / (float)(nextGuildTier.VictoryPointsRequired - guildTierDefinition.VictoryPointsRequired));
	}

	public static GuildTierDefinition GetNextGuildTier(int guildTier)
	{
		if (guildTier > 1)
		{
			return gameEconomyData.GetGuildTierDefinition(guildTier - 1);
		}
		return null;
	}

	public static GuildTierDefinition GetPreviousGuildTier(int guildTier)
	{
		if (guildTier < gameEconomyData.GuildWarConfig.GuildBattleMinimumTier)
		{
			return gameEconomyData.GetGuildTierDefinition(guildTier + 1);
		}
		return null;
	}

	public static int GetVictoryPointsOnCurrentTier()
	{
		GuildModel guildModel = GameManager.Instance.playerModel.GuildModel;
		if (guildModel == null)
		{
			return 0;
		}
		GuildTierDefinition guildTierDefinition = gameEconomyData.GetGuildTierDefinition(guildModel.GuildBattleTier);
		return guildModel.CurrentVictoryPoints - guildTierDefinition.VictoryPointsRequired;
	}

	public static int GetVictoryPointsToNextTier(int fromTier)
	{
		GuildTierDefinition guildTierDefinition = gameEconomyData.GetGuildTierDefinition(fromTier);
		GuildTierDefinition nextGuildTier = GetNextGuildTier(fromTier);
		if (nextGuildTier == null)
		{
			return 0;
		}
		return nextGuildTier.VictoryPointsRequired - guildTierDefinition.VictoryPointsRequired;
	}

	public static int GetVictoryPointsToTier(int toTier)
	{
		if (toTier < gameEconomyData.GuildWarConfig.GuildBattleMinimumTier)
		{
			return GetVictoryPointsToNextTier(toTier + 1);
		}
		return 0;
	}
}
