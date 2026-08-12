using System;
using Client.Connectivity;
using UnityEngine;

public class CreditsPopup : HUDElement
{
	[Tooltip("The credits text file")]
	public TextAsset TextFile;

	[Tooltip("The credits text label")]
	public UILabel CreditsLabel;

	[Tooltip("The Scroll view object")]
	public UIScrollView ScrollView;

	private float StartPosY;

	public override void Start()
	{
		UpdateUI();
	}

	private string GetServerInfo()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			try
			{
				string[] array = new Uri(SignalRClient.Instance.CurrentHostPort).Host.ToUpper().Split('-');
				if (array.Length > 2)
				{
					return array[2];
				}
			}
			catch (Exception)
			{
			}
			return "OTHER";
		}
		return "OFFLINE";
	}

	public override void UpdateUI()
	{
		if (CreditsLabel != null && (bool)TextFile)
		{
			string text = GameStart.hotfixVersion;
			string serverInfo = GetServerInfo();
			if (serverInfo != "PRD")
			{
				text = text + " - " + serverInfo;
			}
			CreditsLabel.text = TextFile.text.Replace("{Version}", text);
			StartPosY = ScrollView.panel.transform.localPosition.y;
		}
	}

	public override void Update()
	{
		base.Update();
		if (ScrollView != null)
		{
			ScrollView.MoveRelative(new Vector3(0f, 1f, 0f));
		}
		if (IsCreditsDone(ScrollView))
		{
			Close();
		}
	}

	private bool IsCreditsDone(UIScrollView view)
	{
		if (CreditsLabel == null)
		{
			return false;
		}
		if (view.panel.transform.localPosition.y - StartPosY > (float)CreditsLabel.height - view.panel.GetViewSize().y)
		{
			return true;
		}
		return false;
	}
}
