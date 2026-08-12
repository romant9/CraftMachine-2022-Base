namespace TWDModel
{
	public class SupportRemoveSupportItemEntry
	{
		public string Identifier { get; set; }

		public bool RemoveItem { get; set; }

		public SupportRemoveSupportItemEntry(string identifier, bool removeItem)
		{
			Identifier = identifier;
			RemoveItem = removeItem;
		}
	}
}
