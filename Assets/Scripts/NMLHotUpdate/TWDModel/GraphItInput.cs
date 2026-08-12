using System;

namespace TWDModel
{
	public class GraphItInput : Attribute
	{
		public string Comment { get; set; }

		public string Id { get; set; }

		public GraphItInput(string id, string comment = "")
		{
			Id = id;
			Comment = comment;
		}
	}
}
