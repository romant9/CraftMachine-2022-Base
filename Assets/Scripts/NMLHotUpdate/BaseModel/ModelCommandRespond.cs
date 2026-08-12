namespace BaseModel
{
	public class ModelCommandRespond : IModelCommandRespond
	{
		public int SequenceId { get; set; }

		public int Code { get; set; }

		public string Message { get; set; }

		public ModelCommandRespond()
		{
		}

		public ModelCommandRespond(int sequenceId)
		{
			SequenceId = sequenceId;
		}

		public ModelCommandRespond(int sequenceId, int code)
		{
			SequenceId = sequenceId;
			Code = code;
		}

		public ModelCommandRespond(int sequenceId, int code, string message)
		{
			SequenceId = sequenceId;
			Code = code;
			Message = message;
		}
	}
}
