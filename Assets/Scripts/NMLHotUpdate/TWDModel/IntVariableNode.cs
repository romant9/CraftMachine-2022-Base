using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class IntVariableNode : NodeBase
	{
		[GraphItVariable("")]
		public int InitialValue;

		public int currentValue { get; set; }

		[JsonIgnore]
		[GraphItExportData("Current Value", "")]
		public int CurrentValue
		{
			get
			{
				return currentValue;
			}
			set
			{
				if (value != currentValue)
				{
					currentValue = value;
					ValueChanged();
				}
			}
		}

		public IntVariableNode()
		{
		}

		public IntVariableNode(IntVariableNode node)
			: base(node)
		{
			InitialValue = node.InitialValue;
			currentValue = node.currentValue;
		}

		public override NodeBase RecordValue()
		{
			return new IntVariableNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			currentValue = InitialValue;
		}

		[GraphItOutput("Value Changed", "")]
		public void ValueChanged()
		{
			Fire("Value Changed");
		}
	}
}
