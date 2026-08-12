using UnityEngine;

public class SPRemoldCommonConfirmPopup : HUDElement
{
	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel contentLabel;

	[SerializeField]
	private UILabel okLabel;

	[SerializeField]
	private UILabel cancelLabel;

	private Callback OKcallBack;

	public override void Open()
	{
		base.Open();
	}

	public void SetContent(string titleLabel, string contentLabel, string okLabel, string cancelLabel)
	{
		this.titleLabel.text = titleLabel;
		this.contentLabel.text = contentLabel;
		this.okLabel.text = okLabel;
		this.cancelLabel.text = cancelLabel;
	}

	public void SetOKcallBack(Callback OKcallBack)
	{
		this.OKcallBack = OKcallBack;
	}

	public void OnClickConfirmButton()
	{
		if (OKcallBack != null)
		{
			OKcallBack();
		}
		Close();
	}
}
