using BaseModel;

namespace TWDModel
{
	public class ModifyGuildGroupCommand : TWDGroupCommand
	{
		public string Description { get; set; }

		public GuildJoinType JoinType { get; set; }

		public string Purpose { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (!guildModel.IsValidDescriptionLength(Description))
			{
				return this;
			}
			if (!guildModel.IsValidPurpose(Purpose, tWDModelManager.GameEconomyData.ConfigData.GuildPurposeTypes))
			{
				if (!string.IsNullOrEmpty(Purpose))
				{
					return this;
				}
				Purpose = GuildModel.GetDefaultPurpose(tWDModelManager.GameEconomyData.ConfigData.GuildPurposeTypes);
			}
			if (JoinType == GuildJoinType.Closed && guildModel.AdExpireTimeStampSeconds * 1000 > guildModel.TimeStamp)
			{
				manager.Debug.LogWarning("Clearing guild ad because of guild modification. Creator: " + ((guildModel.AdCreatorId != null) ? guildModel.AdCreatorId : "") + ", TimeStamp: " + guildModel.TimeStamp + ",  AdCreationTimeStampSeconds: " + guildModel.AdCreationTimeStampSeconds + ", AdExpireTimeStampSeconds: " + guildModel.AdExpireTimeStampSeconds + ", AdAvailableTimeSeconds: " + guildModel.AdAvailableTimeSeconds);
				guildModel.ClearGuildAd();
			}
			guildModel.Description = Description;
			guildModel.JoinType = JoinType;
			if (guildModel.Purpose != Purpose)
			{
				if (guildModel.IsPurposeEditable(tWDModelManager.GameEconomyData.ConfigData.GuildPurposeChangeInterval))
				{
					guildModel.Purpose = Purpose;
					guildModel.LastPurposeEditTimeStamp = guildModel.TimeStamp;
				}
				else
				{
					manager.Debug.LogWarning("Trying to change guild purpose before purpose editing interval has passed. Guild id: " + guildModel.Id + ", TimeStamp: " + guildModel.TimeStamp + ",  LastPurposeEditTimeStamp: " + guildModel.LastPurposeEditTimeStamp + ", editable time interval (secs): " + tWDModelManager.GameEconomyData.ConfigData.GuildPurposeChangeInterval);
				}
			}
			guildModel.NotifyChange("GuildModified");
			SaveGroupModel(manager);
			return this;
		}
	}
}
