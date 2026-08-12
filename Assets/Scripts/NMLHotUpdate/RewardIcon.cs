using TWDModel;
using UnityEngine;

public class RewardIcon : NUIGridItem
{
	[SerializeField]
	private UISprite spriteIcon;

	[SerializeField]
	private UITexture textureIcon;

	[Header("Optional")]
	[SerializeField]
	private UILabel amountLabel;

	private void Awake()
	{
		DebugIdString = "RewardIcon";
	}

	public bool SetReward(IReward reward)
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		Helpers.GameObjectSetActive(textureIcon, value: false);
		if (reward != null && reward is RewardCurrency)
		{
			HelpersUI.SetContentToLabel(amountLabel, (reward as RewardCurrency).Amount.ToString(), (reward as RewardCurrency).Amount > 0);
			return HelpersUI.SetSprite(spriteIcon, HelpersGfx.GetCurrencyIconName((reward as RewardCurrency).CurrencyType, GameManager.Instance.playerModel));
		}
		if (reward != null && reward is RewardSkipChallange)
		{
			HelpersUI.SetContentToLabel(amountLabel, (reward as RewardSkipChallange).Amount.ToString(), (reward as RewardSkipChallange).Amount > 0);
			HelpersGfx.GetIconNameForIReward(reward, out var spriteName, null, null, null, GameManager.Instance.playerModel);
			return HelpersUI.SetSprite(spriteIcon, spriteName);
		}
		if ((reward != null && reward is RewardEquipment) || reward is RewardRandomEquipment)
		{
			if (reward is RewardEquipment rewardEquipment)
			{
				int num = ((rewardEquipment.Amount == 0) ? 1 : rewardEquipment.Amount);
				HelpersUI.SetContentToLabel(amountLabel, num.ToString());
				if (textureIcon != null)
				{
					textureIcon.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
					Helpers.GameObjectSetActive(spriteIcon, value: false);
					Helpers.GameObjectSetActive(textureIcon, value: true);
					return true;
				}
			}
			else
			{
				HelpersUI.SetContentToLabel(amountLabel, "1");
			}
			return HelpersUI.SetSprite(spriteIcon, HelpersGfx.GetSpriteNameForLootType(DropEventDefinition.DropEventTag.PreferEquipment));
		}
		Helpers.GameObjectSetActive(textureIcon, value: false);
		Helpers.GameObjectSetActive(spriteIcon, value: false);
		Helpers.GameObjectSetActive(amountLabel, value: false);
		return false;
	}

	public bool SetReward(DropEventDefinition.DropEventTag dropTag, int amount = -1)
	{
		HelpersUI.SetContentToLabel(amountLabel, amount.ToString(), amount > -1);
		HelpersUI.SetSprite(spriteIcon, HelpersGfx.GetSpriteNameForLootType(dropTag));
		return Helpers.GameObjectSetActive(base.gameObject, dropTag != DropEventDefinition.DropEventTag.None);
	}
}
