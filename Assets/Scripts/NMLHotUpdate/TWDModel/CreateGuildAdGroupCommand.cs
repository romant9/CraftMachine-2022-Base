using BaseModel;

namespace TWDModel
{
	public class CreateGuildAdGroupCommand : TWDGroupCommand
	{
		public string AdCreatorId { get; set; }

		public long ExpirationTimeSeconds { get; set; }

		public int AdBucket { get; set; }

		public string AdUniqueId { get; set; }

		public override GroupCommandBase Execute(ModelManager manager)
		{
			GuildModel guildModel = (GuildModel)manager.GetGroupModel(GroupId);
			if (manager != null && guildModel != null)
			{
				manager.Debug.LogWarning("Attempting to create guild ad. Creator: " + AdCreatorId + ", TimeStamp: " + guildModel.TimeStamp + ",  AdCreationTimeStampSeconds: " + guildModel.AdCreationTimeStampSeconds + ", AdExpireTimeStampSeconds: " + guildModel.AdExpireTimeStampSeconds + ", AdAvailableTimeSeconds: " + guildModel.AdAvailableTimeSeconds);
			}
			if (guildModel.CreateAd(AdCreatorId, ExpirationTimeSeconds, AdBucket, AdUniqueId) == TWDModelResult.OK)
			{
				if (manager != null && guildModel != null)
				{
					manager.Debug.LogWarning("Created guild ad. Creator: " + AdCreatorId + ", TimeStamp: " + guildModel.TimeStamp + ",  AdCreationTimeStampSeconds: " + guildModel.AdCreationTimeStampSeconds + ", AdExpireTimeStampSeconds: " + guildModel.AdExpireTimeStampSeconds + ", AdAvailableTimeSeconds: " + guildModel.AdAvailableTimeSeconds);
				}
				SaveGroupModel(manager);
			}
			return this;
		}
	}
}
