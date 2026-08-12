using TWDModel;
using UnityEngine;

public class ReturnRewardCard : MonoBehaviour
{
	[SerializeField]
	private UISprite background;

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
			if (primaryReward != null)
			{
				HelpersGfx.GetIconNameForIReward(primaryReward, out var spriteName, null, null, null);
				HelpersUI.SetSprite(background, spriteName);
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
				BuildingsHUD.Get()?.CreateCollectAnim(Item.RewardEntries, base.gameObject);
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
