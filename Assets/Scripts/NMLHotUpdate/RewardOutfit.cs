using System.Collections.Generic;
using TWDModel;

public class RewardOutfit : IReward
{
	public List<string> PreferredOrder { get; set; }

	public RewardType Type => RewardType.Outfit;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		if (PreferredOrder != null && PreferredOrder.Count > 0)
		{
			for (int i = 0; i < PreferredOrder.Count; i++)
			{
				string text = PreferredOrder[i];
				if (!manager.Player.SurvivorContainer.HasOutfit(text))
				{
					manager.Player.SurvivorContainer.AddOutfit(text);
					return text;
				}
			}
		}
		return null;
	}
}
