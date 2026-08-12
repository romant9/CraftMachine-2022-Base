using System.Collections.Generic;

namespace TWDModel
{
	public class QuestCompleteContext
	{
		public delegate long Function(QuestCompleteContext context, List<QuestDefinitionOperator> arguments);

		public Dictionary<string, Function> Functions = new Dictionary<string, Function>();

		public QuestVariables Variables = new QuestVariables();

		public Dictionary<string, long> ValueMap = new Dictionary<string, long>();

		public List<string> StringValues = new List<string>();

		public TWDModelManager ModelManager { get; private set; }

		public QuestCompleteContext(TWDModelManager modelManager)
		{
			ModelManager = modelManager;
		}

		public long MapStringValueToInteger(string value)
		{
			if (value == null)
			{
				return 0L;
			}
			if (ValueMap.ContainsKey(value))
			{
				return ValueMap[value];
			}
			return 0L;
		}

		public string MapValueToString(long value)
		{
			if (value < StringValues.Count)
			{
				return StringValues[(int)value];
			}
			return null;
		}
	}
}
