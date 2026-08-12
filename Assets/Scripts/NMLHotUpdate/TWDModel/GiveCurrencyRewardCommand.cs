using BaseModel;

namespace TWDModel
{
	public class GiveCurrencyRewardCommand : ModelCommand
	{
		public RewardCurrency Reward;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager manager2)
			{
				if (Reward == null)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				Reward.Give(manager2);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
