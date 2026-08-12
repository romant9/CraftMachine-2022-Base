using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildShopPopup : UIToggleContent
{
	[Header("Main List")]
	[SerializeField]
	private NUIScrollableList scrollableList;

	[Header("Timer")]
	[SerializeField]
	private UILabel resetTimerLabel;

	[SerializeField]
	private UISprite resetTimerBackground;

	[SerializeField]
	private Color normalTimerBackgroundColor;

	[SerializeField]
	private Color warningTimerBackgroundColor;

	private const float TimerUpdateInterval = 1f;

	public const string defaultGuildCardPrefabName = "Shop_Guild_Card";

	private List<GuildShopItemInfo> currentGuildShopItems;

	private bool showResetEffectOnCards;

	public static void OpenGuildShop()
	{
		if (GuildWarHelper.IsGuildMember())
		{
			SocialPopupGuild socialPopupGuild = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialPopupGuild) as SocialPopupGuild;
			if (socialPopupGuild != null)
			{
				GuildBattleOverviewPopup guildBattleOverviewPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildBattleOverviewPopup) as GuildBattleOverviewPopup;
				if (guildBattleOverviewPopup != null)
				{
					guildBattleOverviewPopup.Close();
				}
				socialPopupGuild.OpenForTab(4);
			}
		}
		else
		{
			AlertPopup.ShowPopupGetText("Popup.Social.NotInAGuild", "Popup.Social.GuildMessage", "Button.Ok", null);
		}
	}

	public override void Activate()
	{
		base.Activate();
		Setup();
		GameManager.Instance.playerModel.GuildShopModel.Changed -= OnGuildShopModelChanged;
		GameManager.Instance.playerModel.GuildShopModel.Changed += OnGuildShopModelChanged;
		UIEvent.OnUIEvent -= OnUiEvent;
		UIEvent.OnUIEvent += OnUiEvent;
	}

	public void Setup()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/open_shop");
		UpdateTab();
		List<int> list = new List<int>();
		foreach (GuildShopItemInfo currentGuildShopItem in currentGuildShopItems)
		{
			if (currentGuildShopItem.Unlocked && !currentGuildShopItem.Seen)
			{
				list.Add(currentGuildShopItem.ItemDefinition.ID);
			}
		}
		MarkItemsAsSeen(list);
		if (GuildWarHelper.GetTimeLeftToNextSeason() == 0L)
		{
			Helpers.GameObjectSetActive(resetTimerLabel, value: false);
			Helpers.GameObjectSetActive(resetTimerBackground, value: false);
		}
		else
		{
			HelpersUI.SetColor(resetTimerBackground, GuildWarHelper.IsSeasonOngoing() ? normalTimerBackgroundColor : warningTimerBackgroundColor);
			StartCoroutine(UpdateTimer(1f));
		}
	}

	private void OnDisable()
	{
		GameManager.Instance.playerModel.Changed -= OnGuildShopModelChanged;
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	public virtual void Clear()
	{
		ClearList();
		currentGuildShopItems = new List<GuildShopItemInfo>();
		StopCoroutine(UpdateTimer(0f));
		showResetEffectOnCards = false;
	}

	public virtual void UpdateTab()
	{
		SetupAvailableItems();
		if (scrollableList != null)
		{
			scrollableList.SaveCurrentScrollPosition();
			UpdateListWithData(currentGuildShopItems, resetScrollPosition: false);
		}
		if (!showResetEffectOnCards || scrollableList.currentItemsList == null)
		{
			return;
		}
		showResetEffectOnCards = false;
		for (int i = 0; i < scrollableList.currentItemsList.Count; i++)
		{
			if (scrollableList.currentItemsList[i] != null && scrollableList.currentItemsList[i] is GuildShopItemCard)
			{
				(scrollableList.currentItemsList[i] as GuildShopItemCard).TriggerResetEffects();
			}
		}
	}

	private void UpdateListWithData<T>(List<T> data, bool resetScrollPosition, string prefabResourceOverride = "") where T : class
	{
		if (scrollableList != null)
		{
			scrollableList.GetScrollPosition();
			scrollableList.Clear();
			NUIListItem<T> nUIListItem = null;
			string text = "";
			string text2 = "";
			for (int i = 0; i < data.Count; i++)
			{
				text = "";
				text2 = "";
				if (data[i] == null)
				{
					continue;
				}
				if (string.IsNullOrEmpty(prefabResourceOverride))
				{
					if (data[i] is GuildShopItemInfo)
					{
						text = "Shop_Guild_Card";
					}
				}
				else
				{
					text = prefabResourceOverride;
				}
				nUIListItem = scrollableList.InstantiateAdd(text) as NUIListItem<T>;
				if (nUIListItem != null)
				{
					nUIListItem.SetData(data[i]);
					continue;
				}
				Debug.LogError("GuildShopPopup: Could not load Prefab from: " + text + "Type:" + data[i]?.ToString() + " Item: " + text2);
			}
			if (resetScrollPosition)
			{
				scrollableList.SortAndReset();
			}
			else
			{
				StartCoroutine(SetUpScrollView());
			}
			nUIListItem = null;
		}
		else
		{
			Debug.LogError("GuildShopPopup: No Prefab Reference to a NUIScrollableList defined!");
		}
	}

	private IEnumerator SetUpScrollView()
	{
		yield return null;
		scrollableList.SortAndRepositionItems();
		scrollableList.ResetScrollPosition();
	}

	private void ClearList()
	{
		if (scrollableList != null)
		{
			scrollableList.Clear();
		}
	}

	private IEnumerator UpdateTimer(float interval)
	{
		while (interval > 0f)
		{
			HelpersUI.SetContentToLabel(resetTimerLabel, LocalizationManager.GetText("Popup.GuildShop.SeasonResetIn{parameter}", GuildWarHelper.GetFormatedTimeLeftToNextSeason()));
			yield return new WaitForSeconds(interval);
		}
	}

	private void SetupAvailableItems()
	{
		currentGuildShopItems = new List<GuildShopItemInfo>();
		currentGuildShopItems = GameManager.Instance.playerModel.GuildShopModel.GuildShopAvailableItems.Values.ToList();
		bool highTierFirst = GameManager.Instance.gameEconomyData.GuildWarConfig.GuildShopHighestTierFirst;
		currentGuildShopItems.StableSort(delegate(GuildShopItemInfo a, GuildShopItemInfo b)
		{
			int num = 0;
			num = a.SoldOut.CompareTo(b.SoldOut);
			if (num == 0)
			{
				num = a.ItemDefinition.VIPRequired.CompareTo(b.ItemDefinition.VIPRequired);
			}
			if (num == 0)
			{
				num = -a.Unlocked.CompareTo(b.Unlocked);
			}
			if (num == 0)
			{
				num = a.ItemDefinition.TierRequirement.CompareTo(b.ItemDefinition.TierRequirement);
				if (!a.Unlocked || !highTierFirst)
				{
					num *= -1;
				}
			}
			return num;
		});
	}

	private void OnGuildShopModelChanged(ModelObject m, string changed, object args)
	{
		if (changed == "GuildShopRestocked")
		{
			showResetEffectOnCards = true;
			UpdateTab();
		}
	}

	private void OnUiEvent(string type, object parameter)
	{
	}

	private void MarkItemsAsSeen(List<int> itemIds)
	{
		if (itemIds.Count > 0)
		{
			Helpers.ExecuteCommand(new MarkGuildShopItemsSeenCommand(itemIds));
		}
	}
}
