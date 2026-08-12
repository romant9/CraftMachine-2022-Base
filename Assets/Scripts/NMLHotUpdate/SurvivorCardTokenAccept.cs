using System;
using System.Linq;
using TWDModel;
using UnityEngine;

public class SurvivorCardTokenAccept : MonoBehaviourExtended
{
	[SerializeField]
	private UIButtonWithLabel buttonAcceptSurvivor;

	[SerializeField]
	private UIButtonWithLabel buttonAcceptClassTokens;

	public UISprite classIconSprite;

	[NonSerialized]
	public int LootEntryIndex = -1;

	private void Awake()
	{
		DebugIdString = "SurviorCardTokenAccept";
	}

	public void UpdateWithModel(SurvivorModel survivorModel, bool selected)
	{
		if (IsNotNull(survivorModel, "UpdateWithModel"))
		{
			if (buttonAcceptSurvivor != null)
			{
				if (IsLoadDataManager)
				{
					DebugTWD.LogMycode("if (IsLoadDataManager)");
					buttonAcceptSurvivor.gameObject.SetActive(false);
				}
				else
				{
					buttonAcceptSurvivor.isEnabled = selected && GameManager.Instance.playerModel.SurvivorContainer.CanAddSurvivor();
					buttonAcceptSurvivor.SetClickCallback(OnClickAcceptSurvivor);
				}
			}
			if (buttonAcceptClassTokens != null)
			{
				if (!IsLoadDataManager) buttonAcceptClassTokens.isEnabled = selected;
				tokenAmount = survivorModel.DemoteTokens.ToString();
				buttonAcceptClassTokens.SetContentToLabelTwo(survivorModel.DemoteTokens.ToString());
				buttonAcceptClassTokens.SetClickCallback(OnClickAcceptTokens);

				if (CallCraft.Instance.IsAccepted)
				{
					buttonAcceptClassTokens.gameObject.SetActive(false);
				}
				else
				{
					buttonAcceptClassTokens.gameObject.SetActive(true);
				}
			}
			if (classIconSprite != null)
			{
				classIconSprite.spriteName = HelpersGfx.GetCurrencyIconName(SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivorModel));
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (buttonAcceptClassTokens != null)
		{
			buttonAcceptClassTokens.Clear();
		}
		if (buttonAcceptSurvivor != null)
		{
			buttonAcceptSurvivor.Clear();
		}
	}

	private void OnClickAcceptSurvivor(UIButtonExtended button)
	{
		EventManager.NotifyClick("SurvivorAccept");
		UIEvent.Send("OnAcceptSelectedLootEntrySurvivor", LootEntryIndex);
	}

	private void OnClickAcceptTokens(UIButtonExtended button)
	{
		if (!IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			UIEvent.Send("OnAcceptSelectedLootEntryTokens", LootEntryIndex);
			return;
		}
		SelectSurvivorsPopup selectSurvivorsPopup = CallCraft.Instance._SelectSurvivorsPopup;
		if (selectSurvivorsPopup != null)
		{
			CallCraft.Instance.CurrentCall.IsAccepted = true;

			selectSurvivorsPopup.OnClickAcceptSelectedLoot(LootEntryIndex);

			CallGridItem CurrentCallGridItem = CallCraft.Instance.CurrentCall;
			CurrentCallGridItem.AcceptIndexesGroups.First().gameObject.SetActive(true);
			CurrentCallGridItem.tokenValues[LootEntryIndex].transform.parent.gameObject.SetActive(true);
			CurrentCallGridItem.AcceptIndexes.First()[LootEntryIndex].Set(true);

			LootEntry loot = selectSurvivorsPopup.CardsList[LootEntryIndex].GetLootEntry();
			CurrentCallGridItem.LootEntryList.Add(loot);

			HelpersUI.SetContentToLabel(CurrentCallGridItem.tokenValues[LootEntryIndex], tokenAmount);
			HelpersUI.SetSprite(CurrentCallGridItem.tokenSprites[LootEntryIndex], classIconSprite.spriteName);

			CallCraft.Instance.IsCallFinish = true;
		}
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public string tokenAmount { get; set; }
	#endregion

	#region mycode
	public void OnClickAcceptTokens()
	{
		var selectSurvivorsPopup = CallCraft.Instance._SelectSurvivorsPopup;
		if (selectSurvivorsPopup != null)
		{
			selectSurvivorsPopup.OnClickAcceptSelectedLoot(LootEntryIndex);

			CallGridItem CurrentCallGridItem = CallCraft.Instance.CurrentCall;
			CurrentCallGridItem.AcceptIndexesGroups.First().gameObject.SetActive(true);
			CurrentCallGridItem.tokenValues[LootEntryIndex].transform.parent.gameObject.SetActive(true);
			CurrentCallGridItem.AcceptIndexes.First()[LootEntryIndex].Set(true);

			LootEntry loot = selectSurvivorsPopup.CardsList[LootEntryIndex].GetLootEntry();
			CurrentCallGridItem.LootEntryList.Add(loot);

			HelpersUI.SetContentToLabel(CurrentCallGridItem.tokenValues[LootEntryIndex], tokenAmount);
			HelpersUI.SetSprite(CurrentCallGridItem.tokenSprites[LootEntryIndex], classIconSprite.spriteName);
		}
	}
	#endregion
}
