namespace TWDModel
{
	public class Migration420 : TWDModelMigration
	{
		public Migration420()
		{
			base.Version = "4.2.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			player.UpdateCurrencyCapacity(CurrencyType.ShivaToken);
			for (int i = 0; i < player.SurvivorContainer.Survivors.Count; i++)
			{
				player.SurvivorContainer.Survivors[i].TraitContainer.RemoveTrait("RetaliateMultiplier");
			}
			return true;
		}
	}
}
