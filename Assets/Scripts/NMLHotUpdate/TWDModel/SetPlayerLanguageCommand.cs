using BaseModel;

namespace TWDModel
{
	public class SetPlayerLanguageCommand : ModelCommand
	{
		public string Language;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager.GetPlayer() as PlayerModel).Language = Language;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
