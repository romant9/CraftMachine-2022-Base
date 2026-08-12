using System;
using System.Collections.Generic;

public class PlayerHubSocialListPanel : ScrollableListPanel<string>
{
	private List<string> items = new List<string>();

	protected override bool LastEntryAtTop => false;

	public PlayerHubSocialListPanel()
	{
		try
		{
			items.Add("Fb");
			items.Add("Instagram");
			items.Add("Twitter");
			items.Add("Forums");
			items.Add("Discord");
		}
		catch (Exception arg)
		{
			Debug.LogError($"PlayerHubSocialListPanel fail:{arg}");
		}
	}

	public void OnEnable()
	{
		SetCards(items);
	}
}
