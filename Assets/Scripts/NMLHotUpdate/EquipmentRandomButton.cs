using UnityEngine;

public class EquipmentRandomButton : MonoBehaviourExtended
{
	[SerializeField]
	private GameObject[] rarityStarsList;

	[SerializeField]
	private UISprite weaponSprite;

	[SerializeField]
	private UISprite rarityBgSprite;

	[SerializeField]
	private UIButtonExtended button;

	private RewardRandomEquipment rewardRandomEquipment;

	public void Setup(RewardRandomEquipment reward)
	{
		rewardRandomEquipment = reward;
		if (rewardRandomEquipment != null)
		{
			HelpersUI.SetSprite(rarityBgSprite, GameManager.Instance.GetEquipmentBackgroundRaritySprite(rewardRandomEquipment.RarityLevel));
			string spriteName = "";
			HelpersGfx.GetIconNameForIReward(rewardRandomEquipment, out spriteName, null, null, null);
			HelpersUI.SetSprite(weaponSprite, spriteName);
			setRarityStars(rewardRandomEquipment.RarityLevel);
			if (button != null)
			{
				button.SetClickCallback(OnButtonClicked);
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (button != null)
		{
			button.Clear();
		}
		rewardRandomEquipment = null;
	}

	public void OnButtonClicked(UIButtonExtended target)
	{
		if (rewardRandomEquipment != null)
		{
			string rarityLevel = HelpersLocalization.GetRarityLevel(rewardRandomEquipment.RarityLevel);
			string survivorClassName = HelpersLocalization.GetSurvivorClassName(rewardRandomEquipment.SurvivorClass);
			string text = LocalizationManager.GetText("Popup.Shop.Random" + rewardRandomEquipment.Category.ToString() + ".Tooltip{RarityName}{ClassName}", rarityLevel, survivorClassName);
			TooltipManager.OpenTextBoxWithText(base.gameObject, text);
		}
	}

	protected void OnPoolReturn()
	{
		Clear();
	}

	private void setRarityStars(int rarityIndex)
	{
		if (rarityStarsList != null && rarityStarsList.Length != 0)
		{
			for (int i = 0; i < rarityStarsList.Length; i++)
			{
				Helpers.GameObjectSetActive(rarityStarsList[i], i <= rarityIndex);
			}
		}
	}
}
