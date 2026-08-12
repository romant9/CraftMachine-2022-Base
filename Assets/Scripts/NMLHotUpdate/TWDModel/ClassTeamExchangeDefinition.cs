using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class ClassTeamExchangeDefinition
	{
		public int ID;

		public int ChallengeID;

		public string Content;

		public string Cost;

		public int Limit;

		public bool IsCloseExchange;

		[NonSerialized]
		[JsonIgnore]
		public Rewards ContentRewards;

		[NonSerialized]
		[JsonIgnore]
		public Rewards CostRewards;

		public void InitializeRewards(TWDModelManager manager)
		{
			if (manager != null)
			{
				if (ContentRewards == null && !string.IsNullOrEmpty(Content))
				{
					ContentRewards = new Rewards(Content, manager);
				}
				if (CostRewards == null && !string.IsNullOrEmpty(Cost))
				{
					CostRewards = new Rewards(Cost, manager);
				}
			}
		}
	}
}
