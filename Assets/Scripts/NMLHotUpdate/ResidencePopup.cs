using BaseModel;
using Client.Connectivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class ResidencePopup : HUDElement
{
	public UIButtonToggleSet toggleSet;

	[SerializeField]
	private GameObject[] tabContainers;

	[SerializeField]
	private UIButtonExtended shopButton;

	[SerializeField]
	private ComponentInventoryPanel componentInventoryPanel;

	private int currentSlotIndex;

	public override void Open()
	{
		base.Open();
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			DebugTWD.Log("Open Residence Popup");
			return;
		}
		if (GameManager.Instance.playerModel.Camp.GetBuilding("Residence") is ResidenceBuildingModel)
		{
			UpdateComponentInventory();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_residence");
		}
	}

	public override void Close()
	{
		UIEvent.Send("OnResidenceClosed");
		base.Close();
	}

	public void OpenAtTabIndex(int index)
	{
		if (toggleSet != null)
		{
			Open();
			toggleSet.SetSelectedIndex(index);
		}
	}

	private void OnEnable()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			OpenAtTabIndex(lastSelectedTabIndex);
			return;
		}
		GameManager.Instance.playerModel.Changed += OnPlayerChanged;
		UIEvent.OnUIEvent += OnUIEvent;
		toggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Combine(toggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnTabChanged));
		if (shopButton != null)
		{
			shopButton.SetClickCallback(OnShopClicked);
		}
	}

	private void OnDisable()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			return;
		}
		GameManager.Instance.playerModel.Changed -= OnPlayerChanged;
		UIEvent.OnUIEvent -= OnUIEvent;
		if (shopButton != null)
		{
			shopButton.RemoveClickCallback(OnShopClicked);
		}
		UIButtonToggleSet uIButtonToggleSet = toggleSet;
		uIButtonToggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Remove(uIButtonToggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnTabChanged));
	}

	public void UpdateComponentInventory()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel == null) return;
		List<CurrencyModel> list = new List<CurrencyModel>();
		List<CurrencyType> allComponentBaseCurrencies = ComponentHelper.GetAllComponentBaseCurrencies();
		if (!IsLoadDataManager)
		{
			for (int i = 0; i < allComponentBaseCurrencies.Count; i++)
			{
				list.Add(playerModel.GetCurrency(allComponentBaseCurrencies[i]));
			}
			componentInventoryPanel.SetCards(list);
		}
		else
		{
			DebugTWD.LogMycode("if (!IsLoadDataManager)");
			CraftSettings settings = CraftSettings.Instance;
			for (int i = 0; i < allComponentBaseCurrencies.Count; i++)
			{
				CurrencyModel currencyModel;
				if (!settings.IsRealPlayerData)
				{
					var _currencyModel = settings.Currency.FirstOrDefault(x => x.Type == allComponentBaseCurrencies[i]);
					if (_currencyModel != null)
					{
						currencyModel = _currencyModel;
					}
					else
					{
						currencyModel = new CurrencyModel(allComponentBaseCurrencies[i]);
						currencyModel.SetValue(settings.CurrencyCountMax);
						settings.Currency.Add(currencyModel);
					}
				}
				else
				{
					currencyModel = playerModel.GetCurrency(allComponentBaseCurrencies[i]);
				}
				list.Add(currencyModel);
			}
			if (componentInventoryPanel != null && componentInventoryPanel.gameObject.activeInHierarchy)
			{
				componentInventoryPanel.SetCards(list);
			}
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager) return");
			return;
		}
		if (type == "OnPopUpClose" && parameter is HUDElement && (parameter as HUDElement).UIType != UIType.CampResidencePopup)
		{
			UpdateComponentInventory();
		}
	}

	protected void OnPlayerChanged(ModelObject model, string changed, object args)
	{
		if (changed == LootManagerModel.BadgeCreatedEvent)
		{
			UpdateComponentInventory();
		}
	}

	public void SetCurrentTab(int index)
	{
		if (!IsLoadDataManager)
		{
			for (int i = 0; i < tabContainers.Length; i++)
			{
				tabContainers[i].SetActive(i == index);
			}
			StartCoroutine(DelayedScrollViewReset(index));
		}
		else
		{
			if (lastSelectedTabIndex == 0)
			{
				HUDManager.Instance.CloseIfExists(UIType.ConsumablesCampPopup);
			}
			for (int i = 0; i < tabContainers.Length; i++)
			{
				var radioCallPopup = tabContainers[i].GetComponent<NewPhonePopup>();
				var survivorsPopup = tabContainers[i].GetComponent<SurvivorManagementPopUp>();
				SelectWeaponsPopup selectWeaponPopup = NewPhonePopup.Instance != null ? NewPhonePopup.Instance._SelectWeaponPopup : null;

				if (selectWeaponPopup != null && i != 3)
				{
					selectWeaponPopup.OnClickClose();
				}
				if (radioCallPopup != null)
				{
					if (index == 3 && DataManager.Instance.DoOpenPhonesPopup)
					{
						radioCallPopup.gameObject.SetActive(true);
						if (!radioCallPopup.IsInitDone)
							radioCallPopup.Open();
						else radioCallPopup.UpdateUI();
						radioCallPopup.EnableAllCallButtonsByPrice();
					}
					else
					{
						radioCallPopup.gameObject.SetActive(false);
					}
				}
				else if (survivorsPopup != null)
				{
					// Training Grounds
					if (index == 4)
					{
						survivorsPopup.gameObject.SetActive(true);
						if (!survivorsPopup.IsInitDone)
						{
							survivorsPopup.Open();
						}
						else survivorsPopup.UpdateUI();

						if (survivorsPopup.SurvivorInfoPopupCurrent != null)
						{
							survivorsPopup.SurvivorInfoPopupCurrent.gameObject.SetActive(true);
						}
					}
					else
					{
						if (survivorsPopup.gameObject.activeSelf)
						{
							var SurvivorInfoPopupCurrent = survivorsPopup.SurvivorInfoPopupCurrent;
							if (SurvivorInfoPopupCurrent != null && SurvivorInfoPopupCurrent.gameObject.activeSelf)
							{
								SurvivorInfoPopupCurrent.OnClickClose();
							}
							survivorsPopup.OnClickClose();
						}
					}
				}
				else
				{
					var inventory = tabContainers[i].GetComponent<ResidenceBadgeInventoryTab>();
					var guildPopup = tabContainers[i].GetComponent<SocialPopupGuild>();
					//Craft
					if (i == 1 && i == index)
					{

					}
					//Badge reroll
					if (i == 2 && i == index && inventory != null)
					{
						inventory.SetBadgesList();
					}
					if (i == 5 && i == index)
					{
						//Workshop
						if (tabContainers[i].TryGetComponent<WorkshopPopup>(out var workshopPopup))
						{
							workshopPopup.gameObject.SetActive(true);
							workshopPopup.Open();
						}
					}
					if (i == 6 && i == index && guildPopup != null)
					{
						//Guild
						if (!GWTeamUtils.Instance.IsGuildLoaded)
						{
							if (!OfflineManager.Instance.IsPlayerLoaded)
							{
								DebugTWD.Log("Игрок не загружен");
								return;
							}
							StartCoroutine(OpenGuildTab(i, index, false));
							return;
						}
					}
					tabContainers[i].SetActive(i == index);
				}
			}
			StartCoroutine(DelayedScrollViewReset(index));
			PlayUITabAnimations(index);
			lastSelectedTabIndex = index;
		}
	}

	protected void OnShopClicked(UIButtonExtended button)
	{
		ShopPopup shopPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ShopPopup) as ShopPopup;
		if (shopPopup != null)
		{
			shopPopup.OpenForTab(!IsLoadDataManager ? 2 : 4);
		}
	}

	protected void OnTabChanged(UIButtonExtended toggle)
	{
		SetCurrentTab(toggleSet.GetSelectedIndex());
	}

	private IEnumerator DelayedScrollViewReset(int tabIndex)
	{
		yield return null;
		UIScrollView[] componentsInChildren = tabContainers[tabIndex].GetComponentsInChildren<UIScrollView>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ResetPosition();
		}
	}



	#region myparams
	public static ResidencePopup Instance { get; private set; }
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private int lastSelectedTabIndex;
	#endregion

	#region mycode
	public int GetSelectedIndex()
	{
		if (toggleSet != null)
		{
			return toggleSet.GetSelectedIndex();
		}
		return -1;
	}

	public IEnumerator WaitForPlayer()
	{
		float startTime = Time.realtimeSinceStartup;
		while (!OfflineManager.Instance.IsPlayerLoaded)
		{
			if (Time.realtimeSinceStartup - startTime > 20f)
			{
				DebugTWD.LogWarning("Can't load player");
				yield break;
			}
			yield return null;
		}
		DataManager.Instance.Player.Changed += OnPlayerChanged;
		UIEvent.OnUIEvent += OnUIEvent;
		UIButtonToggleSet uIButtonToggleSet = toggleSet;
		uIButtonToggleSet.OnChangeDelegate = (UIButtonToggleSet.OnTabsChangeDelegate)Delegate.Combine(uIButtonToggleSet.OnChangeDelegate, new UIButtonToggleSet.OnTabsChangeDelegate(OnTabChanged));
		if (shopButton != null)
		{
			shopButton.SetClickCallback(OnShopClicked);
		}
		if (DataManager.Instance.Player.Camp.GetBuilding("Residence") is ResidenceBuildingModel)
		{
			UpdateComponentInventory();
		}
		DebugTWD.Log("Residence Popup Awake");
	}

	public void UpadateEquipUIData()
	{
		var info = DataManager.Instance.SurvivorManagementPopUp.SurvivorInfoPopupCurrent;
		if (info != null)
		{
			info.UpdateUI();
			DataManager.Instance.SurvivorManagementPopUp.UpdateUI();
			DebugTWD.Log("Update All Equipment");
		}
	}

	public IEnumerator OpenGuildTab(int i, int index, bool hideMode)
	{
		yield return DataManager.Instance.Player;

		SignalRClient.Instance?.SetLoadingStatus(true);

		if (GWTeamUtils.Instance.IsOpponentGuild)
		{
			if (GWTeamUtils.Instance.OpponentGuilModel == null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading)?.Open();

				if (string.IsNullOrEmpty(GWTeamUtils.Instance.CustomGuildID))
				{
					GWTeamUtils.Instance.OpponentGuildID = DataManager.Instance.Player.GuildWarModel.CurrentBattle.EnemyGuildData.GroupId;
				}
				else
				{
					GWTeamUtils.Instance.OpponentGuildID = GWTeamUtils.Instance.GuildModel.GuildWarModel.CurrentBattle.EnemyGuildData.GroupId;
				}

				GWTeamUtils.Instance.LoadGuildData(true);
				yield return new WaitUntil(() => GWTeamUtils.Instance.IsGuildLoaded);

				if (GWTeamUtils.Instance.OpponentGuilModel == null)
				{
					GWTeamUtils.Instance.IsGuildLoaded = false;
					GWTeamUtils.Instance.ChangeGuildModelBase(false);
					yield break;
				}
			}
			GWTeamUtils.Instance.GuildName.text = GWTeamUtils.Instance.OpponentGuilModel?.Name ?? GWTeamUtils.Instance.GuildModel.GuildWarModel.CurrentBattle.EnemyGuildData.GuildName;
		}
		else
		{
			if (GWTeamUtils.Instance.GuildModel == null)
			{
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();

				if (string.IsNullOrEmpty(GWTeamUtils.Instance.CustomGuildID))
				{
					GWTeamUtils.Instance.GuildID = DataManager.Instance.Player.GuildId;
				}
				else
				{
					GWTeamUtils.Instance.GuildID = GWTeamUtils.Instance.CustomGuildID;
				}
				GWTeamUtils.Instance.LoadGuildData(false);
				yield return new WaitUntil(() => GWTeamUtils.Instance.IsGuildLoaded);
				SignalRClient.Instance?.SetLoadingStatus(false);
			}

			if (GWTeamUtils.Instance.GuildModel == null)
			{
				if (!hideMode) lastSelectedTabIndex = index;
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
				SignalRClient.Instance?.SetLoadingStatus(false);
				yield break;
			}

			CraftSettings.Instance.MissionHubButtonEnable(true);
			GWTeamUtils.Instance.GuildName.text = GWTeamUtils.Instance.GuildModel.Name;
		}

		if (!hideMode)
		{
			if (!tabContainers[i].activeSelf)
			{
				tabContainers[i].SetActive(i == index);
			}
			StartCoroutine(DelayedScrollViewReset(index));
			PlayUITabAnimations(index);
			lastSelectedTabIndex = index;
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
	}

	private void PlayUITabAnimations(int openedTab)
	{
		for (int i = 0; i < toggleSet.GetUIButtonToggleList.Length; i++)
		{
			TweenerPlayer component = toggleSet.GetUIButtonToggleList[i].GetComponent<TweenerPlayer>();
			var label = component.transform.GetChild(0).GetComponent<UILabel>();
			if (i == openedTab)
			{
				//раскрыть
				label.overflowMethod = UILabel.Overflow.ShrinkContent;
				component.PlayGroup(11, lastSelectedTabIndex == 0);
			}
			else if (i == lastSelectedTabIndex)
			{
				label.overflowMethod = UILabel.Overflow.ClampContent;
				component.PlayGroup(10, lastSelectedTabIndex == 0);
			}
		}
	}

	public void ActivateTab(int index, bool isActivate)
	{
		tabContainers[index].SetActive(isActivate);
	}

	private void Awake()
	{
		if (IsLoadDataManager)
		{
			if (Instance != null)
			{
				Debug.LogError("Multiple ResidencePopup!");
				Destroy(this.gameObject);
				return;
			}
			Instance = this;
		}
	}
	#endregion
}
