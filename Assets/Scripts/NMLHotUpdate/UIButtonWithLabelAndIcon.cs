using UnityEngine;

public class UIButtonWithLabelAndIcon : UIButtonWithLabel
{
	[SerializeField]
	private UISprite firstIcon;

	[SerializeField]
	private UISprite secondIcon;

	[SerializeField]
	private UISprite thirdIcon;

	public void SetContentToIconOne(string spriteName, bool show = true)
	{
		SetContent(firstIcon, spriteName, show);
	}

	public void SetContentToIconTwo(string spriteName, bool show = true)
	{
		SetContent(secondIcon, spriteName, show);
	}

	public void SetContentToIconThree(string spriteName, bool show = true)
	{
		SetContent(thirdIcon, spriteName, show);
	}

	private void SetContent(UISprite sprite, string spriteName, bool show = true)
	{
		HelpersUI.SetSprite(sprite, spriteName, show);
	}
}
