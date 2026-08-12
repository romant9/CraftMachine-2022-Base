using System;

namespace TWDModel
{
	public class GraphItImportData : Attribute
	{
		public string Comment { get; set; }

		public string Id { get; set; }

		public GraphItImportData(string id, string comment = "")
		{
			Id = id;
			Comment = comment;
		}
	}
}
