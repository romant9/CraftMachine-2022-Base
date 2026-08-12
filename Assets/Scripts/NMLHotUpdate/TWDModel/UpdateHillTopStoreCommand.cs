using BaseModel;

namespace TWDModel
{
	public class UpdateHillTopStoreCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Player.HillTopStore.UpdateSlots();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
