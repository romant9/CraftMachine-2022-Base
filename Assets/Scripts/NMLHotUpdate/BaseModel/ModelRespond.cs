namespace BaseModel
{
	public class ModelRespond : IModelRespond
	{
		public string ModelJson { get; set; }

		public long Time { get; set; }

		public LockRespond LockState { get; set; }
	}
}
