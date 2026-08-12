using System;

namespace TWDModel
{
	public class GraphItExportData : Attribute
	{
		public string Comment { get; set; }

		public string Id { get; set; }

		public GraphItExportData(string id, string comment = "")
		{
			Id = id;
			Comment = comment;
		}
	}
}
