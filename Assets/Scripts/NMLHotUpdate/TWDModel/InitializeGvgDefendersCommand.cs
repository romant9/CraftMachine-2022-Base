using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class InitializeGvgDefendersCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = (TWDModelManager)manager;
			if (InitializeGvGDefenders(tWDModelManager.Player, tWDModelManager.GameEconomyData))
			{
				tWDModelManager.Metrics.GvgDefendersUpdated(auto: true).Send();
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}

		private bool InitializeGvGDefenders(PlayerModel playerModel, GameEconomyData gameEconomyData)
		{
			if (playerModel.GvGDefenders != null && playerModel.GvGDefenders.Count == 9 && playerModel.GvGDefenders.Count((SurvivorMockData x) => string.IsNullOrEmpty(x.AnalyticsId) || x.AnalyticsId == "0") == 0)
			{
				playerModel.Debug.LogError("Trying to initialize defenders when is already initialized");
				return false;
			}
			playerModel.GvGDefenders = new List<SurvivorMockData>();
			int count = playerModel.SurvivorContainer.Survivors.Count;
			int num = playerModel.SurvivorContainer.Survivors.Max((SurvivorModel x) => x.Level);
			for (int num2 = count; num2 < 9; num2++)
			{
				SurvivorModel survivor = playerModel.SurvivorContainer.CreateRandomSurvivor(0, num, num);
				if (!playerModel.SurvivorContainer.CanAddSurvivor())
				{
					playerModel.SurvivorContainer.SurvivorGiftSlotsCount++;
				}
				playerModel.SurvivorContainer.AddSurvivor(survivor);
			}
			if (playerModel.SurvivorContainer.Survivors.Count < 9)
			{
				playerModel.Debug.LogError("Trying to initialize Gvg defenders with less than 9 survivors in the roster");
				return false;
			}
			List<Tuple<int, FixedPoint, SurvivorModel>> list = GvGModelHelper.CalculateAndSortPlayerAdjustedLevelForSurvivors(playerModel, gameEconomyData);
			for (int num3 = 0; num3 < list.Count && num3 < 9; num3++)
			{
				if (!AddSurvivorToDefenders(playerModel, gameEconomyData, list[num3].Third))
				{
					playerModel.GvGDefenders = null;
					return false;
				}
			}
			return true;
		}

		private bool AddSurvivorToDefenders(PlayerModel playerModel, GameEconomyData gameEconomyData, SurvivorModel survivorModel)
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
			playerModel.GvGDefenders.Add(survivorMockData);
			return true;
		}
	}
}
