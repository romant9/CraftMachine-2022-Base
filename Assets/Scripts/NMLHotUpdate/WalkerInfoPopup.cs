using System.Collections.Generic;
using BaseModel;
using Client.Tweener;
using TWDModel;
using UnityEngine;

public class WalkerInfoPopup : HUDElement
{
	public enum WalkerInfoPopupStates
	{
		Info = 0,
		InfoHide = 1,
		InfoShow = 2,
		ShowUpgrade = 3,
		ShowUpgradeFromCamp = 4
	}

	[Header("Walker Info")]
	[SerializeField]
	private UILabel walkerNameLabel;

	[SerializeField]
	private WalkerInfoOptionsPanel infoOptions;

	[SerializeField]
	private TraitsPanel surviviorTraitsInfo;

	[SerializeField]
	private WalkerUpgradeView upgradeView;

	[SerializeField]
	private UISprite classIconSprite;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private WalkerStatisticsPanel statisticsPanel;

	public WalkerInfoPopupStates currentState;

	private WalkerInfoPopupStates newState;

	private Dictionary<string, TweenAnchors> allInfoTweens = new Dictionary<string, TweenAnchors>();

	private int countCompleted;

	public ActorModel walkerModel { get; private set; }

	public OutpostWalkerModel outpostWalkerModel { get; private set; }

	public override void UpdateUI()
	{
		if (outpostWalkerModel == null)
		{
			Debug.LogError("Cant Update: No survivorModel!");
			return;
		}
		if (classIconSprite != null)
		{
			classIconSprite.spriteName = "Ui_Icon_Class_" + outpostWalkerModel.ActorDefinition.Class.ToString();
		}
		if (currentState == WalkerInfoPopupStates.ShowUpgrade || currentState == WalkerInfoPopupStates.ShowUpgradeFromCamp)
		{
			if (upgradeView != null)
			{
				upgradeView.Show();
				upgradeView.SetInfo(outpostWalkerModel);
				setActiveMainInfoPanels(value: false);
				if (currentState == WalkerInfoPopupStates.ShowUpgradeFromCamp)
				{
					SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.OpenForSelected(outpostWalkerModel.Id, outpostWalkerModel.IsLocked, FullscreenActorOverlay.BackgroundType.Walker);
					SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.SetToOffset();
				}
				SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.RequestShowUpgradeAnim();
				updateNamePosition();
			}
		}
		else if (currentState == WalkerInfoPopupStates.Info)
		{
			ResetWalkerPosition();
			setActiveMainInfoPanels(value: true);
			if (upgradeView != null)
			{
				upgradeView.gameObject.SetActive(value: false);
			}
			if (statisticsPanel != null)
			{
				findTween(statisticsPanel.gameObject);
				statisticsPanel.SetInfo(outpostWalkerModel);
			}
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.OpenForSelected(outpostWalkerModel.Id, outpostWalkerModel.IsLocked, FullscreenActorOverlay.BackgroundType.Walker);
			updateNamePosition();
			if (!(infoOptions != null))
			{
				return;
			}
			findTween(infoOptions.gameObject);
			infoOptions.Show();
			string text = null;
			CageDefinition nextUpgradeDefinition = outpostWalkerModel.NextUpgradeDefinition;
			CageBuildingModel cageBuildingModel = GameManager.Instance.playerModel.Camp.GetBuilding("Cage") as CageBuildingModel;
			bool flag = cageBuildingModel?.IsUpgrading ?? false;
			bool flag2 = cageBuildingModel != null && (cageBuildingModel.UpgradingWalker != null || cageBuildingModel.UpgradedUnseenModel != null);
			bool canUpgrade = outpostWalkerModel.CanUpgrade;
			if (!flag2 && !flag && canUpgrade)
			{
				infoOptions.showPayTrainButtons();
				infoOptions.SetPayButton(LocalizationManager.GetText("Popup.UpgradeWalker.Button.LevelUp"), outpostWalkerModel.GetUpgradeCashier(instantUpgrade: false), outpostWalkerModel.UpgradeTime);
				infoOptions.SetInstantPayButton(outpostWalkerModel.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: true), LocalizationManager.GetText("Popup.DefaultPopup.Button.Instant"));
			}
			else if (outpostWalkerModel.IsLocked)
			{
				text = LocalizationManager.GetText("Popup.UpgradeWalker.CompleteEpisode{Name}", HelpersLocalization.GetEpisodeTitle(GameManager.Instance.playerModel.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(outpostWalkerModel.CurrentUpgradeDefinition.EpisodeLock)));
				infoOptions.showPayTrainButtons(showPayButton: false);
				infoOptions.SetInstantPayButton(outpostWalkerModel.GetUnlockCashier(), LocalizationManager.GetText("Button.Unlock"));
			}
			else
			{
				if (flag2)
				{
					text = LocalizationManager.GetText("Popup.UpgradeWalker.UpgradeWalkerUpgrading");
				}
				else if (!canUpgrade)
				{
					text = ((!outpostWalkerModel.HasReachedMaxLevel) ? LocalizationManager.GetText("Popup.UpgradeWalker.CageLevelRequired{Level}", nextUpgradeDefinition.DependencyLevelRequired) : LocalizationManager.GetText("Popup.UpgradeWalker.TrainingComplete"));
				}
				else if (flag)
				{
					text = LocalizationManager.GetText("Popup.UpgradeWalker.CageUpgrading");
				}
				infoOptions.hidePayTrainButtons();
			}
			infoOptions.showMessage(text);
			infoOptions.SetBackButton(!string.IsNullOrEmpty(text));
			bool flag3 = !outpostWalkerModel.IsLocked && outpostWalkerModel.CanUpgradeAmount;
			if (flag3)
			{
				infoOptions.SetUpgradeAmountPayButton(outpostWalkerModel.GetUpgradeAmountCashier());
				infoOptions.showUpgradeAmountLocked(null);
			}
			if (flag3 || outpostWalkerModel.IsLocked || outpostWalkerModel.Level == 0)
			{
				infoOptions.showUpgradeAmountLocked(null);
			}
			else if (outpostWalkerModel.Amount >= 1 && (outpostWalkerModel.NextUpgradeAmountDefinition == null || outpostWalkerModel.NextUpgradeAmountDefinition.AmountDependencyLevelRequired == 0))
			{
				infoOptions.showUpgradeAmountLocked(LocalizationManager.GetText("Popup.UpgradeWalker.MaxAmountReached"));
			}
			else
			{
				infoOptions.showUpgradeAmountLocked(LocalizationManager.GetText("Popup.UpgradeWalker.CageLevelRequiredAmount{Level}", outpostWalkerModel.NextUpgradeAmountDefinition.AmountDependencyLevelRequired));
			}
			infoOptions.showUpgradeAmountButton(flag3);
		}
		else if (currentState == WalkerInfoPopupStates.InfoHide)
		{
			HideTweenAll();
		}
		else if (currentState == WalkerInfoPopupStates.InfoShow)
		{
			if (upgradeView != null)
			{
				upgradeView.Hide();
			}
			ShowTweenAll();
		}
	}

	private void OnDestroy()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		outpostWalkerModel = model as OutpostWalkerModel;
		newState = currentState;
		UIEvent.OnUIEvent += OnUIEvent;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_trainingground");
		TutorialView.Instance.UpdateSuggestion();
		UpdateUI();
	}

	private void setActiveMainInfoPanels(bool value)
	{
		if (statisticsPanel != null)
		{
			statisticsPanel.gameObject.SetActive(value);
		}
		if (infoOptions != null)
		{
			infoOptions.gameObject.SetActive(value);
		}
	}

	private void SetNamePanel()
	{
		walkerNameLabel.text = HelpersLocalization.GetActorClassName(Faction.Walker.ToString(), outpostWalkerModel.ActorDefinition.Class);
		descriptionLabel.text = HelpersLocalization.GetWalkerCageDescription(outpostWalkerModel.ActorDefinition.Class);
	}

	private void updateNamePosition()
	{
		if (walkerNameLabel != null && SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.NameTargets.Length > 1)
		{
			walkerNameLabel.transform.OverlayPosition(SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.NameTargets[1].transform.position, SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.gameObject.GetComponentInChildren<Camera>());
			Vector3 localPosition = walkerNameLabel.transform.localPosition;
			localPosition.z = 0f;
			walkerNameLabel.transform.transform.localPosition = localPosition;
			SetNamePanel();
		}
	}

	private void HideTweenAll()
	{
		if (newState == WalkerInfoPopupStates.ShowUpgrade)
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.AnimateOffset();
		}
		setActiveMainInfoPanels(value: true);
		foreach (KeyValuePair<string, TweenAnchors> allInfoTween in allInfoTweens)
		{
			if (allInfoTween.Value != null)
			{
				allInfoTween.Value.PlayForward();
				allInfoTween.Value.SetCallback(hideComplete);
			}
		}
	}

	private void ShowTweenAll()
	{
		if (newState == WalkerInfoPopupStates.Info)
		{
			SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.AnimateBack();
		}
		setActiveMainInfoPanels(value: true);
		foreach (KeyValuePair<string, TweenAnchors> allInfoTween in allInfoTweens)
		{
			if (allInfoTween.Value != null)
			{
				allInfoTween.Value.PlayBackwards();
				allInfoTween.Value.SetCallback(showComplete);
			}
		}
	}

	private void ResetWalkerPosition()
	{
		SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.ResetPosition();
	}

	private void hideComplete()
	{
		if (countCompletedCheck())
		{
			currentState = newState;
			UpdateUI();
		}
	}

	private void showComplete()
	{
		if (countCompletedCheck())
		{
			currentState = newState;
			UpdateUI();
		}
	}

	private bool countCompletedCheck()
	{
		countCompleted++;
		if (countCompleted >= allInfoTweens.Count)
		{
			countCompleted = 0;
			return true;
		}
		return false;
	}

	private void findTween(GameObject obj)
	{
		TweenAnchors tweenAnchors = null;
		if (allInfoTweens != null && obj != null)
		{
			tweenAnchors = obj.GetComponent<TweenAnchors>();
			if (!allInfoTweens.ContainsKey(obj.name) && tweenAnchors != null)
			{
				allInfoTweens.Add(obj.name, tweenAnchors);
			}
		}
	}

	private void OnUIEvent(string type, object parameter)
	{
	}

	public void trainClicked()
	{
		if (outpostWalkerModel != null && outpostWalkerModel.CanUpgrade)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeCageWalkerCommand(outpostWalkerModel)
			{
				Instant = false,
				Cashier = outpostWalkerModel.GetUpgradeCashier(instantUpgrade: false)
			});
			EventManager.NotifyClick("Buy");
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_accept");
		UIEvent.Send("OnWalkerUpgraded", outpostWalkerModel);
		Close();
	}

	public void trainInstantClicked()
	{
		if (outpostWalkerModel.IsLocked)
		{
			UnlockClicked();
		}
		else if (outpostWalkerModel != null && outpostWalkerModel.CanUpgrade)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeCageWalkerCommand(outpostWalkerModel)
			{
				Instant = true,
				Cashier = outpostWalkerModel.GetUpgradeCashier(instantUpgrade: true, addInitialSurvivorPoints: true)
			}, InstantUpgradeCallback);
		}
	}

	public void InstantUpgradeCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			TWDModelResult num = Helpers.ExecuteCommand(new UpgradedModelViewedCommand(GameManager.Instance.playerModel.Camp.GetBuilding("Cage") as CageBuildingModel));
			UpdateUI();
			if (num == TWDModelResult.OK)
			{
				currentState = WalkerInfoPopupStates.InfoHide;
				newState = WalkerInfoPopupStates.ShowUpgrade;
			}
			UpdateUI();
			UIEvent.Send("OnSurvivorInstantUpgraded", outpostWalkerModel);
		}
	}

	public void UpgradeAmountClicked()
	{
		if (outpostWalkerModel != null && outpostWalkerModel.CanUpgradeAmount)
		{
			ConsumeCurrencyCommandUtils.Execute(new UpgradeCageWalkerAmountCommand(outpostWalkerModel)
			{
				Cashier = outpostWalkerModel.GetUpgradeAmountCashier()
			}, UpgradeAmountCallback);
		}
	}

	private void UpgradeAmountCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			UpdateUI();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/more_walkers");
		}
	}

	public void UnlockClicked()
	{
		if (outpostWalkerModel != null && outpostWalkerModel.IsLocked)
		{
			ConsumeCurrencyCommandUtils.Execute(new UnlockCageWalkerCommand(outpostWalkerModel)
			{
				Cashier = outpostWalkerModel.GetUnlockCashier()
			}, UnlockCallback);
		}
	}

	private void UnlockCallback(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			UpdateUI();
		}
	}

	private void OnPopupsCancel()
	{
		UpdateUI();
	}

	public override void OnClickClose()
	{
		if (currentState == WalkerInfoPopupStates.ShowUpgrade)
		{
			currentState = WalkerInfoPopupStates.InfoShow;
			newState = WalkerInfoPopupStates.Info;
			UpdateUI();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_upgrade_close");
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
			UIEvent.Send("SurvivorListRefreshed");
			base.OnClickClose();
		}
	}

	public override void Close()
	{
		EventManager.NotifyClick("Close");
		EventManager.NotifyClick("Back");
		SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.close();
		UIEvent.OnUIEvent -= OnUIEvent;
		currentState = WalkerInfoPopupStates.Info;
		newState = WalkerInfoPopupStates.Info;
		UIEvent.Send("OnSurvivorInfoClosed");
		base.Close();
	}
}
