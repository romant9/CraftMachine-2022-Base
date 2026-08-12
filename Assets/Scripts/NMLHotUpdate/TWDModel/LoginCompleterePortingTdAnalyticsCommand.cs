using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class LoginCompleterePortingTdAnalyticsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			tWDModelManager.TdMetrics.SetEventType("login_server_state");
			foreach (EquipmentModel.ConsumableType value in Enum.GetValues(typeof(EquipmentModel.ConsumableType)))
			{
				if (value != EquipmentModel.ConsumableType.Unknown)
				{
					int count = tWDModelManager.Player.Equipment.GetConsumablesOfType(value).Count;
					switch (value)
					{
					case EquipmentModel.ConsumableType.Grenade:
						tWDModelManager.TdMetrics.AddProperty("tool_grenade_state", Convert.ToString(count));
						break;
					case EquipmentModel.ConsumableType.MedKit:
						tWDModelManager.TdMetrics.AddProperty("tool_medkit_state", Convert.ToSingle(count));
						break;
					case EquipmentModel.ConsumableType.Flare:
						tWDModelManager.TdMetrics.AddProperty("tool_flare_state", Convert.ToSingle(count));
						break;
					case EquipmentModel.ConsumableType.BlastGrenade:
						tWDModelManager.TdMetrics.AddProperty("tool_blast_state", Convert.ToSingle(count));
						break;
					case EquipmentModel.ConsumableType.Gore:
						tWDModelManager.TdMetrics.AddProperty("tool_gore_state", Convert.ToSingle(count));
						break;
					}
				}
			}
			WeeklyChallengeModel weeklyChallenge = tWDModelManager.Player.WeeklyChallenge;
			tWDModelManager.TdMetrics.AddProperty("challenge_personalstart_state", Convert.ToSingle(weeklyChallenge.NumberStars.ToString()));
			tWDModelManager.TdMetrics.AddProperty("challenge_guildstart_state", Convert.ToSingle(weeklyChallenge.NumberStarsGuild.ToString()));
			CurrencyModel currency = tWDModelManager.Player.GetCurrency(CurrencyType.EquipmentToken1min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_workshop_1min", Convert.ToSingle(currency.Value));
			CurrencyModel currency2 = tWDModelManager.Player.GetCurrency(CurrencyType.EquipmentToken10min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_workshop_10min", Convert.ToSingle(currency2.Value));
			CurrencyModel currency3 = tWDModelManager.Player.GetCurrency(CurrencyType.EquipmentToken20min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_workshop_20min", Convert.ToSingle(currency3.Value));
			CurrencyModel currency4 = tWDModelManager.Player.GetCurrency(CurrencyType.EquipmentToken1h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_workshop_1h", Convert.ToSingle(currency4.Value));
			CurrencyModel currency5 = tWDModelManager.Player.GetCurrency(CurrencyType.EquipmentToken3h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_workshop_3h", Convert.ToSingle(currency5.Value));
			CurrencyModel currency6 = tWDModelManager.Player.GetCurrency(CurrencyType.EquipmentToken7h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_workshop_7h", Convert.ToSingle(currency6.Value));
			CurrencyModel currency7 = tWDModelManager.Player.GetCurrency(CurrencyType.EquipmentToken14h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_workshop_14h", Convert.ToSingle(currency7.Value));
			CurrencyModel currency8 = tWDModelManager.Player.GetCurrency(CurrencyType.EquipmentTokenBP);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_workshop_max", Convert.ToSingle(currency8.Value));
			CurrencyModel currency9 = tWDModelManager.Player.GetCurrency(CurrencyType.TrainingToken5min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_training_5min", Convert.ToSingle(currency9.Value));
			CurrencyModel currency10 = tWDModelManager.Player.GetCurrency(CurrencyType.TrainingToken20min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_training_20min", Convert.ToSingle(currency10.Value));
			CurrencyModel currency11 = tWDModelManager.Player.GetCurrency(CurrencyType.TrainingToken1h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_training_1h", Convert.ToSingle(currency11.Value));
			CurrencyModel currency12 = tWDModelManager.Player.GetCurrency(CurrencyType.TrainingToken3h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_training_3h", Convert.ToSingle(currency12.Value));
			CurrencyModel currency13 = tWDModelManager.Player.GetCurrency(CurrencyType.TrainingToken8h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_training_8h", Convert.ToSingle(currency13.Value));
			CurrencyModel currency14 = tWDModelManager.Player.GetCurrency(CurrencyType.TrainingToken16h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_training_16h", Convert.ToSingle(currency14.Value));
			CurrencyModel currency15 = tWDModelManager.Player.GetCurrency(CurrencyType.TrainingTokenBP);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_training_max", Convert.ToSingle(currency15.Value));
			CurrencyModel currency16 = tWDModelManager.Player.GetCurrency(CurrencyType.BuildingToken1min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_1min", Convert.ToSingle(currency16.Value));
			CurrencyModel currency17 = tWDModelManager.Player.GetCurrency(CurrencyType.BuildingToken5min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_5min", Convert.ToSingle(currency17.Value));
			CurrencyModel currency18 = tWDModelManager.Player.GetCurrency(CurrencyType.BuildingToken10min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_10min", Convert.ToSingle(currency18.Value));
			CurrencyModel currency19 = tWDModelManager.Player.GetCurrency(CurrencyType.BuildingToken30min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_30min", Convert.ToSingle(currency19.Value));
			CurrencyModel currency20 = tWDModelManager.Player.GetCurrency(CurrencyType.BuildingToken1h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_1h", Convert.ToSingle(currency20.Value));
			CurrencyModel currency21 = tWDModelManager.Player.GetCurrency(CurrencyType.BuildingToken6h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_6h", Convert.ToSingle(currency21.Value));
			CurrencyModel currency22 = tWDModelManager.Player.GetCurrency(CurrencyType.BuildingToken12h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_12h", Convert.ToSingle(currency22.Value));
			CurrencyModel currency23 = tWDModelManager.Player.GetCurrency(CurrencyType.BuildingToken24h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_24h", Convert.ToSingle(currency23.Value));
			CurrencyModel currency24 = tWDModelManager.Player.GetCurrency(CurrencyType.SuperBuildingTokenBP);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_building_max", Convert.ToSingle(currency24.Value));
			CurrencyModel currency25 = tWDModelManager.Player.GetCurrency(CurrencyType.HealingToken1min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_healing_1min", Convert.ToSingle(currency25.Value));
			CurrencyModel currency26 = tWDModelManager.Player.GetCurrency(CurrencyType.HealingToken5min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_healing_5min", Convert.ToSingle(currency26.Value));
			CurrencyModel currency27 = tWDModelManager.Player.GetCurrency(CurrencyType.HealingToken10min);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_healing_10min", Convert.ToSingle(currency27.Value));
			CurrencyModel currency28 = tWDModelManager.Player.GetCurrency(CurrencyType.HealingToken1h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_healing_1h", Convert.ToSingle(currency28.Value));
			CurrencyModel currency29 = tWDModelManager.Player.GetCurrency(CurrencyType.HealingToken2h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_healing_2h", Convert.ToSingle(currency29.Value));
			CurrencyModel currency30 = tWDModelManager.Player.GetCurrency(CurrencyType.HealingToken4h);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_healing_4h", Convert.ToSingle(currency30.Value));
			CurrencyModel currency31 = tWDModelManager.Player.GetCurrency(CurrencyType.HealingTokenBP);
			tWDModelManager.TdMetrics.AddProperty("speeduptoken_healing_max", Convert.ToSingle(currency31.Value));
			CurrencyModel currency32 = tWDModelManager.Player.GetCurrency(CurrencyType.Supplies);
			tWDModelManager.TdMetrics.AddProperty("supplies", Convert.ToSingle(currency32.TotalValue));
			CurrencyModel currency33 = tWDModelManager.Player.GetCurrency(CurrencyType.SurvivalPoints);
			tWDModelManager.TdMetrics.AddProperty("survivalPoints", Convert.ToSingle(currency33.TotalValue));
			CurrencyModel currency34 = tWDModelManager.Player.GetCurrency(CurrencyType.ApocalypticEquipToken);
			tWDModelManager.TdMetrics.AddProperty("apocalypticEquipToken", Convert.ToSingle(currency34.Value));
			List<EquipmentItemModel> allEquipments = tWDModelManager.Player.Equipment.GetAllEquipments();
			if (allEquipments != null && allEquipments.Count > 0)
			{
				List<LoginServerStateApocalweaponState> list = new List<LoginServerStateApocalweaponState>();
				foreach (EquipmentItemModel item in allEquipments)
				{
					if (item.RarityLevel >= 5)
					{
						int breakthroughLevel = item.BreakthroughLevel;
						list.Add(new LoginServerStateApocalweaponState(item.Definition.ID, item.Level, breakthroughLevel, item.RarityLevel));
					}
				}
				tWDModelManager.TdMetrics.AddProperty("apocalweapon_state", list);
			}
			tWDModelManager.TdMetrics.Send();
			tWDModelManager.TdMetrics.SetEventType("login_server_survivor_state");
			tWDModelManager.TdMetrics.AddProperty("council_level", tWDModelManager.Player.CouncilLevel);
			List<SurvivorModel> list2 = tWDModelManager.Player.SurvivorContainer.Survivors.Where((SurvivorModel x) => LoginServerReportSurvivor.ReportSurvivorIds.Contains(x.Definition.ID)).ToList();
			List<LoginServerSurvivorState> list3 = new List<LoginServerSurvivorState>();
			foreach (string reportSurvivorId in LoginServerReportSurvivor.ReportSurvivorIds)
			{
				SurvivorModel survivorModel = list2.Find((SurvivorModel x) => x.Definition.ID == reportSurvivorId);
				if (survivorModel != null)
				{
					int num = 0;
					num += survivorModel.SurvivorRarityLevel + 1;
					foreach (UpgradeTraitsData upgradeTrait in survivorModel.UpgradeTraits)
					{
						if (!upgradeTrait.IsTactical)
						{
							num += upgradeTrait.RarityLevel + 1;
						}
					}
					list3.Add(new LoginServerSurvivorState(reportSurvivorId, num));
				}
				else
				{
					list3.Add(new LoginServerSurvivorState(reportSurvivorId, 0));
				}
			}
			tWDModelManager.TdMetrics.AddProperty("Hero_TotalTraitsLevel", list3);
			tWDModelManager.TdMetrics.Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
