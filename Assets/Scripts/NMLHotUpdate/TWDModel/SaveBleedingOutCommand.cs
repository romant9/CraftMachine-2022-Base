using BaseModel;

namespace TWDModel
{
	public class SaveBleedingOutCommand : ModelCommand
	{
		public int TargetActorId { get; private set; }

		public SaveBleedingOutCommand()
		{
		}

		public SaveBleedingOutCommand(ActorModel actor, ActorModel target)
			: base(actor)
		{
			TargetActorId = target.ModelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			ActorModel model2 = manager.GetModel<ActorModel>(TargetActorId);
			if (model != null && model.IsValid() && model2 != null && model2.IsValid() && model2.IsBleedingOut)
			{
				model2.FinishTimedEffect(interrupted: true);
				model2.NotifyBleedingOutFinished();
				model.EndAction();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
