using System;
using System.Linq;
using System.Text;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using static SurvivorCard;

public class TokenCard : MonoBehaviourExtended
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UISprite iconSprite;

	[SerializeField]
	private UIButtonExtended mainButton;

	[SerializeField]
	private UIButtonExtended acceptButton;

	[SerializeField]
	private UITexture portraitTexture;

	[SerializeField]
	private GameObject acceptHiglight;

	[SerializeField]
	private UILabel ownedAmountLabel;

	[Header("Collected")]
	[SerializeField]
	private GameObject collectedParent;

	[SerializeField]
	private UILabel collectedNameLabel;

	[SerializeField]
	private UILabel collectedAmountLabel;

	[SerializeField]
	private UIButtonExtended unlockButton;

	[SerializeField]
	private TokenCardRerollLocking tokenCardRerollLocking;

	private UIWidget widgetCached;

	public LootEntry entryInternal;

	public int entryIndexInternal = -1;

	private string actorId = "";

	private ActorDefinition actorDefinition;

	private bool selected;

	private Cashier heroUnlockCashier;

	private Cashier upgradeCashier;

	private SurvivorModel survivorModel;

	[NonSerialized]
	public bool ForRerolling;

	private bool isHeroUnlocked;

	public UIWidget widget => widgetCached;

	private void Awake()
	{
		DebugIdString = "TokenCard";
		widgetCached = GetComponent<UIWidget>();
	}

	public void Init(LootEntry entry, int lootEntryIndex)
	{
		entryInternal = entry;
		entryIndexInternal = lootEntryIndex;
		if (entryInternal != null)
		{
			actorId = SurvivorToken.GetHeroId(entryInternal.RewardedCurrency);
			actorDefinition = GameManager.Instance.playerModel.gameEconomyData.GetActorDefinition(actorId);
			if (acceptButton != null)
			{
				acceptButton.SetClickCallback(OnClickAccept);
			}
			if (unlockButton != null)
			{
				unlockButton.SetClickCallback(OnClickUnlock);
			}
			heroUnlockCashier = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCashier(entryInternal.RewardedCurrency);
			isHeroUnlocked = GameManager.Instance.playerModel.SurvivorContainer.HasHero(actorId);
			if (isHeroUnlocked)
			{
				survivorModel = GameManager.Instance.playerModel.SurvivorContainer.GetHeroById(actorId);
				upgradeCashier = survivorModel.GetUpgradeTraitCashier();
			}
			if (OfflineManager.IsLoadDataManager)
			{
				DebugTWD.Log("Phone Call Type: " + GameManager.Instance.playerModel.PhoneCall.CallType);

				if (CallCraft.Instance.IsAccepted || ForRerolling)
				{
					acceptButton.gameObject.SetActive(false);
				}
				else
				{
					var callType = GameManager.Instance.playerModel.PhoneCall.CallType;
					acceptButton.gameObject.SetActive(callType != PhoneCallDefinitionType.GuaranteedHero);
				}
				TweenManager.PlayTweenGroup(acceptHiglight, 3, resetToEnd: true);
				Helpers.GameObjectSetActive(acceptHiglight, value: true);
				if (mainButton)
				{
					var btScale = mainButton.GetComponent<UIButtonScale>();
					if (!btScale)
					{
						btScale = mainButton.gameObject.AddComponent<UIButtonScale>();
						btScale.hover = Vector3.one;
						btScale.pressed = Vector3.one * .9f;
						btScale.tweenTarget = this.transform;
					}
				}
			}
		}
		else
		{
			DebugLogError("Could not init with NULL entry!");
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (acceptButton != null)
		{
			acceptButton.Clear();
		}
	}

	public UIButtonExtended GetButton()
	{
		return mainButton;
	}

	public UIButtonExtended GetAcceptButton()
	{
		return acceptButton;
	}

	public TokenCardRerollLocking GetTokenCardRerollLocking()
	{
		return tokenCardRerollLocking;
	}

	private void UpdateSurviorCardReroll()
	{
		if (Helpers.GameObjectSetActive(tokenCardRerollLocking, ForRerolling))
		{
			tokenCardRerollLocking.UpdateLockingButtons();
		}
	}

	public void UpdateUI()
	{
		if (!OfflineManager.IsLoadDataManager)
		{
			if (entryInternal != null && actorDefinition != null)
			{
				HelpersUI.SetContentToLabel(nameLabel, actorDefinition.Name);
				HelpersUI.SetContentToLabel(amountLabel, entryInternal.RewardedAmount.ToString());
			}
			if (acceptButton != null)
			{
				acceptButton.isEnabled = selected && !ForRerolling;
			}

			UpdateSurviorCardReroll();
			if (selected && entryInternal != null && heroUnlockCashier != null)
			{
				string content = "";
				int value = GameManager.Instance.playerModel.GetCurrency(entryInternal.RewardedCurrency).Value;
				int totalCost = heroUnlockCashier.GetTotalCost(entryInternal.RewardedCurrency);
				if (isHeroUnlocked)
				{
					if (survivorModel != null && upgradeCashier != null && (survivorModel.CanUpgradeSurvivorRarity() || survivorModel.CanUpgradeTraitRarity()))
					{
						int totalCost2 = upgradeCashier.GetTotalCost(entryInternal.RewardedCurrency);
						if (survivorModel.CanUpgradeSurvivorRarity())
						{
							content = LocalizationManager.GetText("TokenCard.ToNextPromote.Description{Owned}{Needed}", value, totalCost2);
						}
						else if (survivorModel.CanUpgradeTraitRarity())
						{
							content = LocalizationManager.GetText("TokenCard.ToNextUpgrade.Description{Owned}{Needed}", value, totalCost2);
						}
					}
					else
					{
						content = LocalizationManager.GetText("TokenCard.OwnedTokens.Description{Owned}", GameManager.Instance.playerModel.GetCurrency(entryInternal.RewardedCurrency).Value);
					}
				}
				else
				{
					content = LocalizationManager.GetText("TokenCard.ToHeroUnlock.Description{Owned}{Needed}", value, totalCost);
				}
				HelpersUI.SetContentToLabel(ownedAmountLabel, content);
			}
		}
		else
		{
			if (entryInternal != null && actorDefinition != null)
			{
				HelpersUI.SetContentToLabel(nameLabel, actorDefinition.Name);

				DebugTWD.Log("Update for " + actorDefinition.Name + ". Reward: " + entryInternal.RewardedCurrency + " | " + entryInternal.RewardedAmount, DebugType.Call);

				int RewardedAmount = HelpersUI.GetActualRewardValue(CallCraft.Instance.CurrentCallButton, entryInternal.RewardedAmount);

				HelpersUI.SetContentToLabel(amountLabel, RewardedAmount.ToString());

				UpdateTokenStats();
			}

			UpdateSurviorCardReroll();

			if (CallCraft.Instance.IsAccepted || ForRerolling)
			{
				acceptButton.gameObject.SetActive(false);
			}
			else
			{
				var callType = GameManager.Instance.playerModel.PhoneCall.CallType;
				acceptButton.gameObject.SetActive(callType != PhoneCallDefinitionType.GuaranteedHero);
			}
			TweenManager.PlayTweenGroup(acceptHiglight, 3, resetToEnd: true);
		}
		if (actorDefinition != null)
		{
			Texture portrait = PortraitManager.Instance.GetPortrait(PortraitRenderSource.fromActorDefinition(actorDefinition));
			if (portrait == null)
			{
				Helpers.GameObjectSetActive(portraitTexture, value: false);
				ModularCharacter modularCharacter = ActorView.GetPrefabForActor(actorDefinition.ID, actorDefinition.VisualAsset);
				if (modularCharacter == null)
				{
					modularCharacter = ActorView.SelectRandomPrefabForActorDefinition(actorDefinition.ID, actorDefinition.Gender);
				}
				PortraitManager.Instance.CreatePortrait(PortraitRenderSource.fromActorDefinition(actorDefinition), modularCharacter, OnMissingPortraitRendered);
			}
			else
			{
				Helpers.GameObjectSetActive(portraitTexture, value: true);
				portraitTexture.mainTexture = portrait;
			}
		}
		else
		{
			Debug.LogError("Attempt to UpdateUI for TokenCard with null actorDefinition.");
		}
		HelpersUI.SetSprite(iconSprite, HelpersGfx.GetCurrencyIconName(entryInternal.RewardedCurrency));
	}

	public void SetSeleted(bool value)
	{
		if (selected != value && !ForRerolling)
		{
			selected = value;
			if (!OfflineManager.IsLoadDataManager)
			{
				if (Helpers.GameObjectSetActive(acceptHiglight, value: true))
				{
					if (selected)
					{
						TweenManager.PlayTweenGroup(acceptHiglight, 3);
					}
					else
					{
						TweenManager.PlayTweenGroup(acceptHiglight, 4);
					}
				}
			}
		}
		UpdateUI();
	}

	public void CollectCard(bool allowShowingUnlockButton)
	{
		if (entryInternal == null)
		{
			return;
		}
		int value = GameManager.Instance.playerModel.GetCurrency(entryInternal.RewardedCurrency).Value;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(value);
		if (isHeroUnlocked)
		{
			if (survivorModel != null && upgradeCashier != null && (survivorModel.CanUpgradeSurvivorRarity() || survivorModel.CanUpgradeTraitRarity()))
			{
				stringBuilder.Append("/");
				stringBuilder.Append(upgradeCashier.GetTotalCost(entryInternal.RewardedCurrency));
			}
		}
		else
		{
			stringBuilder.Append("/");
			stringBuilder.Append(heroUnlockCashier.GetTotalCost(entryInternal.RewardedCurrency).ToString());
			if (allowShowingUnlockButton && GameManager.Instance.playerModel.GetCurrency(entryInternal.RewardedCurrency).Value >= heroUnlockCashier.GetTotalCost(entryInternal.RewardedCurrency))
			{
				Helpers.GameObjectSetActive(collectedParent, value: true);
				TweenManager.PlayTweenGroup(collectedParent, 3);
			}
		}
		if (actorDefinition != null)
		{
			HelpersUI.SetContentToLabel(collectedNameLabel, actorDefinition.Name);
		}
		HelpersUI.SetContentToLabel(collectedAmountLabel, stringBuilder.ToString());
	}

	private void OnClickAccept(UIButtonExtended button)
	{
		Helpers.GameObjectSetActive(acceptButton, value: false);
		Helpers.GameObjectSetActive(ownedAmountLabel, value: false);
		if (!OfflineManager.IsLoadDataManager)
		{
			if (selected)
			{
				UIEvent.Send("OnAcceptSelectedLootEntryTokens", entryIndexInternal);
			}
		}
		else
		{
			if (_SelectSurvivorsPopup != null)
			{
				CallCraft.Instance.CurrentCall.IsAccepted = true;

				_SelectSurvivorsPopup.OnClickAcceptSelectedLoot(entryIndexInternal);

				CallGridItem CurrentCallGridItem = CallCraft.Instance.CurrentCall;
				CurrentCallGridItem.tokenValues[entryIndexInternal].transform.parent.gameObject.SetActive(true);
				CurrentCallGridItem.AcceptIndexesGroups.First().gameObject.SetActive(true);
				CurrentCallGridItem.AcceptIndexes.First()[entryIndexInternal].Set(true);

				var loot = _SelectSurvivorsPopup.CardsList[entryIndexInternal].GetLootEntry();
				CurrentCallGridItem.LootEntryList.Add(loot);

				int RewardedAmount = HelpersUI.GetActualRewardValue(CallCraft.Instance.CurrentCallButton, entryInternal.RewardedAmount);

				HelpersUI.SetContentToLabel(CurrentCallGridItem.tokenValues[entryIndexInternal], RewardedAmount.ToString());
				HelpersUI.SetSprite(CurrentCallGridItem.tokenSprites[entryIndexInternal], iconSprite.spriteName);

				CallCraft.Instance.IsCallFinish = true;
			}
		}
	}

	private void OnClickUnlock(UIButtonExtended button)
	{
		Helpers.GameObjectSetActive(unlockButton, value: false);
		UIEvent.Send("OnTriggerHeroUnlock", actorDefinition);
	}

	private void OnMissingPortraitRendered(IPortraitRenderSource info)
	{
		if (portraitTexture != null && info != null && actorDefinition.ID == info.ActorDefinitionId)
		{
			portraitTexture.mainTexture = PortraitManager.Instance.GetPortrait(info);
			portraitTexture.gameObject.SetActive(value: true);
		}
	}



	#region myparams
	public SelectSurvivorsPopup _SelectSurvivorsPopup => CallCraft.Instance != null ? CallCraft.Instance._SelectSurvivorsPopup : null;
	#endregion

	#region mycode
	public void UpdateTokenStats()
	{
		string content = "";
		int value = GameManager.Instance.playerModel.GetCurrency(entryInternal.RewardedCurrency).Value;
		int totalCost = heroUnlockCashier.GetTotalCost(entryInternal.RewardedCurrency);
		if (isHeroUnlocked)
		{
			if (survivorModel != null && upgradeCashier != null && (survivorModel.CanUpgradeSurvivorRarity() || survivorModel.CanUpgradeTraitRarity()))
			{
				int totalCost2 = upgradeCashier.GetTotalCost(entryInternal.RewardedCurrency);
				if (survivorModel.CanUpgradeSurvivorRarity())
				{
					content = LocalizationManager.GetText("TokenCard.ToNextPromote.Description{Owned}{Needed}", value, totalCost2);
				}
				else if (survivorModel.CanUpgradeTraitRarity())
				{
					content = LocalizationManager.GetText("TokenCard.ToNextUpgrade.Description{Owned}{Needed}", value, totalCost2);
				}
			}
			else
			{
				content = LocalizationManager.GetText("TokenCard.OwnedTokens.Description{Owned}", value);
			}
		}
		else
		{
			content = LocalizationManager.GetText("TokenCard.ToHeroUnlock.Description{Owned}{Needed}", value, totalCost);
		}
		HelpersUI.SetContentToLabel(ownedAmountLabel, content);
	}

	public void OnClickAccept()
	{
		if (_SelectSurvivorsPopup != null)
		{
			_SelectSurvivorsPopup.OnClickAcceptSelectedLoot(entryIndexInternal);

			CallGridItem CurrentCallGridItem = CallCraft.Instance.CurrentCall;
			CurrentCallGridItem.tokenValues[entryIndexInternal].transform.parent.gameObject.SetActive(true);
			CurrentCallGridItem.AcceptIndexesGroups.First().gameObject.SetActive(true);
			CurrentCallGridItem.AcceptIndexes.First()[entryIndexInternal].Set(true);

			var loot = _SelectSurvivorsPopup.CardsList[entryIndexInternal].GetLootEntry();
			CurrentCallGridItem.LootEntryList.Add(loot);

			int RewardedAmount = HelpersUI.GetActualRewardValue(CallCraft.Instance.CurrentCallButton, entryInternal.RewardedAmount);

			HelpersUI.SetContentToLabel(CurrentCallGridItem.tokenValues[entryIndexInternal], RewardedAmount.ToString());
			HelpersUI.SetSprite(CurrentCallGridItem.tokenSprites[entryIndexInternal], iconSprite.spriteName);
		}
	}

	public void OnFullInfoClicked()
	{
		SurvivorModel item = survivorModel;
		var survivorInfoPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
		DebugTWD.LogMycode("if (IsLoadDataManager)");
		DataManager.Instance.SurvivorManagementPopUp.SurvivorInfoPopupCurrent = survivorInfoPopup;
		survivorInfoPopup.transform.localScale = Vector3.one * .9f;
		survivorInfoPopup.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverviewLimited;

		survivorInfoPopup.OpenForModel(item);
	}
	#endregion
}
