using UnityEngine;

public class DeadlyInfoPopup : HUDElement
{
	[Header("Title Labels")]
	[SerializeField]
	private UILabel titleLabelsOne;

	[SerializeField]
	private UILabel titleLabelsTwo;

	[SerializeField]
	private UILabel titleLabelsThree;

	[Header("Info Labels")]
	[SerializeField]
	private UILabel rewardLabel;

	[SerializeField]
	private UILabel pointLabelOne;

	[SerializeField]
	private UILabel pointLabelTwo;

	[Header("Buttons")]
	[SerializeField]
	private UIButton okButton;

	public override void Open()
	{
		base.Open();
		UpdateUI();
	}

	public override void UpdateUI()
	{
		if (titleLabelsOne != null && titleLabelsTwo != null)
		{
			_ = titleLabelsThree != null;
		}
		if (rewardLabel != null && pointLabelOne != null)
		{
			_ = pointLabelTwo != null;
		}
	}

	public override void Close()
	{
		EventManager.NotifyClick("Close");
		EventManager.NotifyClick("Back");
		base.Close();
	}
}
