using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RewardBox : MonoBehaviour
{
	public List<GameObject> OnClickEffects;

	public List<GameObject> OnSwitchEffects;

	public LootEntry Reward { get; set; }

	public LootEntry Reward2 { get; set; }

	public bool Opened { get; set; }

	public bool Open(LootScreenType screenType, int boxIndex)
	{
		if (Opened)
		{
			return false;
		}
		CampHUD.Get().PauseCurrencyMeters = true;
		GameManager.Instance.CheckConnectionReachability(showPopup: true, "OpenLootBoxCommand");
		OpenLootBoxCommand openLootBoxCommand = new OpenLootBoxCommand();
		openLootBoxCommand.ScreenType = screenType;
		openLootBoxCommand.BoxIndex = boxIndex;
		if (Helpers.ExecuteCommand(openLootBoxCommand) == TWDModelResult.OK)
		{
			openLootBoxCommand.GetLoot(out var loot, out var loot2);
			Reward = loot;
			Reward2 = loot2;
			Animator componentInChildren = GetComponentInChildren<Animator>();
			if (componentInChildren != null && !componentInChildren.enabled)
			{
				componentInChildren.enabled = true;
			}
			Opened = true;
		}
		string eventName = "reward_screen/container_open_rarity_1";
		if (Reward != null)
		{
			switch (Reward.DropType)
			{
			case DropType.Silver:
				eventName = "reward_screen/container_open_rarity_2";
				break;
			case DropType.Gold:
				eventName = "reward_screen/container_open_rarity_3";
				break;
			}
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName);
		for (int i = 0; i < OnClickEffects.Count; i++)
		{
			OnClickEffects[i].SetActive(value: true);
		}
		return Opened;
	}

	public void OnPlayEffect()
	{
		RewardScreenHandler.Instance.PlayRewardBoxEffect(base.gameObject, Reward, Reward2);
	}

	public void VisualEffect(LootScreenType screenType = LootScreenType.Combat)
	{
		for (int i = 0; i < OnSwitchEffects.Count; i++)
		{
			if (OnSwitchEffects[i].name.Equals("LootBoxGlowBlob") && (screenType == LootScreenType.InUi || screenType == LootScreenType.InUiSurvival || screenType == LootScreenType.Ad || screenType == LootScreenType.InUIPlayer || screenType == LootScreenType.GuildGift || screenType == LootScreenType.TradeCrate || screenType == LootScreenType.IAPBonusGift || screenType == LootScreenType.Quiz || screenType == LootScreenType.DailyQuestChest || screenType == LootScreenType.BattlePassBonusChest))
			{
				OnSwitchEffects[i].SetActive(value: false);
			}
			else
			{
				OnSwitchEffects[i].SetActive(value: true);
			}
		}
	}

	public void OnAnimationEnded()
	{
		RewardScreenHandler.Instance.BoxAnimationOver();
	}
}
