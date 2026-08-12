using BaseModel;

namespace TWDModel
{
	public class TWDValidationModelCommand : ModelCommand
	{
		protected virtual TWDValidationCommandResult Validate(ModelManager manager)
		{
			return TWDValidationCommandResult.OK;
		}

		protected virtual IModelCommandRespond ExecuteInternal(ModelManager modelManager)
		{
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}

		public override IModelCommandRespond Execute(ModelManager modelManager)
		{
			return Validate(modelManager) switch
			{
				TWDValidationCommandResult.OK => ExecuteInternal(modelManager), 
				TWDValidationCommandResult.Canceled => new NGModelCommandRespond(this, TWDModelResult.Skip), 
				_ => new NGModelCommandRespond(this, TWDModelResult.Error), 
			};
		}
	}
}
