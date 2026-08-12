using System;

namespace TWDModel
{
	[Serializable]
	public class DailyQuestDefinition
	{
		public string Id;

		public string DisplayName;

		public string DisplayDescription;

		public string Category;

		public int S;

		public int M;

		public int L;

		public int CouncilLevelMin;

		public int CouncilLevelMax;

		public string Rule;

		public string IsAvailableRule;

		public DailyQuestCompletionWindow CompletionWindow;

		public int CompletionMinCountInWindow;

		public string DeepLink;

		public int GetSize(DailyQuestDefinitionSize size)
		{
			return size switch
			{
				DailyQuestDefinitionSize.S => S, 
				DailyQuestDefinitionSize.M => M, 
				DailyQuestDefinitionSize.L => L, 
				_ => -1, 
			};
		}

		public int GetSizeWithIndex(int index)
		{
			return index switch
			{
				0 => S, 
				1 => M, 
				2 => L, 
				_ => -1, 
			};
		}
	}
}
