namespace TWDModel
{
	public class PersistentMissionVariable
	{
		public string Name { get; private set; }

		public bool ReadOnly { get; private set; }

		public int ValueInt { get; set; }

		public string ValueString { get; set; }

		public PersistentMissionVariable()
		{
		}

		public PersistentMissionVariable(PersistentMissionVariable persistentMissionVariable)
		{
			Name = persistentMissionVariable.Name;
			ReadOnly = persistentMissionVariable.ReadOnly;
			ValueInt = persistentMissionVariable.ValueInt;
			ValueString = persistentMissionVariable.ValueString;
		}

		public static string GetPresetVariableName(PersistentVariablePresetName preset)
		{
			if (preset != PersistentVariablePresetName.None)
			{
				return preset.ToString();
			}
			return null;
		}

		public PersistentMissionVariable(string name, int valueInt, bool readOnly)
		{
			Name = name;
			ReadOnly = readOnly;
			ValueInt = valueInt;
		}

		public PersistentMissionVariable(string name, string valueString, bool readOnly)
		{
			Name = name;
			ReadOnly = readOnly;
			ValueString = valueString;
		}
	}
}
