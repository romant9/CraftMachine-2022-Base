using BaseModel;

namespace TWDModel
{
	public class MarkObjectViewedCommand : ModelCommand
	{
		public MarkObjectViewedCommand()
		{
		}

		public MarkObjectViewedCommand(TWDModelObject obj)
			: base(obj)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager.GetModel(base.ModelId) is IUserViewableObject userViewableObject))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			userViewableObject.OnObjectViewedByUser();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
