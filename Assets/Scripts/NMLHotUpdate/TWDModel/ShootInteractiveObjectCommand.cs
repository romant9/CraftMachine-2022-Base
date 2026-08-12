using BaseModel;

namespace TWDModel
{
	public class ShootInteractiveObjectCommand : ModelCommand
	{
		public int TargetId { get; set; }

		public ShootInteractiveObjectCommand()
		{
		}

		public ShootInteractiveObjectCommand(ActorModel actor, InteractiveObjectModel target)
			: base(actor)
		{
			TargetId = target.ModelId;
		}

		public static bool PerformActions(TWDModelManager manager, ActorModel actor, InteractiveObjectModel target)
		{
			bool num = manager.ExecuteAction(new ShootInteractiveObjectAction(actor, target));
			if (num)
			{
				actor.EndAction();
			}
			return num;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			InteractiveObjectModel model2 = manager.GetModel<InteractiveObjectModel>(TargetId);
			bool flag = PerformActions(manager as TWDModelManager, model, model2);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
