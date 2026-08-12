using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class OutpostInfo : MonoBehaviour
{
	[SerializeField]
	private GameObject FirstOutpostContainer;

	[SerializeField]
	private GameObject OutpostOptionsContainer;

	[SerializeField]
	private GameObject AttackOutpostContainer;

	[SerializeField]
	private GameObject OutpostCreatedContainer;

	[SerializeField]
	private GameObject ProgressContainer;

	[SerializeField]
	private SimpleWidgetGrid GridLayout;

	[SerializeField]
	private UILabel RankingScoreLabel;

	[SerializeField]
	private UILabel OutpostLevelLabel;

	[SerializeField]
	private UILabel ProductionAmountLabel;

	[SerializeField]
	private UILabel ProductionTimerLabel;

	[SerializeField]
	private UILabel ProductionHaltedTimerLabel;

	[SerializeField]
	private UISprite[] DefendersIconsArray;

	[SerializeField]
	private UILabel[] DefendersLevelArray;

	[SerializeField]
	private ButtonWithLabel OutpostCollectButton;

	[SerializeField]
	private ButtonWithLabel RepairButton;

	[SerializeField]
	private UIButton BuyCratesButton;

	[SerializeField]
	private PayButton BuyFirstMatchButton;

	[SerializeField]
	private GameObject ShieldContainer;

	[SerializeField]
	private UILabel ShieldTimerLabel;

	[SerializeField]
	private UISprite TierEmblem;

	[SerializeField]
	private UILabel TierName;

	[SerializeField]
	private UILabel TierCycleTime;

	private OutpostSeason season;

	private long timestamp;

	private bool hasCurrent;

	private BuildingModel outpostBuilding;

	public void UpdateUI()
	{
		bool flag = GameManager.Instance.playerModel.OutpostModel.StoredLevelModel != null;
		if (flag)
		{
			UpdateDefendersIconsAndLevel(GameManager.Instance.playerModel.SurvivorContainer.OutpostDefendingSurvivors);
			RankingScoreLabel.text = GameManager.Instance.playerModel.RankingScore.ToString();
			OutpostLevelLabel.text = GameManager.Instance.playerModel.OutpostWalkerPower.ToString();
		}
		if (FirstOutpostContainer != null)
		{
			FirstOutpostContainer.SetActive(!flag);
		}
		if (OutpostOptionsContainer != null)
		{
			OutpostOptionsContainer.SetActive(flag);
		}
		if (AttackOutpostContainer != null)
		{
			AttackOutpostContainer.SetActive(flag);
		}
		if (OutpostCreatedContainer != null)
		{
			OutpostCreatedContainer.SetActive(flag);
		}
		if (ProgressContainer != null)
		{
			ProgressContainer.SetActive(flag);
		}
		if (BuyFirstMatchButton != null)
		{
			BuyFirstMatchButton.UpdateUI(GameManager.Instance.playerModel.OutpostModel.GetNextMatchCashier());
		}
		if (TierEmblem != null)
		{
			string text = ((GameManager.Instance.playerModel.CurrentOutpostTier != null) ? GameManager.Instance.playerModel.CurrentOutpostTier.Id : "");
			if (!string.IsNullOrEmpty(text))
			{
				TierEmblem.spriteName = HelpersGfx.GetTierEmblemIconName(text);
				TierEmblem.gameObject.SetActive(value: true);
			}
			else
			{
				TierEmblem.gameObject.SetActive(value: false);
			}
		}
		if (TierName != null)
		{
			string textId = ((GameManager.Instance.playerModel.CurrentOutpostTier != null) ? GameManager.Instance.playerModel.CurrentOutpostTier.LocalizationKey : "OutpostTier.NotAvailable");
			TierName.text = LocalizationManager.GetText(textId);
		}
		if (!(TierCycleTime != null))
		{
			return;
		}
		season = GameManager.Instance.gameEconomyData.GetOutpostSeasonById(GameManager.Instance.playerModel.CurrentOutpostSeasonId);
		if (season == null)
		{
			season = GameManager.Instance.gameEconomyData.GetNextOutpostSeason(GameManager.Instance.playerModel.UtcTimeStamp);
			if (season != null)
			{
				timestamp = season.StartTimeMilliseconds;
			}
		}
		else
		{
			hasCurrent = true;
			timestamp = season.EndTimeMilliseconds;
		}
	}

	public void OnDisable()
	{
		if (OutpostCollectButton != null)
		{
			OutpostCollectButton.Clear();
		}
		outpostBuilding = null;
	}

	public void OnEnable()
	{
		if (OutpostCollectButton != null)
		{
			OutpostCollectButton.SetCallback(OnCollectClicked);
		}
		outpostBuilding = GameManager.Instance.playerModel.Camp.GetBuilding("Outpost");
		long shieldTimeMillisLeft = GameManager.Instance.playerModel.GetShieldTimeMillisLeft(GameManager.Instance.playerModel.UtcTimeStamp);
		if (shieldTimeMillisLeft > 0)
		{
			ShieldTimerLabel.text = Helpers.FormatTime(shieldTimeMillisLeft);
			ShieldContainer.SetActive(value: true);
			TweenManager.PlayTweenGroup(ShieldContainer, 0);
		}
		else if (ShieldContainer.activeSelf)
		{
			ShieldContainer.SetActive(value: true);
		}
		UpdateUI();
	}

	public void UpdateDefendersIconsAndLevel(List<SurvivorModel> defendersList)
	{
		if (defendersList == null || DefendersIconsArray == null || DefendersLevelArray == null || DefendersLevelArray.Length != DefendersIconsArray.Length)
		{
			return;
		}
		for (int i = 0; i < defendersList.Count; i++)
		{
			if (i < DefendersIconsArray.Length && DefendersIconsArray[i] != null && DefendersLevelArray[i] != null && defendersList[i] != null)
			{
				DefendersIconsArray[i].spriteName = HelpersGfx.GetSurvivorClassIconName(defendersList[i].SurvivorClass.ToString(), defendersList[i].SurvivorRarityLevel);
				DefendersLevelArray[i].text = defendersList[i].Level.ToString();
			}
		}
	}

	public void UpdateDefendersIconsAndLevel(List<SurvivorClass> classList, List<int> rarityList)
	{
		if (DefendersIconsArray == null || classList == null || rarityList == null || classList.Count != rarityList.Count)
		{
			return;
		}
		for (int i = 0; i < classList.Count; i++)
		{
			if (i < DefendersIconsArray.Length && DefendersIconsArray[i] != null)
			{
				DefendersIconsArray[i].spriteName = HelpersGfx.GetSurvivorClassIconName(classList[i].ToString(), rarityList[i]);
			}
		}
	}

	public void Update()
	{
		if (GameManager.Instance.playerModel.OutpostModel.StoredLevelModel == null)
		{
			return;
		}
		long shieldTimeMillisLeft = GameManager.Instance.playerModel.GetShieldTimeMillisLeft(GameManager.Instance.playerModel.UtcTimeStamp);
		if (shieldTimeMillisLeft > 0)
		{
			ShieldTimerLabel.text = Helpers.FormatTime(shieldTimeMillisLeft);
		}
		else if (ShieldContainer.activeSelf)
		{
			ShieldContainer.SetActive(value: true);
		}
		if (season != null)
		{
			long num = timestamp - GameManager.Instance.playerModel.UtcTimeStamp;
			if (num < 0)
			{
				num = 0L;
			}
			string text = (hasCurrent ? LocalizationManager.GetText("OutpostSeason.EndsIn{Time}", Helpers.FormatTimeNoZero(num)) : LocalizationManager.GetText("OutpostSeason.StartsIn{Time}", Helpers.FormatTimeNoZero(num)));
			TierCycleTime.text = text;
		}
		if (ProductionAmountLabel != null && outpostBuilding != null)
		{
			if (outpostBuilding.Producer.IsProductionHalted)
			{
				ProductionAmountLabel.gameObject.SetActive(value: false);
				ProductionHaltedTimerLabel.gameObject.transform.parent.gameObject.SetActive(value: true);
				ProductionHaltedTimerLabel.text = Helpers.FormatTime(outpostBuilding.Producer.ProductionHaltedTimer);
				RepairButton.gameObject.SetActive(value: true);
				BuyCratesButton.gameObject.SetActive(value: false);
			}
			else
			{
				ProductionAmountLabel.gameObject.SetActive(value: true);
				ProductionHaltedTimerLabel.gameObject.transform.parent.gameObject.SetActive(value: false);
				ProductionAmountLabel.text = outpostBuilding.Producer.Rate + "/" + LocalizationManager.GetText("Generic.Time.HourSmall");
				RepairButton.gameObject.SetActive(value: false);
				BuyCratesButton.gameObject.SetActive(value: true);
			}
		}
		if (OutpostCollectButton != null && outpostBuilding != null)
		{
			OutpostCollectButton.gameObject.SetActive(outpostBuilding != null && outpostBuilding.CanCollect);
		}
	}

	public void OnEditTeamClicked()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		TeamSelectionPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MapTeamSelection) as TeamSelectionPopup;
		obj.SurvivorType = SurvivorContainerModel.SurvivorType.Outpost;
		obj.SetUITypeOpenOnClose(UIType.OutpostPopup);
		obj.Open();
	}

	public void OnFindMatchClicked()
	{
		ShowAttackConfirmation();
	}

	private void StartFindMatch()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			ConsumeCurrencyCommandUtils.Execute(new PayForFirstMatchmaking
			{
				Cashier = GameManager.Instance.playerModel.OutpostModel.GetNextMatchCashier()
			}, PayForFirstMatchCallback);
		}
		else
		{
			Debug.LogError("Not connected to server - Cannot continue to matchmaking!");
		}
	}

	private void ShowAttackConfirmation()
	{
		long shieldTimeMillisLeft = GameManager.Instance.playerModel.GetShieldTimeMillisLeft(GameManager.Instance.playerModel.UtcTimeStamp);
		if (shieldTimeMillisLeft > 0)
		{
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.Outpost.ShieldWarning.Title"), LocalizationManager.GetText("Popup.Outpost.ShieldWarning.Body") + "\n" + LocalizationManager.GetText("Popup.Outpost.ShieldWarning.ShieldTimeRemaining", Helpers.FormatTimeWithoutSeconds(shieldTimeMillisLeft)), LocalizationManager.GetText("Button.Ok"), delegate
			{
				StartFindMatch();
			}, LocalizationManager.GetText("Button.Cancel"), delegate
			{
			});
		}
		else
		{
			StartFindMatch();
		}
	}

	private void PayForFirstMatchCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK && SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.MatchmakingPopup).Open();
		}
	}

	public void OnEditClicked()
	{
		if (OutpostEditManager.CallStartEditingOutpost())
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			OutpostEditPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopupEdit) as OutpostEditPopup;
			obj.State = OutpostManagementState.SliceEdit;
			obj.Open();
		}
	}

	public void OnCreateClicked()
	{
		if (GameManager.Instance.playerModel.OutpostModel.StoredLevelModel != null)
		{
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.Outpost.Overwrite.Title"), LocalizationManager.GetText("Popup.Outpost.Overwrite.Message"), "", CreateConfirmed);
		}
		else
		{
			CreateConfirmed();
		}
	}

	public void CreateConfirmed()
	{
		Helpers.ExecuteCommand(new SendMetricCommand(SendMetricCommand.MetricType.StartEditOutpost));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_outpost_create");
		SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
		OutpostEditPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopupEdit) as OutpostEditPopup;
		obj.State = OutpostManagementState.SelectBackground;
		obj.Open();
	}

	public void OnCollectClicked(ButtonBase origin)
	{
		if (outpostBuilding != null && Helpers.ExecuteCommand(new CollectBuildingCommand(outpostBuilding)) == TWDModelResult.OK)
		{
			OutpostCollectButton.Button.isEnabled = false;
			UpdateUI();
		}
	}

	public void OnRepairClicked()
	{
		if (outpostBuilding != null && Helpers.ExecuteCommand(new RepairProducerCommand(outpostBuilding.Producer)) == TWDModelResult.OK)
		{
			UpdateUI();
		}
	}

	public void OnBuyCratesClicked()
	{
		ShopPopupHelper.OpenWithIndex(3);
	}
}
