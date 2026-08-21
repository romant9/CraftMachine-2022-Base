namespace BaseModel
{
	public class ChangeNameLoadQueueMessage : LoadQueueMessage
	{
		public string Name { get; set; }

		public long SupportGivenTimestamp { get; set; }

		public string SupportEntityGUID { get; set; }
	}
}
