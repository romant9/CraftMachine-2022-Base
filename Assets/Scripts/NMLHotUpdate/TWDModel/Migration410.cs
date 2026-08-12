using System.Linq;

namespace TWDModel
{
	public class Migration410 : TWDModelMigration
	{
		public Migration410()
		{
			base.Version = "4.1.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			CurrencyModel currencyModel = player.Currencies.FirstOrDefault((CurrencyModel x) => x.Type == CurrencyType.EndlessPassToken);
			CurrencyModel currencyModel2 = player.Currencies.FirstOrDefault((CurrencyModel x) => x.Type == CurrencyType.ShivaToken);
			CurrencyModel currencyModel3 = player.Currencies.FirstOrDefault((CurrencyModel x) => x.Type == CurrencyType.DogToken);
			CurrencyModel currencyModel4 = player.Currencies.FirstOrDefault((CurrencyModel x) => x.Type == CurrencyType.WhisperersMaskToken);
			if (currencyModel == null || currencyModel2 == null || currencyModel3 == null || currencyModel4 == null)
			{
				manager.Debug.LogError($"4.1.0 Migration Error: Can not find currency model {currencyModel == null} {currencyModel2 == null} {currencyModel3 == null} {currencyModel4 == null}");
				return false;
			}
			manager.Player.Currencies.Remove(currencyModel);
			manager.Player.Currencies.Remove(currencyModel3);
			manager.Player.Currencies.Remove(currencyModel2);
			manager.Player.Currencies.Remove(currencyModel4);
			manager.Player.Currencies.Add(currencyModel);
			manager.Player.Currencies.Add(currencyModel2);
			manager.Player.Currencies.Add(currencyModel3);
			manager.Player.Currencies.Add(currencyModel4);
			currencyModel.SetValue(5);
			currencyModel2.SetValue(0);
			currencyModel3.SetValue(0);
			currencyModel4.SetValue(0);
			return true;
		}
	}
}
