using BaseModel;

namespace TWDModel
{
	public class GiveShareRewardCommand : ModelCommand
	{
		public GiveShareRewardCommand()
		{
		}

		public GiveShareRewardCommand(SurvivorModel unlockedHero)
			: base(unlockedHero)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			if (model == null)
			{
				((TWDModelManager)manager).Debug.LogError("Tried to give reward for NULL Hero Model");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (!model.IsHero)
			{
				((TWDModelManager)manager).Debug.LogError("Cannot give reward for share. Survivor is NOT hero.");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (model.UnlockShareRewardedAmount > 0)
			{
				((TWDModelManager)manager).Debug.LogError("Cannot give reward more than once.");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			RewardCurrency unlockShareRewardForSurvivor = tWDModelManager.GameEconomyData.GetUnlockShareRewardForSurvivor(model.Definition);
			TWDModelResult result = ((unlockShareRewardForSurvivor == null) ? TWDModelResult.Error : TWDModelResult.OK);
			if (unlockShareRewardForSurvivor != null && unlockShareRewardForSurvivor.Amount > 0)
			{
				unlockShareRewardForSurvivor.Give(tWDModelManager);
				tWDModelManager.Blackboard.SetCounter(model.UnlockShareRewardKey, unlockShareRewardForSurvivor.Amount);
				if (unlockShareRewardForSurvivor != null)
				{
					RewardCurrency rewardCurrency = unlockShareRewardForSurvivor;
					if (rewardCurrency.Amount > 0)
					{
						tWDModelManager.Metrics.metricsResourcesData.SetOrAdd(rewardCurrency.CurrencyType, rewardCurrency.AmountActuallyAdded, rewardCurrency.GetOverflowAmount());
						tWDModelManager.Metrics.AddFind().AddResources().AddShare()
							.AddHeroUnlock()
							.AddSurvivor(model)
							.Send();
					}
				}
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
