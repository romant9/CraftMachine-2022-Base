using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class AcceptSurvivorTokenCommand : ModelCommand
	{
		public LootEntry ModelLootEntry { get; private set; }

		public AcceptSurvivorTokenCommand()
		{
		}

		public AcceptSurvivorTokenCommand(LootEntry lootEntry)
			: base(lootEntry)
		{
			ModelLootEntry = lootEntry;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			LootEntry model = OfflineManager.IsLoadDataManager ? ModelLootEntry : manager.GetModel<LootEntry>(base.ModelId);
			PlayerModel playerModel = (PlayerModel)manager.GetPlayer();
			int lootIndex = -1;
			if (OfflineManager.IsLoadDataManager && OfflineManager.IsFakeExecuteCommands && model != null)
			{
				lootIndex = playerModel.PhoneCall.LootsList.IndexOf(model);
			}
			else
			{
				if (playerModel.PhoneCall.CanClaimEntireMultiLootsList())
				{
					playerModel.PhoneCall.ContainsLootEntry(model, out lootIndex);
					if (lootIndex == -1)
					{
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
					if (playerModel.PhoneCall.IsLootClaimed(lootIndex))
					{
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
				}
			}
			CurrencyType rewardedCurrency = model.RewardedCurrency;
			int rewardedAmount = model.RewardedAmount;
			CurrencyModel currency = playerModel.GetCurrency(rewardedCurrency);
			if (OfflineManager.IsLoadDataManager && OfflineManager.IsFakeExecuteCommands)
			{
				rewardedAmount = HelpersUI.GetActualRewardValue(CallCraft.Instance.CurrentCallButton, model.RewardedAmount);
			}
			currency.Add(rewardedAmount);

			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = 0;
			BuildingModel building = playerModel.Camp.GetBuilding("RadioTent");
			if (building != null)
			{
				dictionary.Add("radio_tent_level", building.Level.ToString());
			}
			if (!playerModel.PhoneCall.CanClaimEntireMultiLootsList())
			{
				for (int i = 0; i < playerModel.PhoneCall.LootsList.Count; i++)
				{
					if (playerModel.PhoneCall.LootsList[i].DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
					{
						SurvivorModel generatedSurvivor = playerModel.PhoneCall.LootsList[i].GeneratedSurvivor;
						if (generatedSurvivor != null)
						{
							tWDModelManager.Metrics.AddIgnore().AddSurvivor(generatedSurvivor).AddFromSurvivorSource(NewSurvivorSource.Phone)
								.Send();
						}
					}
					else if (playerModel.PhoneCall.LootsList[i].DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken && model != playerModel.PhoneCall.LootsList[i])
					{
						CurrencyType rewardedCurrency2 = playerModel.PhoneCall.LootsList[i].RewardedCurrency;
						int rewardedAmount2 = playerModel.PhoneCall.LootsList[i].RewardedAmount;
						tWDModelManager.Metrics.AddIgnore().AddHeroToken(rewardedCurrency2, rewardedAmount2).AddRadioCall()
							.Send();
					}
				}
			}
			playerModel.PhoneCall.ClearPendingPhoneCallLoot(null, lootIndex);
			tWDModelManager.Metrics.AddFind().AddResources(rewardedCurrency, rewardedAmount, currency.LastAdded).AddRadioCall()
				.AddFromLootDecision(LootDecision.Accept)
				.Send();
			tWDModelManager.Metrics.TdEventType = "RadioCall_Accept";
			tWDModelManager.Metrics.TdEventPropertyTypes = new List<string> { "RadioCall", "RadioCall_Resource_Num", "RadioCall_Acceptance" };
			tWDModelManager.Metrics.SendTdEvent();
			TWDModelResult result = TWDModelResult.OK;
			return new NGModelCommandRespond(this, result);
		}
	}
}
