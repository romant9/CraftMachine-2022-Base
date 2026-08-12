using TWDModel;
using UnityEngine;

public class NewBieRewardCard : MonoBehaviour
{
	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UISprite border;

	[SerializeField]
	private UILabel rewardName;

	[SerializeField]
	private UITexture armorTexture;

	[SerializeField]
	private UITexture weaponTexture;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private UITexture heroTexture;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UISprite currencySprite;

	[SerializeField]
	private GameObject rewardContent;

	private NewbieStageReward pointReward;

	[SerializeField]
	private UIAtlas shopAtlas;

	[SerializeField]
	private GameObject smallBox;

	[SerializeField]
	private GameObject smallBoxLock;

	[SerializeField]
	private GameObject smallBoxUnlock;

	[SerializeField]
	private GameObject bigBox;

	[SerializeField]
	private GameObject bigBoxLock;

	[SerializeField]
	private GameObject bigBoxUnlock;

	[SerializeField]
	private UILabel pointLabel;

	private int flag;

	private bool lastOne;

	private void OnEnable()
	{
	}

	public void UpdateUI(NewbieStageReward reward, int points, bool lastOneFlag)
	{
		pointReward = reward;
		lastOne = lastOneFlag;
		Helpers.GameObjectSetActive(rewardContent, value: false);
		int amountForIReward = HelpersGfx.GetAmountForIReward(reward.RewardEntries.RewardsList[0]);
		amountForIReward = ((amountForIReward == 0) ? 1 : amountForIReward);
		rewardName.text = "x" + Helpers.FormatNumber(amountForIReward, 0, 1);
		Helpers.GameObjectSetActive(rewardName, value: true);
		Helpers.GameObjectSetActive(classIcon, value: false);
		flag = ((reward.PointNeeded <= points) ? 1 : 0);
		for (int i = 0; i < GameManager.Instance.playerModel.NewbieSenvenQuest.HadRewardedStage.Count; i++)
		{
			if (reward.PointNeeded == GameManager.Instance.playerModel.NewbieSenvenQuest.HadRewardedStage[i])
			{
				flag = 2;
				break;
			}
		}
		Helpers.GameObjectSetActive(smallBox, !lastOne);
		Helpers.GameObjectSetActive(bigBox, lastOne);
		if (lastOne)
		{
			Helpers.GameObjectSetActive(bigBoxUnlock, flag == 2);
			Helpers.GameObjectSetActive(bigBoxLock, flag != 2);
			rewardContent.transform.localPosition = new Vector3(rewardContent.transform.localPosition.x, rewardContent.transform.localPosition.y + 20f, rewardContent.transform.localPosition.z);
		}
		else
		{
			Helpers.GameObjectSetActive(smallBoxUnlock, flag == 2);
			Helpers.GameObjectSetActive(smallBoxLock, flag != 2);
		}
		Setup();
		pointLabel.text = reward.PointNeeded.ToString();
	}

	private void Setup()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool value = false;
		bool value2 = false;
		IReward reward = pointReward.RewardEntries.RewardsList[0];
		if (!(reward is RewardEquipment rewardEquipment))
		{
			if (!(reward is RewardCurrency))
			{
				if (reward is RewardTimedBonus rewardTimedBonus)
				{
					value2 = true;
					currencySprite.atlas = shopAtlas;
					currencySprite.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
				}
			}
			else
			{
				value2 = true;
				HelpersGfx.GetIconNameForIReward(pointReward.RewardEntries.RewardsList[0], out var spriteName, null, null, null);
				HelpersUI.SetSprite(currencySprite, spriteName);
			}
		}
		else
		{
			EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(rewardEquipment.EquipmentId);
			flag3 = equipmentDefinition.Category == EquipmentCategory.Utility;
			flag = !flag3 && equipmentDefinition.Type == EquipmentType.Armor;
			flag2 = !flag3 && !flag;
			if (flag)
			{
				armorTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentDefinition);
			}
			else if (flag2)
			{
				weaponTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentDefinition);
			}
			else
			{
				consumableTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentDefinition);
			}
			if (flag3)
			{
				Helpers.GameObjectSetActive(classIcon, value: false);
			}
		}
		Helpers.GameObjectSetActive(armorTexture, flag);
		Helpers.GameObjectSetActive(weaponTexture, flag2);
		Helpers.GameObjectSetActive(heroTexture, value);
		Helpers.GameObjectSetActive(currencySprite, value2);
		Helpers.GameObjectSetActive(consumableTexture, flag3);
	}

	public void OnClickSmall()
	{
		int pointNeeded = pointReward.PointNeeded;
		if (flag == 1)
		{
			if (Helpers.ExecuteCommand(new NewbieSevenQuestStageRewardCommand(pointNeeded)) == TWDModelResult.OK)
			{
				IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
				if (iAPConfirmPopupNew != null)
				{
					iAPConfirmPopupNew.OpenForRewards(pointReward.RewardEntries.RewardsList);
					flag = 2;
					Helpers.GameObjectSetActive(smallBoxLock, value: false);
					Helpers.GameObjectSetActive(smallBoxUnlock, value: true);
				}
			}
		}
		else if (rewardContent.activeSelf)
		{
			Helpers.GameObjectSetActive(rewardContent, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(rewardContent, value: true);
		}
	}

	public void OnClickBig()
	{
		int pointNeeded = pointReward.PointNeeded;
		if (flag == 1)
		{
			if (Helpers.ExecuteCommand(new NewbieSevenQuestStageRewardCommand(pointNeeded)) == TWDModelResult.OK)
			{
				IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
				if (iAPConfirmPopupNew != null)
				{
					iAPConfirmPopupNew.OpenForRewards(pointReward.RewardEntries.RewardsList);
					flag = 2;
					Helpers.GameObjectSetActive(bigBoxLock, value: false);
					Helpers.GameObjectSetActive(bigBoxUnlock, value: true);
				}
			}
		}
		else if (rewardContent.activeSelf)
		{
			Helpers.GameObjectSetActive(rewardContent, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(rewardContent, value: true);
		}
	}
}
