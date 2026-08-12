using System.Collections.Generic;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public class Migration520 : TWDModelMigration
	{
		public Migration520()
		{
			base.Version = "5.2.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			MigrationUtils.DeleteCombatModel(player);
			IEnumerable<IGrouping<CurrencyType, CurrencyModel>> enumerable = from x in player.Currencies
				group x by x.Type into g
				where g.Count() > 1
				select g;
			ModelList<CurrencyModel> modelList = new ModelList<CurrencyModel>();
			if (enumerable != null && enumerable.Count() > 0)
			{
				foreach (IGrouping<CurrencyType, CurrencyModel> duplicate in enumerable)
				{
					CurrencyModel currencyModel = player.Currencies.LastOrDefault((CurrencyModel x) => x.Type == duplicate.Key);
					if (currencyModel != null)
					{
						modelList.Add(currencyModel);
					}
				}
				foreach (CurrencyModel item in modelList)
				{
					player.Currencies.Remove(item);
				}
			}
			return true;
		}
	}
}
