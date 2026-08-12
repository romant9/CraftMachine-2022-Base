using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class MarkGuildShopItemsSeenCommand : ModelCommand
	{
		public List<int> ItemIds { get; private set; }

		public MarkGuildShopItemsSeenCommand()
		{
		}

		public MarkGuildShopItemsSeenCommand(List<int> itemIds)
		{
			ItemIds = itemIds;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.OK;
			(manager as TWDModelManager).Player.GuildShopModel.MarkItemsAsSeen(ItemIds);
			return new NGModelCommandRespond(this, result);
		}
	}
}
