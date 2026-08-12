using System;

namespace TWDModel
{
	public class GraphItOutput : Attribute
	{
		public string Comment { get; set; }

		public string Id { get; set; }

		public GraphItOutput(string id, string comment = "")
		{
			Id = id;
			Comment = comment;
		}
	}
}
