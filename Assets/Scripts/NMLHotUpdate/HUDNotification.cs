using UnityEngine;

public class HUDNotification : HUDElement
{
	public delegate void InfoDelegate(string text);

	public delegate void ErrorDelegate(string text);

	public delegate void HideDelegate();

	[SerializeField]
	[Tooltip("Label show when there is some info")]
	private UILabel infoLabel;

	[SerializeField]
	[Tooltip("Label show when there is some error")]
	private UILabel errorLabel;

	[SerializeField]
	[Tooltip("Time to show in seconds")]
	private float timeToShow;

	public static event InfoDelegate InfoNotification;

	public static event ErrorDelegate ErrorNotification;

	public static event HideDelegate HideNotification;

	public static void Info(string text)
	{
		HUDNotification.InfoNotification?.Invoke(text);
	}

	public static void Error(string text)
	{
		HUDNotification.ErrorNotification?.Invoke(text);
	}

	public static void Hide()
	{
		HUDNotification.HideNotification?.Invoke();
	}

	public override void Open()
	{
		base.Open();
		InternalHide();
		InfoNotification -= ShowInfo;
		InfoNotification += ShowInfo;
		ErrorNotification -= ShowError;
		ErrorNotification += ShowError;
		HideNotification -= InternalHide;
		HideNotification += InternalHide;
	}

	public override void Close()
	{
		base.Close();
		InfoNotification -= ShowInfo;
		ErrorNotification -= ShowError;
		HideNotification -= InternalHide;
	}

	private void ShowInfo(string text)
	{
		InternalHide();
		SetText(infoLabel, text);
	}

	private void ShowError(string text)
	{
		InternalHide();
		SetText(errorLabel, text);
	}

	private void SetText(UILabel label, string text)
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
		if (errorLabel != null && errorLabel.gameObject != null)
		{
			errorLabel.gameObject.SetActive(value: false);
		}
	}
}
