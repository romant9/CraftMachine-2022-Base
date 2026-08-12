using BaseModel;

namespace TWDModel
{
	public class CreateGuildAdCommand : TWDSocialModelCommand
	{
		public string AdCreatorId { get; set; }

		public long ExpirationTimeSeconds { get; set; }

		public int AdBucket { get; set; }

		public string AdUniqueId { get; set; }

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager manager)
		{
			return new CreateGuildAdGroupCommand
			{
				ExpirationTimeSeconds = ExpirationTimeSeconds,
				AdCreatorId = AdCreatorId,
				AdBucket = AdBucket,
				AdUniqueId = AdUniqueId
			};
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelResult tWDModelResult = (modelManager as TWDModelManager).Player.PayForGuildAd(AdUniqueId);
			if (tWDModelResult == TWDModelResult.OK)
			{
				return base.Execute(modelManager);
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
