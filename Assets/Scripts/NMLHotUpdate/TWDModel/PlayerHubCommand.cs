using BaseModel;

namespace TWDModel
{
	public class PlayerHubCommand : ModelCommand
	{
		public string EventName { get; set; }

		public string ItemId { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager)
			{
				tWDModelManager.Metrics.AddStart().AddPlayerHub(EventName);
				if (ItemId != null)
				{
					tWDModelManager.Metrics.AddClick(ItemId);
				}
				tWDModelManager.Metrics.Send();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
