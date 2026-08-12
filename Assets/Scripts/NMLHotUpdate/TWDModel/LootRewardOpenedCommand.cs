using BaseModel;

namespace TWDModel
{
	public class LootRewardOpenedCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			LootEntry model = manager.GetModel<LootEntry>(base.ModelId);
			TWDModelResult result;
			if (model == null)
			{
				result = TWDModelResult.Error;
			}
			else
			{
				model.Opened = true;
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
