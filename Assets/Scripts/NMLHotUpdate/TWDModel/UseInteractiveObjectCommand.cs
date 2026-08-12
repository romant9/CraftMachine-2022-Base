using BaseModel;

namespace TWDModel
{
	public class UseInteractiveObjectCommand : ModelCommand
	{
		public int TargetId { get; private set; }

		public UseInteractiveObjectCommand()
		{
		}

		public UseInteractiveObjectCommand(ActorModel actor, InteractiveObjectModel target)
			: base(actor)
		{
			TargetId = target.ModelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			InteractiveObjectModel model2 = manager.GetModel<InteractiveObjectModel>(TargetId);
			bool flag = false;
			if (tWDModelManager.Player.Combat.CanUseInteractiveObject(model, model2))
			{
				flag = tWDModelManager.ExecuteAction(new StartInteractiveObjectAction(model, model2)) && tWDModelManager.ExecuteAction(new UseInteractiveObjectAction(model, model2));
				if (flag)
				{
					if (!model.IsInteractingWithGuts)
					{
						model.EndAction();
					}
					else
					{
						model.IsInteractingWithGuts = false;
					}
					model.CheckForBonusActionPoint();
					(manager as TWDModelManager).ExecuteAction(new InteractiveObjectFinishedAction(model));
				}
			}
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
