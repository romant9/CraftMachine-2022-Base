using BaseModel;

namespace TWDModel
{
	public class GiveOutpostSurvivorsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if ((manager as TWDModelManager).Player.GiveExtraOutpostSurvivorsAndSlots())
			{
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
