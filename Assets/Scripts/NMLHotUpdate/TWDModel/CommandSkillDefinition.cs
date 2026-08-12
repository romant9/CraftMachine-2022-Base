using System;
using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	[Serializable]
	public class CommandSkillDefinition
	{
		public int ID;

		public CommandSkillSkillType SkillType;

		public CommandSkillFuncType SkillFunc;

		public int UseType;

		public string UseDesc;

		public List<CommandSkillTargetType> TargetType;

		public int Range;

		public int TargetArea;

		public CommandSkillType Type;

		public List<string> Parameters;

		public List<string> EffectIndex;

		public List<string> SelfTraitsApply;

		public List<string> TargetTraitsApply;

		public int Cooldown;

		public int APCost;

		public string Name;

		public string Desc;

		public string Icon;

		public string IconColour;

		public string IconBGColour;

		public int GetParameterCount()
		{
			return Parameters.Count;
		}

		public T GetParameter<T>(int index)
		{
			string text = Parameters[index];
			if (typeof(T).IsEnum)
			{
				return (T)Enum.Parse(typeof(T), text);
			}
			if (typeof(T) == typeof(FixedPoint))
			{
				return (T)(object)new FixedPoint(text.ToString());
			}
			return (T)Convert.ChangeType(text, typeof(T));
		}

		public List<EffectIndexPriorityItem> GetEffectIndexPriorityItems()
		{
			Dictionary<int, EffectIndexPriorityItem> dictionary = new Dictionary<int, EffectIndexPriorityItem>();
			foreach (string item2 in EffectIndex)
			{
				string[] array = item2.Split(':');
				string item = array[0];
				int num = int.Parse(array[1]);
				if (dictionary.ContainsKey(num))
				{
					dictionary[num].NegativeEffects.Add(item);
					continue;
				}
				List<string> list = new List<string>();
				list.Add(item);
				EffectIndexPriorityItem value = new EffectIndexPriorityItem(num, list);
				dictionary.Add(num, value);
			}
			return dictionary.Values.OrderByDescending((EffectIndexPriorityItem effectIndexPriorityItem) => effectIndexPriorityItem.Priority).ToList();
		}
	}
}
