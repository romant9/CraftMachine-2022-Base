using BaseModel;

namespace TWDModel
{
	public class TickModelCommand : ModelCommand
	{
		public bool Save { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (Save)
			{
				tWDModelManager.Save(SaveType.Player);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
