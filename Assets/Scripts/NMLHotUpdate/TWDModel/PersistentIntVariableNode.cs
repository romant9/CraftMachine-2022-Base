using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class PersistentIntVariableNode : NodeBase
	{
		[GraphItVariable("")]
		public string VariableName;

		[GraphItVariable("")]
		public int FailSafeValue;

		[GraphItVariable("")]
		public int MinValue;

		[JsonIgnore]
		[GraphItExportData("Read", "")]
		public int Read => GetValue();

		[JsonIgnore]
		[GraphItImportData("Write Value", "")]
		public int WriteValue
		{
			get
			{
				object obj = Import("Write Value");
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
		}

		public PersistentIntVariableNode()
		{
		}

		public PersistentIntVariableNode(PersistentIntVariableNode node)
			: base(node)
		{
			VariableName = node.VariableName;
			FailSafeValue = node.FailSafeValue;
			MinValue = node.MinValue;
		}

		public override NodeBase RecordValue()
		{
			return new PersistentIntVariableNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		[GraphItOutput("Changed", "")]
		public void ValueChanged()
		{
			Fire("Changed");
		}

		[GraphItOutput("ChangeToMin", "")]
		public void ChangedToMin()
		{
			Fire("ChangeToMin");
		}

		[GraphItInput("Write Execute", "")]
		public void WriteExecute()
		{
			int num = WriteValue;
			if (num < MinValue)
			{
				num = MinValue;
			}
			if (num != GetValue())
			{
				SetValue(num);
				ValueChanged();
				if (num == MinValue)
				{
					ChangedToMin();
				}
			}
		}

		[GraphItInput("Increase", "")]
		public void Increase()
		{
			int value = GetValue() + 1;
			SetValue(value);
			ValueChanged();
		}

		[GraphItInput("Decrease", "")]
		public void Decrease()
		{
			int num = GetValue() - 1;
			if (num < MinValue)
			{
				num = MinValue;
			}
			if (num != GetValue())
			{
				SetValue(num);
				ValueChanged();
				if (num == MinValue)
				{
					ChangedToMin();
				}
			}
		}

		private int GetValue()
		{
			if (base.manager.Player.Combat.PersistentMissionVariableManager == null)
			{
				return FailSafeValue;
			}
			return base.manager.Player.Combat.PersistentMissionVariableManager.GetIntVariable(VariableName, FailSafeValue);
		}

		private void SetValue(int value)
		{
			if (base.manager.Player.Combat.PersistentMissionVariableManager != null && base.manager.Player.Combat.PersistentMissionVariableManager.DoesVariableExist(VariableName))
			{
				base.manager.Player.Combat.PersistentMissionVariableManager.SetIntVariableCreatingIfNecessary(VariableName, value, readOnly: false);
			}
		}
	}
}
