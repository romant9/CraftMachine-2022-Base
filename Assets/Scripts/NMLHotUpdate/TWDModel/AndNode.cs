using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class AndNode : NodeBase
	{
		[GraphItVariable("")]
		public bool CheckForTrue = true;

		[GraphItExportData("State", "Current state of the boolean conditions.")]
		public bool State => CheckForTrue == Value;

		[JsonIgnore]
		[GraphItImportData("Values", "")]
		public bool Value
		{
			get
			{
				List<object> list = ImportValues("Values");
				if (list != null)
				{
					bool flag = true;
					for (int i = 0; i < list.Count; i++)
					{
						bool flag2 = (bool)list[i];
						flag = flag && flag2;
					}
					return flag;
				}
				return false;
			}
		}

		public AndNode()
		{
		}

		public AndNode(AndNode node)
			: base(node)
		{
			CheckForTrue = node.CheckForTrue;
		}

		public override NodeBase RecordValue()
		{
			return new AndNode(this);
		}

		[GraphItInput("Compare", "")]
		public void Compare()
		{
			if (Value == CheckForTrue)
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
