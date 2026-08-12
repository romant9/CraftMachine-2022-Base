using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class RejectSurvivorCommand : PhoneCallBaseCommand
	{
		public NewSurvivorSource source;

		public RejectSurvivorCommand()
		{
		}

		public RejectSurvivorCommand(SurvivorModel newSurvivor, NewSurvivorSource caller)
			: base(newSurvivor)
		{
			source = caller;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			PlayerModel playerModel = (PlayerModel)manager.GetPlayer();
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			int num = -1;
			if (source == NewSurvivorSource.Phone)
			{
				if (!CheckMatchAgainstPhoneCall(tWDModelManager, model))
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (playerModel.PhoneCall.CanClaimEntireMultiLootsList())
				{
					num = playerModel.PhoneCall.SolveLootIndexForSurvivor(model);
					if (num == -1)
					{
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
					if (playerModel.PhoneCall.IsLootClaimed(num))
					{
						return new NGModelCommandRespond(this, TWDModelResult.Error);
					}
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num2 = 0;
			BuildingModel building = playerModel.Camp.GetBuilding("RadioTent");
			if (building != null)
			{
				dictionary.Add("radio_tent_level", building.Level.ToString());
			}
			Dictionary<CurrencyType, OverflowableAmount> dictionary2 = model.GetDemoteCashier().Refund(100, dontAllowMultiplier: true);
			if (source == NewSurvivorSource.Phone && !playerModel.PhoneCall.CanClaimEntireMultiLootsList())
			{
				for (int i = 0; i < playerModel.PhoneCall.LootsList.Count; i++)
				{
					if (playerModel.PhoneCall.LootsList[i].DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
					{
						SurvivorModel generatedSurvivor = playerModel.PhoneCall.LootsList[i].GeneratedSurvivor;
						if (generatedSurvivor != null && generatedSurvivor != model)
						{
							tWDModelManager.Metrics.AddIgnore().AddSurvivor(generatedSurvivor).AddFromSurvivorSource(source)
								.Send();
						}
					}
					else if (playerModel.PhoneCall.LootsList[i].DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
					{
						CurrencyType rewardedCurrency = playerModel.PhoneCall.LootsList[i].RewardedCurrency;
						int rewardedAmount = playerModel.PhoneCall.LootsList[i].RewardedAmount;
						tWDModelManager.Metrics.AddIgnore().AddHeroToken(rewardedCurrency, rewardedAmount).AddFromSurvivorSource(source)
							.Send();
					}
				}
			}
			if (dictionary2.Count > 0)
			{
				if (source == NewSurvivorSource.Mission)
				{
					tWDModelManager.Metrics.AddFind().AddResources(dictionary2).AddFromSurvivorSource(NewSurvivorSource.Mission)
						.AddMissionType()
						.AddFromLootDecision(LootDecision.Reject)
						.Send();
				}
				else
				{
					tWDModelManager.Metrics.AddFind().AddResources(dictionary2).AddFromSurvivorSource(NewSurvivorSource.Phone)
						.AddFromLootDecision(LootDecision.Reject)
						.Send();
				}
			}
			playerModel.PhoneCall.ClearPendingPhoneCallLoot(null, num);
			tWDModelManager.Metrics.TdEventType = "RadioCall_Accept";
			tWDModelManager.Metrics.TdEventPropertyTypes = new List<string> { "RadioCall", "RadioCall_Resource_Num", "RadioCall_Acceptance" };
			tWDModelManager.Metrics.SendTdEvent();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
