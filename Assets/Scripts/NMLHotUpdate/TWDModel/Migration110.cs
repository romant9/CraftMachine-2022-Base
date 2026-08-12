namespace TWDModel
{
	public class Migration110 : TWDModelMigration
	{
		public Migration110()
		{
			base.Version = "1.1.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			FixEquipmentAndSurvivorTimedAction(player);
			return true;
		}

		private bool FixEquipmentAndSurvivorTimedAction(PlayerModel player)
		{
			bool result = false;
			if (player.Camp != null)
			{
				if (player.Camp.GetBuilding("Workshop") is WorkshopBuildingModel { UpgradingModel: not null } workshopBuildingModel)
				{
					EquipmentItemModel equipmentItemModel = workshopBuildingModel.UpgradingModel as EquipmentItemModel;
					if (equipmentItemModel.TimedActionModel != null && !equipmentItemModel.TimedActionModel.IsActionUnderway() && workshopBuildingModel.UpgradedUnseenModel == null)
					{
						workshopBuildingModel.ResetUpgradingModel();
						result = true;
					}
				}
				if (player.Camp.GetBuilding("TrainingGround") is TrainingGroundBuildingModel { UpgradingModel: not null } trainingGroundBuildingModel)
				{
					SurvivorModel survivorModel = trainingGroundBuildingModel.UpgradingModel as SurvivorModel;
					if (survivorModel.TimedActionModel != null && !survivorModel.TimedActionModel.IsActionUnderway() && trainingGroundBuildingModel.UpgradedUnseenModel == null)
					{
						trainingGroundBuildingModel.ResetUpgradingModel();
						result = true;
					}
				}
			}
			return result;
		}
	}
}
