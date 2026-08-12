using BaseModel;

namespace TWDModel
{
	public class ReseedRandomCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			if (tWDModelManager != null)
			{
				tWDModelManager.Player.ReseedRandom();
				if (tWDModelManager.Player.Combat != null)
				{
					tWDModelManager.Player.Combat.ShuffleLootKeys();
				}
			}
			tWDModelManager.Metrics.AddReseedRandom().Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
