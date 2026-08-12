using System.Collections.Generic;
using TWDModel;

public class RewardHeroSkin : IReward
{
	public List<string> PreferredOrder { get; set; }

	public RewardType Type => RewardType.HeroSkin;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		if (PreferredOrder != null && PreferredOrder.Count > 0)
		{
			for (int i = 0; i < PreferredOrder.Count; i++)
			{
				string text = PreferredOrder[i];
				if (!manager.Player.SurvivorContainer.HasHeroSkin(text))
				{
					manager.Player.SurvivorContainer.AddHeroSkin(text);
					return text;
				}
			}
		}
		return null;
	}
}
