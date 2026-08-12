using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ModifyIntVariableNode : NodeBase
	{
		[GraphItVariable("")]
		public IntVariableOperation IntVariableOperation;

		[GraphItVariable("")]
		public int ConstValue;

		[JsonIgnore]
		[GraphItImportData("Target Int", "")]
		public int TargetValue
		{
			get
			{
				object obj = Import("Target Int");
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
			set
			{
				Export("Target Int", value);
			}
		}

		public ModifyIntVariableNode()
		{
		}

		public ModifyIntVariableNode(ModifyIntVariableNode node)
			: base(node)
		{
			IntVariableOperation = node.IntVariableOperation;
			ConstValue = node.ConstValue;
		}

		public override NodeBase RecordValue()
		{
			return new ModifyIntVariableNode(this);
		}

		[GraphItInput("Modify", "")]
		public void Modify()
		{
			if (IntVariableOperation == IntVariableOperation.Set)
			{
				TargetValue = ConstValue;
			}
			else if (IntVariableOperation == IntVariableOperation.Add)
			{
				TargetValue += ConstValue;
			}
			else if (IntVariableOperation == IntVariableOperation.Sub)
			{
				TargetValue -= ConstValue;
			}
		}
	}
}
