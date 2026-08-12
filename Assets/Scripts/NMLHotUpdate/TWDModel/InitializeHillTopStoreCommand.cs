using BaseModel;

namespace TWDModel
{
	public class InitializeHillTopStoreCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Player.HillTopStore.Init();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
