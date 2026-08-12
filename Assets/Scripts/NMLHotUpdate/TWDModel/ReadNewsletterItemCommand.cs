using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ReadNewsletterItemCommand : ModelCommand
	{
		public List<string> ItemsReadId { get; set; }

		public List<string> OldItemsId { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel player = (manager as TWDModelManager).Player;
			foreach (string item in ItemsReadId)
			{
				player.NewsLetterItemsRead.Add(item);
			}
			foreach (string item2 in OldItemsId)
			{
				if (player.NewsLetterItemsRead.Contains(item2))
				{
					player.NewsLetterItemsRead.Remove(item2);
				}
				if (player.NewsLetterItemsInteracted.Contains(item2))
				{
					player.NewsLetterItemsInteracted.Remove(item2);
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
