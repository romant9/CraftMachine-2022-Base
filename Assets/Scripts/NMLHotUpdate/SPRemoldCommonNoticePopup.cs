public class SPRemoldCommonNoticePopup : HUDElement
{
	public UILabel titleLabel;

	public UILabel contentLabel;

	public void SetContent(string title, string content)
	{
		titleLabel.text = title;
		contentLabel.text = content;
	}

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
	}
}
