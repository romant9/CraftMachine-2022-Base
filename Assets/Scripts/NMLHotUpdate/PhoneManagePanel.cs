using Client.Tweener;
using System;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class PhoneManagePanel : MonoBehaviourExtended
{
	[SerializeField]
	private UISprite BgSprite;

	[SerializeField]
	private UIButtonExtended Button;

	[SerializeField]
	private UILabel Label;

	[SerializeField]
	private UIButtonExtended BuySlotsButton;

	[SerializeField]
	private PayButton BuySlotsPayButton;

	[SerializeField]
	private UIWidget BuySlotsWidget;

	[SerializeField]
	private GameObject RerollContainer;

	[SerializeField]
	public UIButtonExtended RerollButton;

	[SerializeField]
	private UILabel RerollButtonLabel;

	[SerializeField]
	public UILabel RerollCountLabel;

	[SerializeField]
	[Tooltip("The tint color for label.")]
	private Color availableCurrencyColor = Color.white;

	[SerializeField]
	[Tooltip("The tint color for label when no more slots")]
	private Color unavailableCurrencyColor = new Color(0.511f, 0.129f, 0.027f, 1f);

	private Vector2 BgInitSize = Vector2.zero;

	private Tweener BgSizeTweener = new Tweener();

	private bool SlotsFull;

	[NonSerialized]
	public int RerollsLeft;

	public Cashier SlotsCashier { get; set; }

	private void Awake()
	{
		DebugIdString = "";
		if (BgSprite != null)
		{
			BgInitSize = BgSprite.localSize;
		}
		if (BuySlotsWidget != null)
		{
			BuySlotsWidget.alpha = 0f;
		}

		if (DeleteButton)
		{
			DebugTWD.LogMycode("if (DeleteButton)");
			DeleteButton.gameObject.SetActive(CallCraft.Instance.SelectedCall != null && NextButton.isEnabled);
		}
	}

	private void Update()
	{
		if (BgSizeTweener != null && BgSizeTweener.animating && BuySlotsWidget != null && BgSprite != null)
		{
			BgSizeTweener.update();
			BgSprite.height = (int)BgSizeTweener.progression.y;
			BuySlotsWidget.alpha = BgSizeTweener.progression.w;
		}
	}

	public void UpdateRerollButtonLabel()
	{
		string content = "";
		if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.PhoneCall != null)
		{
			content = ((!GameManager.Instance.playerModel.PhoneCall.IsAllLootLockedForReroll()) ? SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("PhoneManagePanel.RerollButton.Reroll") : SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("PhoneManagePanel.RerollButton.KeepAll"));
		}
		HelpersUI.SetContentToLabel(RerollButtonLabel, content);
	}

	public void UpdateUI()
	{
		string text = "";
		if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.SurvivorContainer != null)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			int level = Mathf.Min(playerModel.SurvivorContainer.SurvivorSlotsUpgradeLevel, playerModel.gameEconomyData.GetMaxSurvivorSlotsLevel());
			SurvivorContainerModel survivorContainer = GameManager.Instance.playerModel.SurvivorContainer;
			if (survivorContainer != null)
			{
				SlotsCashier = survivorContainer.GetPurchaseNextSlotsLevelCashier();
			}
			if (SlotsCashier != null && BuySlotsPayButton != null)
			{
				BuySlotsPayButton.UpdateUI(SlotsCashier);
			}
			if (playerModel.gameEconomyData.GetSurvivorSlotsData(level) != null)
			{
				SlotsFull = playerModel.SurvivorContainer.Survivors.Count >= playerModel.SurvivorContainer.SurvivorSlotsCount;
				text = playerModel.SurvivorContainer.Survivors.Count + "/" + playerModel.SurvivorContainer.SurvivorSlotsCount;
			}
			Helpers.GameObjectSetActive(RerollContainer, RerollsLeft > 0);

			if (AcceptButton && OfflineManager.IsLoadDataManager)
			{
				AcceptButton.isEnabled = RerollsLeft == 0;
				if (CallCraft.Instance.CurrentCallButton == null && CallCraft.Instance.SelectedCall != null)
				{
					CallCraft.Instance.CurrentCallButton = CallCraft.Instance.SelectedCall.CallButton;
				}

				if (AcceptButton.isEnabled && CallCraft.Instance.CurrentCallButton.PhoneCallDefinition.Type == PhoneCallDefinitionType.None)
				{
					AcceptButton.isEnabled = false;
				}
			}

			string content = "";
			UpdateRerollButtonLabel();
			if (RerollsLeft > 0)
			{
				content = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("PhoneManagePanel.RerollsLeft{Count}", RerollsLeft);
			}
			HelpersUI.SetContentToLabel(RerollCountLabel, content);
		}
		if (Label != null)
		{
			if (SlotsFull)
			{
				Label.color = unavailableCurrencyColor;
			}
			else
			{
				Label.color = availableCurrencyColor;
			}
			Label.text = text;
		}
	}

	public void EnableButtons()
	{
		Button.isEnabled = true;
		RerollButton.gameObject.SetActive(true);
		RerollButton.isEnabled = true;

		if (NextButton) NextButton.isEnabled = false;
		if (ReturnButton) ReturnButton.isEnabled = false;
	}

	public void DisableButtons()
	{
		Button.isEnabled = false;
		RerollButton.isEnabled = false;

		if (NextButton) NextButton.isEnabled = true;
		if (ReturnButton) ReturnButton.isEnabled = true;
	}

	public void Show(bool skipTween = false)
	{
		UpdateUI();
		base.gameObject.SetActive(value: true);
		TweenManager.PlayTweenGroup(base.gameObject, 5, forward: true, UpdateBuySlotsUIState, skipTween);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void SetClickRerollCallback(UIButtonExtended.OnClickCallback callback)
	{
		if (RerollButton != null)
		{
			RerollButton.SetClickCallback(callback);
		}
	}

	public void SetClickManageCallback(UIButtonExtended.OnClickCallback callback)
	{
		if (Button != null)
		{
			Button.SetClickCallback(callback);
		}
	}

	public void SetClickBuySlotsCallback(UIButtonExtended.OnClickCallback callback)
	{
		if (BuySlotsButton != null)
		{
			BuySlotsButton.SetClickCallback(callback);
		}
	}

	public override void Clear()
	{
		base.Clear();
		if ((bool)Button)
		{
			Button.Clear();
		}
		if ((bool)BuySlotsButton)
		{
			BuySlotsButton.Clear();
		}
	}

	public void UpdateBuySlotsUIState()
	{
		if (!(BuySlotsButton != null) || !(BuySlotsWidget != null) || !(BgSprite != null))
		{
			return;
		}
		if (SlotsFull && CanBuyMoreSlots())
		{
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_swoosh_2");
			}
			Vector4 vector = BgSprite.localSize;
			vector.w = BgSprite.alpha;
			float y = BgInitSize.y + BuySlotsWidget.localSize.y;
			Vector4 to = vector;
			to.y = y;
			to.w = 1f;
			BgSizeTweener = new Tweener();
			BgSizeTweener.easeFromTo(vector, to, 1f, EasingFunctions.BackEaseOut);
		}
		else
		{
			BgSizeTweener = new Tweener();
			BgSprite.height = (int)BgInitSize.y;
			BuySlotsWidget.alpha = 0f;
		}
	}

	private void BuyClicked(UIButtonExtended button)
	{
		DebugLog("Buy clicked");
	}

	private bool CanBuyMoreSlots()
	{
		return SlotsCashier != null;
	}


	#region myparams
	[SerializeField]
	private SelectSurvivorsPopup _SelectSurvivorsPopup;
	public UIButtonExtended AcceptButton;
	public UIButtonExtended NextButton;
	public UILabel NextButtonLabel;

	public UIButtonExtended ReturnButton;
	public UIButtonExtended DeleteButton;
	public UIButtonExtended CancelButton;

	public List<int> LootIndexes { get; set; }
	#endregion

	#region mycode
	public void Init()
	{
		DebugIdString = "";
		if (BgSprite != null)
		{
			BgInitSize = BgSprite.localSize;
		}
		if (BuySlotsWidget != null)
		{
			BuySlotsWidget.alpha = 0f;
		}

		LootIndexes = new List<int>();

		AcceptButton.SetClickCallback(OnClickAccept);

		var callType = DataManager.Instance.Player.PhoneCall.CallType;
		DebugTWD.LogWarning("CallType  " + callType);

		if (callType == PhoneCallDefinitionType.GuaranteedHero || callType == PhoneCallDefinitionType.None)
		{
			AcceptButton.gameObject.SetActive(true);
		}
		else
		{
			AcceptButton.gameObject.SetActive(false);
		}

		AcceptButton.isEnabled = false;
		NextButton.isEnabled = false;
		ReturnButton.isEnabled = false;

		DeleteButton.gameObject.SetActive(false);
	}

	private void OnClickAccept(UIButtonExtended button)
	{
		CallGridItem CurrentCallGridItem = CallCraft.Instance.CurrentCall;
		CurrentCallGridItem.IsAccepted = true;
		foreach (var index in LootIndexes)
		{
			//_SelectSurvivorsPopup.OnClickAcceptSelectedLoot(index);

			CurrentCallGridItem.tokenValues[index].transform.parent.gameObject.SetActive(true);

			var loot = _SelectSurvivorsPopup.CardsList[index].GetLootEntry();
			CurrentCallGridItem.LootEntryList.Add(loot);

			if (loot.RewardedCurrency == CurrencyType.None)
			{
				var survivor = _SelectSurvivorsPopup.CardsList[index].GetComponent<SurvivorCard>().Item;

				HelpersUI.SetContentToLabel(CurrentCallGridItem.tokenValues[index], survivor.DemoteTokens.ToString());
				HelpersUI.SetSprite(CurrentCallGridItem.tokenSprites[index], HelpersGfx.GetCurrencyIconName(SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivor)));
			}
			else
			{
				int RewardedAmount = HelpersUI.GetActualRewardValue(CallCraft.Instance.CurrentCallButton, loot.RewardedAmount);

				HelpersUI.SetContentToLabel(CurrentCallGridItem.tokenValues[index], RewardedAmount.ToString());
				HelpersUI.SetSprite(CurrentCallGridItem.tokenSprites[index], HelpersGfx.GetCurrencyIconName(loot.RewardedCurrency));
			}
		}
		button.isEnabled = false;

		var radioCount = DataManager.Instance.Player.GetCurrency(CurrencyType.Phone).Value;
		var currentPrice = CallCraft.Instance.InitCallData?.Price ?? 0;
		DebugTWD.Log("OnClickAccept " + currentPrice + " | " + radioCount);

		if (currentPrice > radioCount && !OfflineManager.IsFreeAll)
		{
			HelpersUI.SetButtonState(NextButton, UIButtonColor.State.Disabled);
		}
		ReturnButton.isEnabled = true;
		NextButton.isEnabled = true;
		CallCraft.Instance.IsCallFinish = true;
		CallCraft.Instance.CalculateHeroTokenQueue();
	}

	public void OnClickCallItem(bool isSelected, string nextButtonTextEn, string nextButtonTextRu)
	{
		HelpersUI.SetContentToLabel(RerollCountLabel, "");
		RerollButton.gameObject.SetActive(false);
		NextButton.isEnabled = true;
		DeleteButton.gameObject.SetActive(isSelected);
		DeleteButton.isEnabled = isSelected;
		_SelectSurvivorsPopup.ManagePanel.ReturnButton.isEnabled = true;
		var nextButtonLabelUpdater = _SelectSurvivorsPopup.ManagePanel.NextButtonLabel.GetComponent<LocalizationUIUpdater>();
		nextButtonLabelUpdater.EnCustomText = nextButtonTextEn;
		nextButtonLabelUpdater.RuCustomText = nextButtonTextRu;
		if (nextButtonLabelUpdater.gameObject.activeInHierarchy) nextButtonLabelUpdater.UpdateContent();
	}
	#endregion
}
