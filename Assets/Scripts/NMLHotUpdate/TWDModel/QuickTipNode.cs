using System;

namespace TWDModel
{
	[Serializable]
	public class QuickTipNode : NodeBase
	{
		[GraphItVariable("Tip ID")]
		public string TipID;

		public QuickTipNode()
		{
		}

		public QuickTipNode(QuickTipNode node)
			: base(node)
		{
			TipID = node.TipID;
		}

		public override NodeBase RecordValue()
		{
			return new QuickTipNode(this);
		}

		[GraphItInput("Show", "")]
		public void Show()
		{
			NotifyChange("Show", TipID);
			Out();
		}

		[GraphItOutput("Out", "")]
		public void Out()
		{
			Fire("Out");
		}
	}
}
