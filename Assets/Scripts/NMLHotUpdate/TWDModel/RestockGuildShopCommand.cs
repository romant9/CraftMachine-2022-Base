using BaseModel;

namespace TWDModel
{
	public class RestockGuildShopCommand : ModelCommand
	{
		public bool OnNewTier { get; private set; }

		public bool OnNewWar { get; private set; }

		public RestockGuildShopCommand()
		{
		}

		public RestockGuildShopCommand(bool onNewTier, bool onNewWar)
		{
			OnNewTier = onNewTier;
			OnNewWar = onNewWar;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.OK;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (OnNewTier)
			{
				tWDModelManager.Player.GuildShopModel.UpdateGuildShopItemsOnNewTier();
			}
			if (OnNewWar)
			{
				tWDModelManager.Player.GuildShopModel.RestockGuildShopItems(OnNewTier, OnNewWar);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
