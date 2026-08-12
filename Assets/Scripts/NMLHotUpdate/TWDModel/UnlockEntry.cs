namespace TWDModel
{
	public struct UnlockEntry
	{
		public BlackboardEntryType EntryType;

		public string EntryKey;

		public int Target;

		public UnlockEntry(BlackboardEntryType inType, string inKey, int target = 1)
		{
			EntryType = inType;
			EntryKey = inKey;
			Target = target;
		}
	}
}
