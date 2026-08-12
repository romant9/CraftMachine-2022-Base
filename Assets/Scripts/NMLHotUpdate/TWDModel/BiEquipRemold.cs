namespace TWDModel
{
	public class BiEquipRemold
	{
		public string Id;

		public int Level;

		public string Type;

		public BiEquipRemold(string id, int level)
		{
			Id = id;
			Level = level;
		}

		public BiEquipRemold(string id, int level, string type)
		{
			Id = id;
			Level = level;
			Type = type;
		}
	}
}
