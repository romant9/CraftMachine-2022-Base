using System.Collections.Generic;

namespace TWDModel
{
	public class PersistentMissionVariableManager : TWDModelObject
	{
		public List<PersistentMissionVariable> variables { get; set; }

		public PersistentMissionVariableManager()
		{
			variables = new List<PersistentMissionVariable>();
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Start()
		{
			base.Start();
		}

		public override bool IsValid()
		{
			return true;
		}

		public void Clear()
		{
			variables.Clear();
		}

		private void ValidateVarCount()
		{
		}

		public void Reset(List<PersistentMissionVariable> toVariablesValues)
		{
			variables.Clear();
			for (int i = 0; i < toVariablesValues.Count; i++)
			{
				variables.Add(toVariablesValues[i]);
			}
			ValidateVarCount();
		}

		public List<PersistentMissionVariable> GetAllVariableValues()
		{
			return variables;
		}

		public void CreateIntVariable(string name, int startValue, bool readOnly)
		{
			if (DoesVariableExist(name))
			{
				base.Debug.LogError("PersistentMissionVariableManager.CreateIntVariable attempted for an already existing variable name.");
				return;
			}
			PersistentMissionVariable item = new PersistentMissionVariable(name, startValue, readOnly);
			variables.Add(item);
			ValidateVarCount();
		}

		public void CreateStringVariable(string name, string startValue, bool readOnly)
		{
			if (DoesVariableExist(name))
			{
				base.Debug.LogError("PersistentMissionVariableManager.CreateStringVariable attempted for an already existing variable name.");
				return;
			}
			PersistentMissionVariable item = new PersistentMissionVariable(name, startValue, readOnly);
			variables.Add(item);
			ValidateVarCount();
		}

		private int GetVariableIndex(string name)
		{
			for (int i = 0; i < variables.Count; i++)
			{
				if (variables[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		public bool DoesVariableExist(string name)
		{
			return GetVariableIndex(name) != -1;
		}

		public int GetIntVariable(string name, int defaultValue)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				return variables[variableIndex].ValueInt;
			}
			return defaultValue;
		}

		public string GetStringVariable(string name, string defaultValue)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				return variables[variableIndex].ValueString;
			}
			return defaultValue;
		}

		public void DecreaseIntVariable(string name)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				variables[variableIndex].ValueInt--;
			}
		}

		public void DecreaseIntVariableUntilZero(string name)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1 && variables[variableIndex].ValueInt > 0)
			{
				variables[variableIndex].ValueInt--;
			}
		}

		public void IncreaseIntVariable(string name)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				variables[variableIndex].ValueInt++;
			}
		}

		public void SetIntVariable(string name, int value)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				if (variables[variableIndex].ReadOnly)
				{
					base.Debug.LogError("Attempt to set read-only persistent variable '" + name + "' value.");
				}
				else if (variables[variableIndex].ValueInt != value)
				{
					variables[variableIndex].ValueInt = value;
				}
			}
		}

		public void SetIntVariableNoNotificationAllowingReadOnly(string name, int value)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				variables[variableIndex].ValueInt = value;
			}
		}

		public void SetIntVariableCreatingIfNecessary(string name, int value, bool readOnly)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				if (variables[variableIndex].ReadOnly)
				{
					base.Debug.LogError("Attempt to set read-only persistent variable '" + name + "' value.");
				}
				else if (variables[variableIndex].ValueInt != value)
				{
					variables[variableIndex].ValueInt = value;
				}
			}
			else
			{
				CreateIntVariable(name, value, readOnly);
			}
		}

		public void SetStringVariable(string name, string value)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				if (variables[variableIndex].ReadOnly)
				{
					base.Debug.LogError("Attempt to set read-only persistent variable '" + name + "' value.");
				}
				else if (variables[variableIndex].ValueString != value)
				{
					variables[variableIndex].ValueString = value;
				}
			}
		}

		public void SetStringVariableNoNotificationAllowingReadOnly(string name, string value)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				variables[variableIndex].ValueString = value;
			}
		}

		public void SetStringVariableCreatingIfNecessary(string name, string value, bool readOnly)
		{
			int variableIndex = GetVariableIndex(name);
			if (variableIndex != -1)
			{
				if (variables[variableIndex].ReadOnly)
				{
					base.Debug.LogError("Attempt to set read-only persistent variable '" + name + "' value.");
				}
				else if (variables[variableIndex].ValueString != value)
				{
					variables[variableIndex].ValueString = value;
				}
			}
			else
			{
				CreateStringVariable(name, value, readOnly);
			}
		}
	}
}
