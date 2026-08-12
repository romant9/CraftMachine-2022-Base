namespace TWDModel
{
	public class ChangeNameLoadQueueMessage : SupportLoadQueueMessage
	{
		public string Name { get; set; }

		public ChangeNameLoadQueueMessage()
		{
		}

		public ChangeNameLoadQueueMessage(string name)
		{
			Name = name;
		}

		public override bool Execute(TWDModelManager manager)
		{
			string name = manager.Player.Name;
			manager.Player.Name = Name;
			manager.Metrics.AddChangeName(Name, string.IsNullOrEmpty(name) ? "" : name).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			return true;
		}
	}
}
