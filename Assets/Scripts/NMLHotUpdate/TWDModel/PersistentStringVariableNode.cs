using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class PersistentStringVariableNode : NodeBase
	{
		[GraphItVariable("")]
		public string VariableName;

		[GraphItVariable("")]
		public string FailSafeValue;

		[JsonIgnore]
		[GraphItExportData("Read", "")]
		public string Read => GetValue();

		[JsonIgnore]
		[GraphItImportData("Write Value", "")]
		public string WriteValue
		{
			get
			{
				object obj = Import("Write Value");
				if (obj != null)
				{
					return (string)obj;
				}
				return "";
			}
		}

		public PersistentStringVariableNode()
		{
		}

		public PersistentStringVariableNode(PersistentStringVariableNode node)
			: base(node)
		{
			VariableName = node.VariableName;
			FailSafeValue = node.FailSafeValue;
		}

		public override NodeBase RecordValue()
		{
			return new PersistentStringVariableNode(this);
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

		[GraphItInput("Write Execute", "")]
		public void WriteExecute()
		{
			string writeValue = WriteValue;
			if (writeValue != GetValue())
			{
				SetValue(writeValue);
				ValueChanged();
			}
		}

		private string GetValue()
		{
			if (base.manager.Player.Combat.PersistentMissionVariableManager == null)
			{
				return FailSafeValue;
			}
			return base.manager.Player.Combat.PersistentMissionVariableManager.GetStringVariable(VariableName, FailSafeValue);
		}

		private void SetValue(string value)
		{
			if (base.manager.Player.Combat.PersistentMissionVariableManager != null && base.manager.Player.Combat.PersistentMissionVariableManager.DoesVariableExist(VariableName))
			{
				base.manager.Player.Combat.PersistentMissionVariableManager.SetStringVariableCreatingIfNecessary(VariableName, value, readOnly: false);
			}
		}
	}
}
