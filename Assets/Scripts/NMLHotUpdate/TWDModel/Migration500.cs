namespace TWDModel
{
	public class Migration500 : TWDModelMigration
	{
		public Migration500()
		{
			base.Version = "5.0.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.BuildingTokenBP, CurrencyType.SuperBuildingTokenBP, CurrencyType.TrainingTokenBP, CurrencyType.SuperTrainingTokenBP, CurrencyType.EquipmentTokenBP, CurrencyType.SuperEquipmentTokenBP, CurrencyType.HealingTokenBP, CurrencyType.BattlePassPoints, CurrencyType.FreeGuildGiftPerk, CurrencyType.BattlePassPremium);
			return true;
		}
	}
}
