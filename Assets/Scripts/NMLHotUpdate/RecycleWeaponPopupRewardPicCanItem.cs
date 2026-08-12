using TWDModel;
using UnityEngine;

public class RecycleWeaponPopupRewardPicCanItem : MonoBehaviour
{
	[SerializeField]
	private UISprite skillClassIcon;

	[SerializeField]
	private UILabel countLabel;

	[SerializeField]
	private RouletteRewardCard rewardCard;

	[SerializeField]
	private RecycleWeaponPopupRewardPicItem picItem;

	public void Setup(RewardShowPicEntry reward, int count, string survivorClass)
	{
		Helpers.GameObjectSetActive(rewardCard, value: false);
		Helpers.GameObjectSetActive(picItem, value: true);
		picItem.Setup(reward);
		if (string.IsNullOrEmpty(survivorClass))
		{
			Helpers.GameObjectSetActive(skillClassIcon, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(skillClassIcon, value: true);
			skillClassIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(survivorClass);
		}
		countLabel.text = $"{reward.Count * count}";
	}

	public void Setup(IReward reward, int count)
	{
		Helpers.GameObjectSetActive(rewardCard, value: true);
		Helpers.GameObjectSetActive(picItem, value: false);
		Helpers.GameObjectSetActive(skillClassIcon, value: false);
		rewardCard.Bind(reward);
		rewardCard.SetAmountContainerEnable(enable: false);
		int amount = rewardCard.amount;
		countLabel.text = $"{amount * count}";
	}
}
