using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class UpdateGuildBattleHighscoresGroupCommand : TWDGroupCommand
	{
		public long LastLeaderboardsUpdateTimestamp { get; private set; }

		public List<ScoreDataEntry> GuildScores { get; private set; }

		public List<ScoreDataEntry> PlayerHighscores { get; private set; }

		public bool BattleEnded { get; set; }

		public UpdateGuildBattleHighscoresGroupCommand(long lastLeaderboardsUpdateTimestamp, List<ScoreDataEntry> guildScores, List<ScoreDataEntry> playerHighscores, bool battleEnd)
		{
			GuildScores = guildScores;
			PlayerHighscores = playerHighscores;
			LastLeaderboardsUpdateTimestamp = lastLeaderboardsUpdateTimestamp;
			BattleEnded = battleEnd;
		}

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLogError("UpdateGuildBattleHighscoresGroupCommand: No Guild found with GroupId: " + GroupId);
				return this;
			}
			guildModel.GuildWarModel.CurrentBattle.UpdateGuildBattleHighScores(GuildScores, PlayerHighscores, LastLeaderboardsUpdateTimestamp);
			if (BattleEnded)
			{
				guildModel.GuildWarModel.SaveGuildBattleInfo();
			}
			return this;
		}
	}
}
