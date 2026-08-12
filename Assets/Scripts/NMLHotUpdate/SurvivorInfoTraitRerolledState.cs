using TWDModel;

public class SurvivorInfoTraitRerolledState : SurvivorInfoStateBase
{
	public override void Init()
	{
		base.Init();
		CurrentState = States.SurvivorTraitRerolled;
	}

	public override void UpdateUI()
	{
		Helpers.GameObjectSetActive(base.SurvivorStatistics, value: true);
		Helpers.GameObjectSetActive(base.SurvivorTraitsList.gameObject, value: true);
		UpdtateTraitRerollPanel();
	}

	public override void Enter()
	{
		base.Enter();
		PlayAnchorTween(base.SurvivorStatistics, TweenAnchorId.Hide);
		PlayAnchorTween(base.SurvivorRightSidePanel, TweenAnchorId.Hide);
	}

	private void UpdtateTraitRerollPanel()
	{
		if (Helpers.GameObjectSetActive(base.TraitRerollPanel, value: true))
		{
			base.TraitRerollPanel.UpdateWith(base.SurvivorModel, new UIButtonExtended.OnClickCallback[4] { OnTrait1Chosen, OnTrait2Chosen, OnOldTraitChosen, OnOkClicked });
		}
	}

	private void OnTrait1Chosen(UIButtonExtended button)
	{
		ChooseTrait(0);
	}

	private void OnTrait2Chosen(UIButtonExtended button)
	{
		ChooseTrait(1);
	}

	private void OnOldTraitChosen(UIButtonExtended button)
	{
		ChooseTrait(-1);
	}

	private void ChooseTrait(int choice)
	{
		if (OfflineManager.IsLoadDataManager || OfflineManager.IsFakeExecuteCommands)
		{
			if ((choice == -1 || choice == 0 || choice == 1) && SurvivorModel != null && !string.IsNullOrEmpty(SurvivorModel.TraitToBeRerolledCandidate) && SurvivorModel.RandomTraitsFromReroll != null && SurvivorModel.RandomTraitsFromReroll.Count == 2)
			{
				DebugTWD.Log("SurvivorModel.TraitToBeRerolledCandidate: " + SurvivorModel.TraitToBeRerolledCandidate);

				string text = (choice == -1) ? SurvivorModel.TraitToBeRerolledCandidate : SurvivorModel.RandomTraitsFromReroll[choice];
				TWDModelResult tWDModelResult = SurvivorModel.ChooseRerolledTrait(choice) ? TWDModelResult.OK : TWDModelResult.Error;
				if (tWDModelResult == TWDModelResult.OK)
				{
					if (choice != -1)
					{
						TraitRerollPanel.TraitChosen(choice);
						SetState(States.SurvivorOverview);
					}
					else
					{
						Cashier cashier = SurvivorModel.RefundTokens(text);
						DebugTWD.Log("SurvivorModel.RefundTokens: " + text + " : " + cashier.LastRefundAmounts.Count);

						ExitState();
					}
				}
			}
		}
		else
		{
			if (Helpers.ExecuteCommand(new ChooseRerolledTraitCommand(base.SurvivorModel, choice)) == TWDModelResult.OK)
			{
				if (choice != -1)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/survivor_upgrade_trait");
					if (!OfflineManager.IsNoEffects) SingularityMonoBehaviour<FullscreenActorOverlay>.Instance.RequestShowUpgradeAnim();
					base.TraitRerollPanel.TraitChosen(choice);
				}
				else
				{
					ExitState();
				}
			}
		}
	}

	protected virtual void OnOkClicked(UIButtonExtended button)
	{
		ExitState();
	}

	private void ExitState()
	{
		base.TraitRerollPanel.ClearCallbacks(includeOkButton: true);
		if (OfflineManager.IsLoadDataManager) TraitRerollPanel.gameObject.SetActive(false);

		SetState(States.SurvivorOverview);
	}
}
