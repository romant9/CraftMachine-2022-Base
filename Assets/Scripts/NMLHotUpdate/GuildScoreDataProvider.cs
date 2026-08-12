using System.Collections.Generic;

public abstract class GuildScoreDataProvider : ScoreDataProvider
{
	protected override ScoreDataEntry CreateEntry()
	{
		return new GuildScoreDataEntry();
	}

	protected override void AddCurrentPlayerData(List<ScoreDataEntry> data)
	{
		if (!GameManager.Instance.playerModel.IsGuildMember)
		{
			return;
		}
		GuildScoreDataEntry guildScoreDataEntry = null;
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].Id == GameManager.Instance.playerModel.GuildId)
			{
				guildScoreDataEntry = data[i] as GuildScoreDataEntry;
				break;
			}
		}
		if (guildScoreDataEntry == null)
		{
			guildScoreDataEntry = CreateEntry() as GuildScoreDataEntry;
			data.Add(guildScoreDataEntry);
		}
		guildScoreDataEntry.Name = GameManager.Instance.playerModel.GuildModel.Name;
		guildScoreDataEntry.Score = GameManager.Instance.playerModel.GuildModel.CurrentChallengeStars;
		guildScoreDataEntry.Id = GameManager.Instance.playerModel.GuildModel.Id;
	}
}
