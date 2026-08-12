namespace BaseModel
{
	public interface IModelRespond
	{
		string ModelJson { get; set; }

		long Time { get; set; }

		LockRespond LockState { get; set; }
	}
}
