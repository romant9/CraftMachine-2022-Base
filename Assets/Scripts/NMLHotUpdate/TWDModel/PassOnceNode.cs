using System;

namespace TWDModel
{
	[Serializable]
	public class PassOnceNode : NodeBase
	{
		public bool HasFired;

		public PassOnceNode()
		{
		}

		public PassOnceNode(PassOnceNode node)
			: base(node)
		{
			HasFired = node.HasFired;
		}

		public override NodeBase RecordValue()
		{
			return new PassOnceNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			HasFired = false;
		}

		[GraphItInput("In", "")]
		public void In()
		{
			Out();
		}

		[GraphItOutput("Out", "")]
		public void Out()
		{
			if (!HasFired)
			{
				HasFired = true;
				Fire("Out");
			}
		}
	}
}
