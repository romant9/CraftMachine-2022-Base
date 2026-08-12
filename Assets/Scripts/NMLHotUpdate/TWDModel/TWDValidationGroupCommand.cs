using BaseModel;

namespace TWDModel
{
	public class TWDValidationGroupCommand : TWDGroupCommand
	{
		protected virtual TWDValidationCommandResult Validate(ModelManager manager)
		{
			return TWDValidationCommandResult.OK;
		}

		protected virtual bool ExecuteInternal(ModelManager modelManager)
		{
			return false;
		}

		public override GroupCommandBase Execute(ModelManager modelManager)
		{
			if (Validate(modelManager) == TWDValidationCommandResult.OK && ExecuteInternal(modelManager))
			{
				SaveGroupModel(modelManager);
			}
			return this;
		}
	}
}
