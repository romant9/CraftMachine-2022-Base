using TWDModel;

public class RewardAvatars : IReward
{
	public int Avatar = -1;

	public int Border = -1;

	public int Color = -1;

	public RewardType Type => RewardType.Avatars;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		int num = 0;
		if (Avatar >= 0)
		{
			if (!manager.Player.IconIndexs.Contains(Avatar))
			{
				manager.Player.AddIconIndex(Avatar);
				return Avatar;
			}
			num = manager.GameEconomyData.ConfigData.AvatarToGold;
		}
		else if (Border >= 0)
		{
			if (!manager.Player.BorderIndexs.Contains(Border))
			{
				manager.Player.AddBorderIndex(Border);
				return Border;
			}
			num = manager.GameEconomyData.ConfigData.BorderrToGold;
		}
		else if (Color >= 0)
		{
			if (!manager.Player.ColorIndexs.Contains(Color))
			{
				manager.Player.AddColorIndex(Color);
				return Color;
			}
			num = manager.GameEconomyData.ConfigData.AvatarColorToGold;
		}
		if (num > 0)
		{
			manager.Player.GetCurrency(CurrencyType.Diamonds).Add(num);
			return CurrencyType.Diamonds;
		}
		return null;
	}
}
