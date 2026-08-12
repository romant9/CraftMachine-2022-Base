using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class SetNextBattleMatchmakingInfoGroupCommand : TWDValidationGroupCommand
	{
		public int RandomSeed { get; private set; }

		public string GuildBattleMatchmakingInfo { get; private set; }

		public bool IsFakeBattle { get; private set; }

		public long StartBattleTimestamp { get; private set; }

		public List<string> RegisteredPlayersList { get; set; }

		public SetNextBattleMatchmakingInfoGroupCommand()
		{
		}

		public SetNextBattleMatchmakingInfoGroupCommand(int randomSeed, string guildBattleMatchmakingInfo, bool isFakeBattle, long startBattleTimestamp)
		{
			RandomSeed = randomSeed;
			GuildBattleMatchmakingInfo = guildBattleMatchmakingInfo;
			IsFakeBattle = isFakeBattle;
			StartBattleTimestamp = startBattleTimestamp;
		}

		protected override TWDValidationCommandResult Validate(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			if (guildModel == null)
			{
				manager.GvGLog("SetNextBattleMatchmakingInfoGroupCommand: No Guild found with GroupId: " + GroupId);
				return TWDValidationCommandResult.Error;
			}
			if (guildModel.GuildWarModel.NextBattlesOpponentMatchmakingInfo.Exists((GuildBattleOpponentMatchmakingEntry m) => m.StartBattleTimeSlot == StartBattleTimestamp))
			{
				manager.GvGLogError($"SetNextBattleMatchmakingInfoGroupCommand: there already is a match setup for timeslot {StartBattleTimestamp}");
				return TWDValidationCommandResult.Canceled;
			}
			if (string.IsNullOrEmpty(GuildBattleMatchmakingInfo))
			{
				manager.GvGLogError("SetNextBattleMatchmakingInfoGroupCommand: GuildBattleMatchmakingInfo is null", guildModel);
				return TWDValidationCommandResult.Error;
			}
			GuildBattleMatchmakingInfo guildBattleMatchmakingInfo = manager.GetMessageSerializer().DeserializeObject<GuildBattleMatchmakingInfo>(GuildBattleMatchmakingInfo);
			if (guildBattleMatchmakingInfo == null)
			{
				manager.GvGLogError("SetNextBattleMatchmakingInfoGroupCommand: Failed to deserialize the GuildBattleMatchmakingInfo", guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (!IsFakeBattle && guildBattleMatchmakingInfo.RegisteredPlayersList.Count == 0)
			{
				manager.GvGLogError("SetNextBattleMatchmakingInfoGroupCommand: Starting battle with 0 participants", guildModel);
				return TWDValidationCommandResult.Error;
			}
			if (guildBattleMatchmakingInfo.PlayerInfoSnapshot.Count < 3)
			{
				manager.GvGLogError("SetNextBattleMatchmakingInfoGroupCommand: PlayerInfoSnapshot doesnt contain enough data", guildModel);
				return TWDValidationCommandResult.Error;
			}
			return TWDValidationCommandResult.OK;
		}

		protected override bool ExecuteInternal(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)(manager as TWDModelManager).GetGroupModel(GroupId);
			GuildBattleOpponentMatchmakingEntry entry = new GuildBattleOpponentMatchmakingEntry
			{
				IsFakeBattle = IsFakeBattle,
				RandomSeed = RandomSeed,
				OpponentMatchmakingInfo = GuildBattleMatchmakingInfo,
				StartBattleTimeSlot = StartBattleTimestamp
			};
			bool flag = guildModel.GuildWarModel.SaveOpponentMatchmakingEntry(entry);
			if (flag && RegisteredPlayersList != null && RegisteredPlayersList.Count > 0)
			{
				manager.GvGLog("SetNextBattleMatchmakingInfoGroupCommand:" + RegisteredPlayersList.Count);
				if (guildModel.GuildWarModel.RegisteredPlayersForBattleSlot.TryGetValue(StartBattleTimestamp, out var value) && value != null && value.Count != RegisteredPlayersList.Count)
				{
					string text = string.Join(",", value);
					string text2 = string.Join(",", RegisteredPlayersList);
					value.Clear();
					value.AddRange(RegisteredPlayersList);
					manager.GvGLogWarning("SetNextBattleMatchmakingInfoGroupCommand#" + GroupId + "#" + text + "#" + text2);
				}
			}
			return flag;
		}
	}
}
