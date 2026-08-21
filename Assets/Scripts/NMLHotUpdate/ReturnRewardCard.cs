using TWDModel;
using UnityEngine;

public class ReturnRewardCard : MonoBehaviour
{
	[SerializeField]
	private GameObject rewardGo;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UILabel itemNumLabel;

	[SerializeField]
	private UILabel dayLabel;

	[SerializeField]
	private GameObject claimedGameobject;

	[SerializeField]
	private GameObject selectedGameObject;

	[SerializeField]
	private UIButton button;

	[SerializeField]
	private EquipmentButton rewardEquip;

	[SerializeField]
	private ReturnLoginShopRewardModSkillItem rewardModSkill;

	public ReturnLoginDayItemModel Item { get; private set; }

	private void Awake()
	{
		button.onClick.Add(new EventDelegate(OnClaimClicked));
	}

	public void Bind(ReturnLoginDayItemModel dayItem)
	{
		Item = dayItem;
		Refresh();
	}

	private void Refresh()
	{
		if (Item != null)
		{
			HelpersUI.SetContentToLabel(dayLabel, LocalizationManager.GetText("GvG.Hub.Calendar.SelectedDay{day}", Item.Day));
			IReward primaryReward = GetPrimaryReward();
			ReturnLoginShopPanel.Apply(primaryReward, rewardIcon, rewardEquip, rewardModSkill);
			Helpers.GameObjectSetActive(rewardGo, rewardIcon.gameObject.activeSelf);
			if (primaryReward != null)
			{
				int numsForIReward = Helpers.GetNumsForIReward(primaryReward);
				HelpersUI.SetContentToLabel(itemNumLabel, numsForIReward.ToString());
			}
			bool haveClaimed = Item.HaveClaimed;
			bool flag = Item.RewardStatus == ReturnLoginRewardStatus.ReadyToClaim;
			Helpers.GameObjectSetActive(claimedGameobject, haveClaimed);
			Helpers.GameObjectSetActive(selectedGameObject, flag && !haveClaimed);
		}
	}

	private void OnClaimClicked()
	{
		if (Item != null && Item.RewardStatus == ReturnLoginRewardStatus.ReadyToClaim)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/reward_claim");
			if (Helpers.ExecuteCommand(new ClaimReturnLoginRewardCommand(Item.Day)) == TWDModelResult.OK)
			{
				ReturnLoginShopPanel.ShowRewardPopup(Item.RewardEntries);
				UIEvent.Send("ReturnLoginSevenDayClaimEvent", Item.Day);
			}
		}
	}

	private IReward GetPrimaryReward()
	{
		if (Item?.RewardEntries == null || Item.RewardEntries.Count <= 0)
		{
			return null;
		}
		return Item.RewardEntries.GetRewardAt(0);
	}
}
