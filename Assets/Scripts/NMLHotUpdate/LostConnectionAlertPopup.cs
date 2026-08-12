using System.Collections;
using Client.Connectivity;
using UnityEngine;

public class LostConnectionAlertPopup : AlertPopup
{
	public static void ShowPopup()
	{
		ShowPopup(LocalizationManager.GetText("Error.ConnectionLost"), LocalizationManager.GetText("Error.ConnectionLost.CheckConnection"), LocalizationManager.GetText("Button.Ok"));
	}

	public new static void ShowPopup(string title, string message, string okButtonLabel, Callback okCallback = null, Priority priority = Priority.Critical)
	{
		LostConnectionAlertPopup lostConnectionAlertPopup = null;
		if (SingularityMonoBehaviour<HUDManager>.Instance != null)
		{
			lostConnectionAlertPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.LostConnectionAlertPopup) as LostConnectionAlertPopup;
		}
		if (lostConnectionAlertPopup == null)
		{
			Debug.LogWarning("Alert popup not found!");
		}
		else if (lostConnectionAlertPopup.IsOpen && !lostConnectionAlertPopup.IsClosing && priority < lostConnectionAlertPopup.PrioritySetting)
		{
			Debug.LogError("Refusing to open popup over already open popup with a PrioritySetting of " + lostConnectionAlertPopup.PrioritySetting.ToString() + ". Pop-up ignored with title \"" + title + "\", message: " + message + " and priority of " + priority);
		}
		else
		{
			lostConnectionAlertPopup.PrioritySetting = priority;
			lostConnectionAlertPopup.SetContent(title, message);
			lostConnectionAlertPopup.SetOkButtonLabel(okButtonLabel);
			lostConnectionAlertPopup.SetCallbacks(lostConnectionAlertPopup.Close);
			lostConnectionAlertPopup.Open();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public override void Open()
	{
		base.Open();
		StartCoroutine(CheckConnectionCoroutine());
	}

	public override void Close()
	{
		if (SignalRClient.Instance.State == SignalRClientState.Connected && Application.internetReachability != NetworkReachability.NotReachable)
		{
			StartCoroutine(CloseWithDelay());
		}
	}

	public override void OnBackButtonClicked()
	{
		Close();
	}

	private IEnumerator CheckConnectionCoroutine()
	{
		while (SignalRClient.Instance.State != SignalRClientState.Connected || Application.internetReachability == NetworkReachability.NotReachable)
		{
			yield return null;
		}
		Close();
	}

	private IEnumerator CloseWithDelay()
	{
		float timeOnline = 0f;
		while (SignalRClient.Instance.State == SignalRClientState.Connected && Application.internetReachability != NetworkReachability.NotReachable)
		{
			timeOnline += Time.deltaTime;
			if (timeOnline >= 10f)
			{
				break;
			}
			yield return null;
		}
		if (timeOnline < 10f)
		{
			StartCoroutine(CheckConnectionCoroutine());
		}
		else
		{
			base.Close();
		}
	}
}
