using System;
using System.Collections;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class AccountDeletionPopup : HUDElement
{
	[SerializeField]
	private UIInput nameLabel;

	[SerializeField]
	private UIButton deleteAccountButton;

	[SerializeField]
	private UILabel nameInstruction;

	[SerializeField]
	private GameObject incorrectPlayerNameIndicator;

	[SerializeField]
	private GameObject textGuide;

	[SerializeField]
	private GameObject[] steps;

	private readonly int nameConfirmationStep = 1;

	private int currentStep;

	public override void Open()
	{
		base.Open();
		for (int i = 1; i < steps.Length; i++)
		{
			steps[i].SetActive(value: false);
		}
		steps[currentStep].SetActive(value: true);
		HelpersUI.SetButtonState(deleteAccountButton, UIButtonColor.State.Disabled);
		HelpersUI.SetContentToLabel(nameInstruction, LocalizationManager.GetText("Popup.DeleteAccount.ConfirmationPage.TypeAccountName{Name}", GameManager.Instance.playerModel.Name));
		Helpers.GameObjectSetActive(incorrectPlayerNameIndicator, value: false);
		nameLabel.OnSelected += OnInputSelected;
		nameLabel.value = string.Empty;
	}

	public override void OnClickClose()
	{
		nameLabel.OnSelected -= OnInputSelected;
		base.OnClickClose();
	}

	public void ProceedToNextStep()
	{
		steps[currentStep].SetActive(value: false);
		currentStep++;
		if (currentStep == nameConfirmationStep && string.IsNullOrEmpty(GameManager.Instance.playerModel.Name))
		{
			DeleteAccount();
		}
		else
		{
			steps[currentStep].SetActive(value: true);
		}
	}

	public void CheckName()
	{
		if (nameLabel.value == GameManager.Instance.playerModel.Name)
		{
			HelpersUI.SetButtonState(deleteAccountButton, UIButtonColor.State.Normal);
		}
		else
		{
			HelpersUI.SetButtonState(deleteAccountButton, UIButtonColor.State.Disabled);
		}
	}

	public void OnInputSelected(bool isSelected)
	{
		Helpers.GameObjectSetActive(incorrectPlayerNameIndicator, !isSelected && nameLabel.value != GameManager.Instance.playerModel.Name);
		Helpers.GameObjectSetActive(textGuide, !isSelected && string.IsNullOrEmpty(nameLabel.value));
	}

	public void DeleteAccount()
	{
		ProceedToNextStep();
		StartCoroutine(SendDataDeletionRequest());
	}

	public void ExitGame()
	{
		GameManager.Instance.ReloadGame();
	}

	private IEnumerator SendDataDeletionRequest()
	{
		Helpers.ExecuteCommand(new SetMarkedForDeletionCommand(marked: true));
		OnCloseCallback = (Callback)Delegate.Combine(OnCloseCallback, new Callback(ExitGame));
		while (SignalRClient.Instance.IsWaitingForResponse)
		{
			yield return null;
		}
		ProceedToNextStep();
	}
}
