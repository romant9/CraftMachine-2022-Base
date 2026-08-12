using BaseModel;

namespace TWDModel
{
	public class SearchSurvivorCommand : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
