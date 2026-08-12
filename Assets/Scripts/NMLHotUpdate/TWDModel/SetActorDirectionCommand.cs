using BaseModel;

namespace TWDModel
{
	public class SetActorDirectionCommand : ModelCommand
	{
		public FixedVec3 ForwardDir { get; set; }

		public SetActorDirectionCommand()
		{
		}

		public SetActorDirectionCommand(ActorModel actor, FixedVec3 forward)
			: base(actor)
		{
			ForwardDir = forward;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			if (model != null)
			{
				model.ForwardDirection = ForwardDir;
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
