using UnityEngine;

public class TwoOptionPopup : HUDElement
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel infoLabel;

	[SerializeField]
	private UILabel option1ButtonLabel;

	[SerializeField]
	private UILabel option2ButtonLabel;

	private Callback option1Callback;

	private Callback option2Callback;

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

	public void SetOption1ButtonLabel(string text)
	{
		if (option1ButtonLabel != null)
		{
			option1ButtonLabel.text = text;
		}
	}

	public void SetOption2ButtonLabel(string text)
	{
		if (option2ButtonLabel != null)
		{
			option2ButtonLabel.text = text;
		}
	}

	public void Option1Pressed()
	{
		Close();
		if (option1Callback != null)
		{
			option1Callback();
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	public void Option2Pressed()
	{
		Close();
		if (option2Callback != null)
		{
			option2Callback();
		}
		else
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	public void SetCallbacks(Callback option1Callback = null, Callback option2Callback = null)
	{
		this.option1Callback = option1Callback;
		this.option2Callback = option2Callback;
	}
}
