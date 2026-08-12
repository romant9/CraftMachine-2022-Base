using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class AddUnhandledPurchaseCommand : ModelCommand
	{
		public StorePurchaseInfo storePurchaseInfo { get; set; }

		public AddUnhandledPurchaseCommand()
		{
		}

		public AddUnhandledPurchaseCommand(StorePurchaseInfo storePurchaseInfo)
		{
			this.storePurchaseInfo = storePurchaseInfo;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager obj = (TWDModelManager)manager;
			PlayerModel player = obj.Player;
			obj.Debug.LogWarning("Adding unhandled purchase: productId: " + storePurchaseInfo.Product.ProductIdentifier);
			if (player.UnhandledPurchases == null)
			{
				player.UnhandledPurchases = new List<StorePurchaseInfo>();
			}
			player.UnhandledPurchases.Add(storePurchaseInfo);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
