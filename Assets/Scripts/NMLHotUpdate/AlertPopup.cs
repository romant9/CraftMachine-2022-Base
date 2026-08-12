using UnityEngine;

public class AlertPopup : HUDElement
{
	public enum Priority
	{
		Default = 0,
		Critical = 1
	}

	[SerializeField]
	protected UILabel titleLabel;

	[SerializeField]
	protected UILabel infoLabel;

	[SerializeField]
	protected UILabel okButtonLabel;

	protected Callback okCallback;

	protected static int sleepTimeoutSetting;

	[HideInInspector]
	public Priority PrioritySetting;

	public void SetContent(string title, string info)
	{
		if (title != null && titleLabel != null)
		{
			titleLabel.text = title;
		}
		if (info != null && infoLabel != null)
		{
			infoLabel.text = info;
		}
	}

	public void SetOkButtonLabel(string text)
	{
		if (okButtonLabel != null)
		{
			okButtonLabel.text = text;
		}
	}

	public virtual void OkPressed()
	{
		EventManager.NotifyClick("OkPressed");
		base.Close();
		if (okCallback != null)
		{
			okCallback();
		}
	}

	public override void OnBackButtonClicked()
	{
		OkPressed();
	}

	public void SetCallbacks(Callback okCallback = null)
	{
		this.okCallback = okCallback;
	}

	public bool HasCallback()
	{
		return okCallback != null;
	}

	public static void ShowPopup(string title, string message, string okButtonLabel, Callback okCallback = null, Priority priority = Priority.Default)
	{
		AlertPopup alertPopup = null;
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			alertPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.AlertPopup) as AlertPopup;
		}
		if (alertPopup == null)
		{
			Debug.LogWarning("Alert popup not found!");
		}
		else if (alertPopup.IsOpen && !alertPopup.IsClosing && priority < alertPopup.PrioritySetting)
		{
			Debug.LogError("Refusing to open popup over already open popup with a PrioritySetting of " + alertPopup.PrioritySetting.ToString() + ". Pop-up ignored with title \"" + title + "\", message: " + message + " and priority of " + priority);
		}
		else
		{
			alertPopup.PrioritySetting = priority;
			alertPopup.SetContent(title, message);
			alertPopup.SetOkButtonLabel(okButtonLabel);
			alertPopup.SetCallbacks(okCallback);
			alertPopup.Open();
		}
	}

	public static void ShowPopupGetText(string title, string message, string okButtonLabel, Callback okCallback, Priority priority = Priority.Default)
	{
		ShowPopup(LocalizationManager.GetText(title), LocalizationManager.GetText(message), LocalizationManager.GetText(okButtonLabel), okCallback, priority);
	}



	#region mycode
	public void SetTransform(Vector2 size, float offsetY, NGUIText.Alignment alignment)
	{
		infoLabel.transform.localPosition = new Vector3(infoLabel.transform.localPosition.x, offsetY, infoLabel.transform.localPosition.z);
		infoLabel.alignment = alignment;
		infoLabel.width = (int)size.x;
		infoLabel.height = (int)size.y;
	}
	#endregion
}
