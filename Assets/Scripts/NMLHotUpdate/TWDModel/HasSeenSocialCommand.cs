using BaseModel;

namespace TWDModel
{
	public class HasSeenSocialCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager.GetPlayer() as PlayerModel).HasSeenSocial = true;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
