using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class UpdateGvgDefendersCommand : TWDSocialModelCommand
	{
		private GuildBattleParticipantInfo playerInfo;

		public List<string> GvgDefendersIds;

		public UpdateGvgDefendersCommand()
		{
		}

		public UpdateGvgDefendersCommand(List<string> gvgDefendersIds)
		{
			GvgDefendersIds = gvgDefendersIds;
		}

		protected override TWDModelResult ValidateCommand(TWDModelManager manager)
		{
			if (!manager.Player.IsGuildMember)
			{
				manager.Debug.LogError("UpdateGvgDefendersCommand: Player is not a Guild Member");
				return TWDModelResult.Error;
			}
			if (GvgDefendersIds.Count != 9)
			{
				manager.Debug.LogError("UpdateGvgDefendersCommand: Defenders count is not 9");
				return TWDModelResult.Error;
			}
			if (manager.Player.UtcTimestampLastGvgDefendersUpdate + manager.GameEconomyData.ConfigData.GvGDefendersCooldown < manager.Player.UtcTimeStamp)
			{
				manager.Debug.LogError("UpdateGvgDefendersCommand: Defenders are on cooldown");
				return TWDModelResult.Error;
			}
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			if (!(modelManager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			List<SurvivorMockData> list = new List<SurvivorMockData>();
			int i;
			for (i = 0; i < GvgDefendersIds.Count; i++)
			{
				SurvivorModel survivorModel = tWDModelManager.Player.SurvivorContainer.Survivors.First((SurvivorModel x) => x.IdForAnalytics == GvgDefendersIds[i]);
				if (survivorModel == null)
				{
					modelManager.Debug.LogError("UpdateGvgDefendersCommand: Cant find defender " + GvgDefendersIds[i]);
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				SurvivorMockData survivorMockData = survivorModel.CreateMockData();
				survivorMockData.AdjustedLevel = (int)GvGModelHelper.GetAdjustedLevelForSurvivor(survivorModel, tWDModelManager.GameEconomyData);
				survivorMockData.TotalDamage = survivorModel.GetHitpoints();
				survivorMockData.OwnerHashedPlayerId = tWDModelManager.Player.HashedId;
				survivorMockData.MockWeapon = survivorModel.GetWeaponEquipment().CreateMockData();
				survivorMockData.MockArmor = survivorModel.GetEquipmentOfCategory(EquipmentCategory.Armor).CreateMockData();
				list.Add(survivorMockData);
			}
			tWDModelManager.Player.UtcTimestampLastGvgDefendersUpdate = tWDModelManager.Player.UtcTimeStamp;
			tWDModelManager.Player.GvGDefenders = list;
			tWDModelManager.Metrics.GvgDefendersUpdated().Send();
			playerInfo = GvGModelHelper.CreateEnemyPlayerData(tWDModelManager.Player, tWDModelManager.GameEconomyData);
			return base.Execute(modelManager) as NGModelCommandRespond;
		}

		protected override GroupCommandBase CreateGroupCommand(TWDModelManager modelManager)
		{
			return new UpdateGvgDefendersGroupCommand(playerInfo);
		}
	}
}
