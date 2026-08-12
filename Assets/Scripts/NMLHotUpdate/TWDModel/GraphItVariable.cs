using System;

namespace TWDModel
{
	public class GraphItVariable : Attribute
	{
		public string Comment { get; set; }

		public GraphItVariable(string comment = "")
		{
			Comment = comment;
		}
	}
}
