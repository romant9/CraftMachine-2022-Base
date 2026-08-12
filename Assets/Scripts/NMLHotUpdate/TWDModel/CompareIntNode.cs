using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class CompareIntNode : NodeBase
	{
		[GraphItVariable("")]
		public ConditionOperator Operator;

		[GraphItVariable("")]
		public int ConstValueB;

		[JsonIgnore]
		[GraphItImportData("Value A", "")]
		public int ValueA
		{
			get
			{
				object obj = Import("Value A");
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
		}

		[JsonIgnore]
		[GraphItImportData("Value B", "")]
		public int ValueB
		{
			get
			{
				object obj = Import("Value B");
				if (obj != null)
				{
					return (int)obj;
				}
				return ConstValueB;
			}
		}

		public CompareIntNode()
		{
		}

		public CompareIntNode(CompareIntNode node)
			: base(node)
		{
			ConstValueB = node.ConstValueB;
			Operator = node.Operator;
		}

		public override NodeBase RecordValue()
		{
			return new CompareIntNode(this);
		}

		[GraphItInput("Compare", "")]
		public void Compare()
		{
			if (IntCondition.Compare(Operator, ValueA, ValueB))
			{
				FireTrue();
			}
			else
			{
				FireFalse();
			}
		}

		[GraphItOutput("True", "")]
		public void FireTrue()
		{
			Fire("True");
		}

		[GraphItOutput("False", "")]
		public void FireFalse()
		{
			Fire("False");
		}
	}
}
