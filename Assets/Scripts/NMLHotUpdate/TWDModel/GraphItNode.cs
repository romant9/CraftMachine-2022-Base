using System;

namespace TWDModel
{
	public class GraphItNode : Attribute
	{
		public NodeType NodeType { get; set; }

		public GraphItNode(NodeType nodeType)
		{
			NodeType = nodeType;
		}
	}
}
