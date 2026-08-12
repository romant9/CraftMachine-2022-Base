namespace BaseModel
{
	public interface IModelCommandRespond
	{
		int SequenceId { get; }

		int Code { get; set; }

		string Message { get; }
	}
}
