using BaseModel;

namespace TWDModel
{
	public class MoveCommand : ModelCommand
	{
		public GridPath Path { get; private set; }

		public MoveCommand()
		{
		}

		public MoveCommand(ActorModel actor, GridPath path)
			: base(actor)
		{
			Path = path;
		}

		public static bool PerformActions(TWDModelManager manager, ActorModel actor, GridPath path, bool globallyBlocking = false)
		{
			if (path.IsValid)
			{
				actor.IsMoving = true;
				MoveAction moveAction = new MoveAction(actor, path, consumeAP: true, globallyBlocking);
				bool num = manager.ExecuteAction(moveAction);
				if (num)
				{
					manager.ExecuteAction(new PostMoveSuccessAction(actor, moveAction));
				}
				actor.IsMoving = false;
				return num;
			}
			return false;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			if (model.TurnComplete)
			{
				manager.Debug.LogError("[Cheat Alert] MoveCommand failed. Actor already completed its turn");
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			GridPath path = GridPath.Create(Path);
			bool flag = PerformActions(manager as TWDModelManager, model, path);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}
	}
}
