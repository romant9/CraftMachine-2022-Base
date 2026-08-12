using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class AcceptSurvivorCommand : PhoneCallBaseCommand
	{
		public NewSurvivorSource Source { get; private set; }

		public AcceptSurvivorCommand()
		{
		}

		public AcceptSurvivorCommand(SurvivorModel newSurvivor, NewSurvivorSource caller)
			: base(newSurvivor)
		{
			Source = caller;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			PlayerModel playerModel = (PlayerModel)manager.GetPlayer();
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			if (model == null)
			{
				tWDModelManager.Debug.LogError("Tried to accept NULL survivor");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (Source == NewSurvivorSource.Phone && !CheckMatchAgainstPhoneCall(tWDModelManager, model))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			TWDModelResult tWDModelResult = ((!playerModel.SurvivorContainer.AddSurvivor(model)) ? TWDModelResult.NoAvailableSlotsForNewSurvivor : TWDModelResult.OK);
			if (tWDModelResult == TWDModelResult.OK)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				int num = 0;
				BuildingModel building = playerModel.Camp.GetBuilding("RadioTent");
				if (building != null)
				{
					dictionary.Add("radio_tent_level", building.Level.ToString());
				}
				if (Source == NewSurvivorSource.Mission)
				{
					if (playerModel.Combat != null)
					{
						playerModel.Combat.ResolveCasualty(model, null);
						playerModel.Combat.ClearExtraSurvivors();
					}
					new Metrics((TWDModelManager)manager).AddFind().AddSurvivor(model).AddFromSurvivorSource(Source)
						.AddMissionType()
						.AddAccept()
						.Send();
				}
				else if (Source == NewSurvivorSource.Phone)
				{
					bool flag = playerModel.PhoneCall.CanClaimEntireMultiLootsList();
					if (flag)
					{
						tWDModelManager.Metrics.AddFind().AddSurvivor(model).AddFromSurvivorSource(Source)
							.AddFromLootDecision(LootDecision.Accept)
							.Send();
					}
					else
					{
						for (int i = 0; i < playerModel.PhoneCall.LootsList.Count; i++)
						{
							if (playerModel.PhoneCall.LootsList[i].DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
							{
								SurvivorModel generatedSurvivor = playerModel.PhoneCall.LootsList[i].GeneratedSurvivor;
								if (generatedSurvivor != null)
								{
									if (generatedSurvivor == model)
									{
										tWDModelManager.Metrics.AddFind().AddSurvivor(generatedSurvivor).AddFromSurvivorSource(Source)
											.AddFromLootDecision(LootDecision.Accept)
											.Send();
									}
									else
									{
										tWDModelManager.Metrics.AddIgnore().AddSurvivor(generatedSurvivor).AddFromSurvivorSource(Source)
											.Send();
									}
								}
							}
							else if (playerModel.PhoneCall.LootsList[i].DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
							{
								CurrencyType rewardedCurrency = playerModel.PhoneCall.LootsList[i].RewardedCurrency;
								int rewardedAmount = playerModel.PhoneCall.LootsList[i].RewardedAmount;
								tWDModelManager.Metrics.AddIgnore().AddHeroToken(rewardedCurrency, rewardedAmount).AddRadioCall()
									.Send();
							}
						}
					}
					int num2 = -1;
					if (flag)
					{
						num2 = playerModel.PhoneCall.SolveLootIndexForSurvivor(model);
						if (num2 == -1)
						{
							return new NGModelCommandRespond(this, TWDModelResult.Error);
						}
						if (playerModel.PhoneCall.IsLootClaimed(num2))
						{
							return new NGModelCommandRespond(this, TWDModelResult.Error);
						}
					}
					playerModel.PhoneCall.ClearPendingPhoneCallLoot(model, num2);
				}
				for (int j = 0; j < model.EquipmentItems.Count; j++)
				{
					if (Source == NewSurvivorSource.Mission)
					{
						tWDModelManager.Metrics.AddFind().AddEquipment(model.EquipmentItems[j]).AddFromSurvivorSource(NewSurvivorSource.Mission)
							.AddMissionType()
							.Send();
					}
					else
					{
						tWDModelManager.Metrics.AddFind().AddEquipment(model.EquipmentItems[j]).AddFromSurvivorSource(NewSurvivorSource.Phone)
							.Send();
					}
				}
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
