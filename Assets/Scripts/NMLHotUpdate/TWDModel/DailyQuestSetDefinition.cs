using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class DailyQuestSetDefinition
	{
		public string Id;

		public int CouncilLevelMin;

		public int CouncilLevelMax;

		public string Q1;

		public string Q2;

		public string Q3;

		public string Q4;

		public string Q5;

		public string RewardSets;

		[JsonIgnore]
		public List<DailyQuestSelectionDefinition> Q1Definition;

		[JsonIgnore]
		public List<DailyQuestSelectionDefinition> Q2Definition;

		[JsonIgnore]
		public List<DailyQuestSelectionDefinition> Q3Definition;

		[JsonIgnore]
		public List<DailyQuestSelectionDefinition> Q4Definition;

		[JsonIgnore]
		public List<DailyQuestSelectionDefinition> Q5Definition;

		private List<DailyQuestSelectionDefinition> LoadDefinitions(string definitionString)
		{
			List<DailyQuestSelectionDefinition> list = new List<DailyQuestSelectionDefinition>();
			string[] array = definitionString.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				DailyQuestSelectionDefinition dailyQuestSelectionDefinition = new DailyQuestSelectionDefinition();
				int num = text.IndexOf('(');
				int num2 = text.IndexOf(')', num);
				dailyQuestSelectionDefinition.Size = DailyQuestDefinitionSize.S;
				dailyQuestSelectionDefinition.Weight = 100;
				if (num > 0 && num2 > 0)
				{
					dailyQuestSelectionDefinition.QuestCategory = text.Substring(0, num);
					num++;
					string[] array2 = text.Substring(num, num2 - num).Split(',');
					if (array2.Length != 0)
					{
						string text2 = array2[0].Trim();
						if (text2 == "M")
						{
							dailyQuestSelectionDefinition.Size = DailyQuestDefinitionSize.M;
						}
						else if (text2 == "L")
						{
							dailyQuestSelectionDefinition.Size = DailyQuestDefinitionSize.L;
						}
					}
					if (array2.Length > 1 && int.TryParse(array2[1].Trim(), out var result))
					{
						dailyQuestSelectionDefinition.Weight = result;
					}
				}
				else
				{
					dailyQuestSelectionDefinition.QuestCategory = text;
				}
				list.Add(dailyQuestSelectionDefinition);
			}
			return list;
		}

		public void LoadSelectionDefinitions()
		{
			Q1Definition = LoadDefinitions(Q1);
			Q2Definition = LoadDefinitions(Q2);
			Q3Definition = LoadDefinitions(Q3);
			Q4Definition = LoadDefinitions(Q4);
			Q5Definition = LoadDefinitions(Q5);
		}
	}
}
