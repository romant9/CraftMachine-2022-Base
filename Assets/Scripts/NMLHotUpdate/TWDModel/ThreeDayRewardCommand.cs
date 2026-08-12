using BaseModel;

namespace TWDModel
{
	public class ThreeDayRewardCommand : ModelCommand
	{
		public int RewardIndex { get; set; }

		public ThreeDayRewardCommand()
		{
		}

		public ThreeDayRewardCommand(int index)
		{
			RewardIndex = index;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ThreeDayModel threeDayModel = tWDModelManager.Player.ThreeDayModel;
			if (threeDayModel.RewardsStatus.Count <= RewardIndex || threeDayModel.RewardsStatus[RewardIndex] != ThreeDayRewardStatus.Unlock)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.ThreeDayModel.RewardIndex(RewardIndex);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
