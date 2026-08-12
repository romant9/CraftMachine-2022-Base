using TWDModel;
using UnityEngine;

public class DistanceBoosterRewardItem : MonoBehaviour
{
	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private GameObject boosterNotActiveState;

	[SerializeField]
	private GameObject rewardLockedState;

	[SerializeField]
	private GameObject rewardRedeemedState;

	[SerializeField]
	private GameObject equipmentRewardContainer;

	[SerializeField]
	private UITexture consumableIcon;

	[SerializeField]
	private GameObject equipmentCardPrefab;

	[SerializeField]
	private GameObject randomEquipmentCardPrefab;

	[SerializeField]
	public Vector3 equipmentCardScale = new Vector3(0.4f, 0.4f, 1f);

	private EquipmentButton equipmentButton;

	private EquipmentRandomButton equipmentRandomButton;

	private int rewardIndex = -1;

	public void Setup(IReward reward, int index)
	{
		rewardIcon.gameObject.SetActive(value: false);
		equipmentRewardContainer.SetActive(value: false);
		consumableIcon.gameObject.SetActive(value: false);
		amountLabel.gameObject.SetActive(value: false);
		rewardIndex = index;
		if (!(reward is RewardCurrency rewardCurrency))
		{
			if (!(reward is RewardRandomEquipment reward2))
			{
				RewardEquipment rewardEquipment = reward as RewardEquipment;
				if (rewardEquipment == null)
				{
					return;
				}
				if (rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
				{
					consumableIcon.gameObject.SetActive(value: true);
					amountLabel.gameObject.SetActive(value: true);
					consumableIcon.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
					amountLabel.text = rewardEquipment.Amount.ToString();
					UIButtonExtended component = consumableIcon.GetComponent<UIButtonExtended>();
					component.Clear();
					component.SetClickCallback(delegate
					{
						TooltipManager.OpenTextBoxWithText(consumableIcon.gameObject, HelpersLocalization.GetShopTooltipForIReward(rewardEquipment));
					});
				}
				else
				{
					equipmentRewardContainer.SetActive(value: true);
					if (equipmentButton == null)
					{
						GameObject gameObject = Helpers.InstantiateToParentAndLayer(equipmentCardPrefab, equipmentRewardContainer);
						gameObject.transform.localScale = equipmentCardScale;
						equipmentButton = gameObject.GetComponent<EquipmentButton>();
					}
					equipmentButton.Setup(rewardEquipment, allowClick: true, traitsUnknown: true);
				}
			}
			else
			{
				equipmentRewardContainer.SetActive(value: true);
				if (equipmentRandomButton == null)
				{
					GameObject gameObject2 = Helpers.InstantiateToParentAndLayer(randomEquipmentCardPrefab, equipmentRewardContainer);
					gameObject2.transform.localScale = equipmentCardScale;
					equipmentRandomButton = gameObject2.GetComponent<EquipmentRandomButton>();
				}
				equipmentRandomButton.Setup(reward2);
			}
		}
		else
		{
			rewardIcon.gameObject.SetActive(value: true);
			amountLabel.gameObject.SetActive(value: true);
			rewardIcon.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
			amountLabel.text = rewardCurrency.Amount.ToString();
		}
	}

	public void UpdateRewardState()
	{
		WeeklySurvivalModel weeklySurvival = GameManager.Instance.playerModel.WeeklySurvival;
		boosterNotActiveState.SetActive(!weeklySurvival.DoubleRewardsEnabled && rewardIndex < weeklySurvival.NumberCompleted);
		rewardRedeemedState.SetActive(weeklySurvival.DoubleRewardsEnabled && rewardIndex < weeklySurvival.NumberCompleted);
		rewardLockedState.SetActive(rewardIndex >= weeklySurvival.NumberCompleted);
	}
}
