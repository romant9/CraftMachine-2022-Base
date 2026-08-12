using UnityEngine;

public class SubscriptionInfoPopup : HUDElement
{
	public override void Open()
	{
		base.Open();
	}

	public override void Close()
	{
		base.Close();
	}

	public void OnClickWeek()
	{
		Application.OpenURL("https://decagames.com/privacy.html");
	}
}
