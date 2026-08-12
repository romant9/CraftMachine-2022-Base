using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class AvatarPopup : HUDElement
{
	[SerializeField]
	private AvatarListPanel avatarListPanel;

	[SerializeField]
	private UIButtonExtended applyButton;

	private AvatarBaseDefinition _avatarBaseDefinition;

	[SerializeField]
	[Tooltip("Label show when there is some info")]
	private UILabel infoLabel;

	[SerializeField]
	private UISprite infoSprite;

	[SerializeField]
	[Tooltip("Time to show in seconds")]
	private float timeToShow = 2f;

	[SerializeField]
	private Color errorColor;

	[SerializeField]
	private Color normalColor;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEventHandler;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEventHandler;
	}

	private void OnUIEventHandler(string type, object parameter)
	{
		if (type == "OnUpdateAvatarSelectCard" && parameter is AvatarBaseDefinition avatarBaseDefinition)
		{
			_avatarBaseDefinition = avatarBaseDefinition;
			UpdateUI();
		}
	}

	public void Show<T>(List<T> data) where T : AvatarBaseDefinition
	{
		Open();
		InternalHide();
		avatarListPanel.Init(data);
		applyButton.isEnabled = false;
	}

	private new void UpdateUI()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (_avatarBaseDefinition != null && playerModel != null)
		{
			bool flag = playerModel.IsAvatarUnlock(_avatarBaseDefinition);
			applyButton.isEnabled = flag;
			if (!flag)
			{
				ShowInfo(LocalizationManager.GetText("Achievement.CooperationUnlock.Title"));
			}
			else
			{
				InternalHide();
			}
		}
	}

	public void OnApplyButton()
	{
		if (_avatarBaseDefinition != null)
		{
			UIEvent.Send("OnApplyAvatarSelectCard", _avatarBaseDefinition);
		}
		Close();
	}

	private void ShowInfo(string text, bool isError = false)
	{
		InternalHide();
		SetInfoText(infoLabel, text);
		infoSprite.color = (isError ? errorColor : normalColor);
	}

	private void SetInfoText(UILabel label, string text)
	{
		if (label != null && label.gameObject != null)
		{
			label.gameObject.SetActive(value: true);
			label.text = text;
			CancelInvoke("InternalHide");
			Invoke("InternalHide", timeToShow);
		}
		else
		{
			Debug.LogError("HUDNotification: Could not show notification because label is NULL!");
		}
	}

	private void InternalHide()
	{
		if (infoLabel != null && infoLabel.gameObject != null)
		{
			infoLabel.gameObject.SetActive(value: false);
		}
	}
}
