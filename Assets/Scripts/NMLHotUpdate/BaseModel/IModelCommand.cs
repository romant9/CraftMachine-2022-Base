namespace BaseModel
{
	public interface IModelCommand
	{
		int SequenceId { get; }

		long Time { get; }

		int ModelId { get; }

		IModelCommandRespond Execute(ModelManager manager);
	}
}
