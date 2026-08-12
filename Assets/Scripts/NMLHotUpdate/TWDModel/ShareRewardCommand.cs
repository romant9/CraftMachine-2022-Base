using BaseModel;

namespace TWDModel
{
	public class ShareRewardCommand : ModelCommand
	{
		public ShareType ShareType;

		public ShareRewardCommand()
		{
		}

		public ShareRewardCommand(ShareManagerModel shareManagerModel, ShareType shareType)
			: base(shareManagerModel)
		{
			ShareType = shareType;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!manager.GetModel<ShareManagerModel>(base.ModelId).GiveShareReward(ShareType))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
