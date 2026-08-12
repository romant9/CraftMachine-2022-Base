using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ReportClientErrorCommand : ModelCommand
	{
		public enum LogLevel
		{
			Error = 0,
			Warning = 1
		}

		private const string GuildBattleActivityIndicatorMessagePrefix = "GuildBattleActivityIndicator: self stale live mission data caused activity indicator.";

		public string Message;

		public LogLevel Level;

		public string GuildBattleActivityIndicatorMissionId;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (Level == LogLevel.Error)
			{
				tWDModelManager.Debug.LogError("Client Error: " + Message);
			}
			else
			{
				tWDModelManager.Debug.LogWarning("Client Warning: " + Message);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}

		private bool IsGuildBattleActivityIndicatorReportValidOnServer(TWDModelManager manager)
		{
			if (string.IsNullOrEmpty(GuildBattleActivityIndicatorMissionId))
			{
				return false;
			}
			PlayerModel playerModel = manager?.Player;
			if (playerModel == null || string.IsNullOrEmpty(playerModel.HashedId))
			{
				return false;
			}
			Dictionary<string, GuildBattleModel.LiveMissionData> dictionary = playerModel.GuildModel?.GvGSeasonModel?.GuildWarModel?.CurrentBattle?.LiveMissionDataPerPlayer;
			if (dictionary == null)
			{
				return false;
			}
			if (!dictionary.TryGetValue(playerModel.HashedId, out var value) || value == null)
			{
				return false;
			}
			return value.LastAttackedMissionId == GuildBattleActivityIndicatorMissionId;
		}
	}
}
