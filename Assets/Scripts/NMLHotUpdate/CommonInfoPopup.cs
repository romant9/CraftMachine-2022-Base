using UnityEngine;

public class CommonInfoPopup : HUDElement
{
	[SerializeField]
	protected UILabel titleLabel;

	[SerializeField]
	protected UILabel infoLabel;

	[SerializeField]
	private GameObject closeArea;

	public override void Open()
	{
		base.Open();
		EnableCloseArea(enable: true);
	}

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

	public override void Close()
	{
		base.Close();
	}

	public void EnableCloseArea(bool enable)
	{
		Helpers.GameObjectSetActive(closeArea, enable);
	}
}
