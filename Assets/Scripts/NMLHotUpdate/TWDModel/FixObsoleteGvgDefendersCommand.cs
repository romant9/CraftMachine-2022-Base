using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class FixObsoleteGvgDefendersCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (UpdateGvgDefenders(tWDModelManager.Player, tWDModelManager.GameEconomyData))
			{
				tWDModelManager.Metrics.GvgDefendersUpdated(auto: true).Send();
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}

		private bool UpdateGvgDefenders(PlayerModel playerModel, GameEconomyData gameEconomyData)
		{
			if (playerModel.GvGDefenders == null)
			{
				playerModel.Debug.LogError("Trying to update defenders when defenders are not initialized");
				return false;
			}
			for (int i = 0; i < playerModel.GvGDefenders.Count; i++)
			{
				string defenderAnalyticId = playerModel.GvGDefenders[i].AnalyticsId;
				if (playerModel.SurvivorContainer.Survivors.Any((SurvivorModel x) => x.IdForAnalytics == defenderAnalyticId))
				{
					continue;
				}
				List<string> defenderIds = playerModel.GvGDefenders.Select((SurvivorMockData x) => x.AnalyticsId).ToList();
				SurvivorModel survivorModel = playerModel.SurvivorContainer.Survivors.FirstOrDefault((SurvivorModel x) => !defenderIds.Contains(x.IdForAnalytics));
				if (survivorModel == null)
				{
					int num = playerModel.SurvivorContainer.Survivors.Max((SurvivorModel x) => x.Level);
					survivorModel = playerModel.SurvivorContainer.CreateRandomSurvivor(0, num, num);
					if (!playerModel.SurvivorContainer.CanAddSurvivor())
					{
						playerModel.SurvivorContainer.SurvivorGiftSlotsCount++;
					}
					playerModel.SurvivorContainer.AddSurvivor(survivorModel);
				}
				if (!ReplaceSurvivorFromDefenders(playerModel, gameEconomyData, survivorModel, defenderAnalyticId))
				{
					playerModel.Debug.LogError("Error adding survivor to defenders");
					return false;
				}
			}
			return true;
		}

		private bool ReplaceSurvivorFromDefenders(PlayerModel playerModel, GameEconomyData gameEconomyData, SurvivorModel survivorModel, string analyticsIdToReplace)
		{
			if (playerModel.GvGDefenders.Any((SurvivorMockData x) => x.AnalyticsId == survivorModel.IdForAnalytics))
			{
				playerModel.Debug.LogError("Trying to add a duplicated survivor to defenders");
				return false;
			}
			if (playerModel.GvGDefenders.Count > 9)
			{
				playerModel.Debug.LogError("Trying to add a 10th survivor to defenders");
				return false;
			}
			SurvivorMockData survivorMockData = survivorModel.CreateMockData();
			survivorMockData.AdjustedLevel = (int)GvGModelHelper.GetAdjustedLevelForSurvivor(survivorModel, gameEconomyData);
			survivorMockData.TotalDamage = survivorModel.GetHitpoints();
			survivorMockData.OwnerHashedPlayerId = playerModel.HashedId;
			survivorMockData.MockWeapon = survivorModel.GetWeaponEquipment().CreateMockData();
			survivorMockData.MockArmor = survivorModel.GetEquipmentOfCategory(EquipmentCategory.Armor).CreateMockData();
			for (int num = 0; num < playerModel.GvGDefenders.Count; num++)
			{
				if (playerModel.GvGDefenders[num].AnalyticsId == analyticsIdToReplace)
				{
					playerModel.GvGDefenders[num] = survivorMockData;
					return true;
				}
			}
			return false;
		}
	}
}
