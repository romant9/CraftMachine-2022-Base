using System;

namespace TWDModel
{
	[Serializable]
	public class GEDSheet : Attribute
	{
		public string SheetName;

		public GEDSheet(string name)
		{
			SheetName = name;
		}
	}
}
