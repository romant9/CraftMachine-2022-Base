using System;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RecycleWeaponConfirmPopup : HUDElement
{
	[SerializeField]
	[Header("Top Section")]
	private UILabel titleLabel;

	[SerializeField]
	[Header("Confirm Info")]
	private UILabel confirmDescLabel;

	[Header("Buttons")]
	[SerializeField]
	private UIButton cancelButton;

	[SerializeField]
	private UIButton confirmButton;

	[SerializeField]
	private UIButtonWithLabel confirmButtonTime;

	[SerializeField]
	private GameObject EntryContainer;

	private RecycleWeaponActivityModel _activityModel;

	private Action _onConfirm;

	private int _countdownSeconds;

	private const int ConfirmCountdownSeconds = 5;

	private void Awake()
	{
		cancelButton.onClick.Add(new EventDelegate(OnClickCancel));
		confirmButton.onClick.Add(new EventDelegate(OnClickConfirm));
	}

	private void OnDestroy()
	{
		CancelCountdown();
	}

	public void SetInfo(RecycleWeaponActivityModel activityModel, Action onConfirm, List<GameObject> EntryContainerRewardPicCans)
	{
		_activityModel = activityModel;
		_onConfirm = onConfirm;
		EntryContainer.RemoveAllChildren();
		foreach (GameObject EntryContainerRewardPicCan in EntryContainerRewardPicCans)
		{
			EntryContainer.AddChild(EntryContainerRewardPicCan);
		}
		EntryContainer.GetComponent<UITable>()?.Reposition();
	}

	public override void Open()
	{
		base.Open();
		BuildTitle();
		BuildConfirmText();
		BuildButtons();
		if (!OfflineManager.IsLoadDataManager) StartCountdown();
	}

	private void BuildTitle()
	{
		if (_activityModel?.CurrentDefinition != null)
		{
			if (_activityModel.Type == 1)
			{
				HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText("RecycleBlueprints"));
			}
			else
			{
				HelpersUI.SetContentToLabel(titleLabel, LocalizationManager.GetText("RecycleWeapons"));
			}
		}
	}

	private void BuildConfirmText()
	{
		if (_activityModel?.CurrentDefinition != null)
		{
			if (_activityModel.Type == 1)
			{
				HelpersUI.SetContentToLabel(confirmDescLabel, LocalizationManager.GetText("RecycleBlueprints.AskConfirm"));
			}
			else
			{
				HelpersUI.SetContentToLabel(confirmDescLabel, LocalizationManager.GetText("RecycleWeapons.AskConfirm"));
			}
		}
	}

	private void BuildButtons()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			confirmButton.isEnabled = true;
			_countdownSeconds = 0;
		}
		else
		{
			confirmButton.isEnabled = false;
			_countdownSeconds = 5;
		}
		UpdateConfirmButtonText();
	}

	private void StartCountdown()
	{
		CancelCountdown();
		StartCoroutine(CountdownRoutine());
	}

	private IEnumerator CountdownRoutine()
	{
		for (_countdownSeconds = 5; _countdownSeconds > 0; _countdownSeconds--)
		{
			UpdateConfirmButtonText();
			yield return new WaitForSeconds(1f);
		}
		UpdateConfirmButtonText();
		confirmButton.isEnabled = true;
	}

	private void UpdateConfirmButtonText()
	{
		if (_countdownSeconds > 0)
		{
			Helpers.GameObjectSetActive(confirmButton, value: false);
			Helpers.GameObjectSetActive(confirmButtonTime, value: true);
			confirmButtonTime.SetContentToLabelOne($"{_countdownSeconds}s");
		}
		else
		{
			Helpers.GameObjectSetActive(confirmButton, value: true);
			Helpers.GameObjectSetActive(confirmButtonTime, value: false);
		}
	}

	private void CancelCountdown()
	{
		_countdownSeconds = 0;
		StopAllCoroutines();
	}

	private void OnClickCancel()
	{
		Close();
	}

	private void OnClickConfirm()
	{
		if (_countdownSeconds <= 0)
		{
			_onConfirm?.Invoke();
			Close();
		}
	}

	public override void Close()
	{
		CancelCountdown();
		base.Close();
	}
}
