using BaseModel;

namespace TWDModel
{
	public class NGModelCommandRespond : ModelCommandRespond
	{
		public NGModelCommandRespond()
		{
		}

		public NGModelCommandRespond(ModelCommand command, TWDModelResult result)
		{
			base.SequenceId = command.SequenceId;
			base.Code = (int)((result != TWDModelResult.OK) ? result : TWDModelResult.OK);
			if (base.Code != 0)
			{
				base.Message = result.ToString();
			}
		}

		public TWDModelResult GetModelResult()
		{
			return (TWDModelResult)base.Code;
		}
	}
}
