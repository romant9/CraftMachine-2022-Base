using BaseModel;

namespace TWDModel
{
	public abstract class TWDSocialModelCommand : ModelCommand
	{
		public TWDSocialModelCommand()
		{
		}

		protected abstract GroupCommandBase CreateGroupCommand(TWDModelManager modelManager);

		protected virtual TWDModelResult ValidateCommand(TWDModelManager modelManager)
		{
			return TWDModelResult.OK;
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			TWDModelManager tWDModelManager = modelManager as TWDModelManager;
			TWDModelResult tWDModelResult = ValidateCommand(tWDModelManager);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			GroupCommandBase command = CreateGroupCommand(tWDModelManager);
			HelpersModel.ExecuteGroupCommand(tWDModelManager, command);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
